using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using UbertweakNfcReaderWeb.Hubs;
using UbertweakNfcReaderWeb.Messages;
using UbertweakNfcReaderWeb.Models;
using UbertweakNfcReaderWeb.Services;

namespace UbertweakNfcReaderWeb;

public class TcpConnectionHandler : ConnectionHandler
{
    private readonly ILogger<TcpConnectionHandler> _logger;
    private readonly ScannerService _connectionManager;
    private readonly PlexusService _plexus;
    private readonly IHubContext<MessageHub, IMessageClient> _hubContext;

    public TcpConnectionHandler(
        ILogger<TcpConnectionHandler> logger,
        ScannerService connectionManager,
        PlexusService plexus,
        IHubContext<MessageHub, IMessageClient> hubContext)
    {
        _logger = logger;
        _connectionManager = connectionManager;
        _plexus = plexus;
        _hubContext = hubContext;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        _logger.LogInformation("{ConnectionId} connected from {IpAddress}", connection.ConnectionId, connection.RemoteEndPoint?.ToString());
        
        if (_plexus.PrimaryConnection != null)
        {
            await _hubContext.Clients.Client(_plexus.PrimaryConnection).ScannerUpdate();
        }

        try
        {
            await StartMonitor(connection);
        }
        catch (ConnectionAbortedException e)
        {
            _connectionManager.RemoveScanner(connection);
        }
        
        _logger.LogInformation("Connection {ConnectionId} disconnected", connection.ConnectionId);
    }

    public async Task StartMonitor(ConnectionContext connection)
    {
        var scanner = await _connectionManager.AddScanner(connection);
        var stringBuffer = "";
        
        while (true)
        {
            var result = await connection.Transport.Input.ReadAsync();
            var buffer = result.Buffer;
        
            foreach (var segment in buffer)
            {
                var message = Encoding.UTF8.GetString(segment.Span);
                
                // _logger.LogInformation("Received message portion from {ConnectionId}: {Message}", connection.ConnectionId, message);

                stringBuffer += message;

                if (!stringBuffer.EndsWith("\n"))
                {
                    continue;
                }
                
                Message? jsonObject;
                
                var assembledMessage = stringBuffer.TrimEnd();
                
                stringBuffer = "";

                // if (assembledMessage.Contains("uid"))
                // {
                //     // Fix uid field not having quotes around it
                //     assembledMessage =
                //         Regex.Replace(assembledMessage, "\"uid\": (([0-9A-F]{2} ?)+)", "\"uid\": \"$1\"");
                // }

                // if (assembledMessage.Contains("option"))
                // {
                //     assembledMessage = assembledMessage.Replace("\"type\": 2", "\"type\": 3");
                // }
                
                AnsiConsole.MarkupInterpolated($"[blue]{connection.ConnectionId}[/] [aqua]-> {assembledMessage}[/]");
                
                try
                {
                    jsonObject = JsonSerializer.Deserialize<Message>(assembledMessage);                    
                } catch (JsonException e)
                {
                    AnsiConsole.MarkupLineInterpolated($"[red]Invalid[/]");
                    _logger.LogInformation("Invalid message from {ConnectionId}: {Message}", connection.ConnectionId, e.Message);
                    continue;
                }
                
                if (jsonObject == null)
                {
                    AnsiConsole.MarkupLineInterpolated($"[red]Invalid[/]");
                    _logger.LogInformation("Invalid message from {ConnectionId} (null)", connection.ConnectionId);
                    continue;
                }
                
                AnsiConsole.MarkupLineInterpolated($" [red]{jsonObject.GetType().Name}[/]");
                
                // _logger.LogInformation(
                //     "Received {Type} message from {ConnectionId}",
                //     jsonObject.GetType(),
                //     connection.ConnectionId
                // );

                if (jsonObject is not ClientMessage clientMessage)
                {
                    _logger.LogInformation("Invalid message from {ConnectionId} (not a client message)", connection.ConnectionId);
                    continue;
                }
                
                scanner.UpdateFromMessage(clientMessage);
                
                if (clientMessage is CardRead cardRead)
                {
                    using (var db = new DatabaseContext())
                    {
                        var card = db.Cards.Include(card => card.User)
                            .SingleOrDefault(c => c.Uid == cardRead.Uid.Trim().Replace(" ", "-"));
                        
                        if (card?.User == null)
                        {
                            await scanner.SendMessage(new SetState
                            {
                                State = ScannerState.InvalidCard
                            });
                        }
                        else
                        {
                            await scanner.SendMessage(new UserDetails
                            {
                                Name = card.User!.Name
                            });
                            
                            await scanner.SendMessage(new SetState
                            {
                                State = ScannerState.ReadyToSelect
                            });
                        }
                    }
                }

                if (clientMessage is OptionSelected optionSelected)
                {
                    using (var db = new DatabaseContext())
                    {
                        var card = db.Cards.Include(card => card.User)
                            .SingleOrDefault(c => c.Uid == optionSelected.Uid.Trim().Replace(" ", "-"));
                        
                        var option = db.VoteOptions.FirstOrDefault(option => option.Number == optionSelected.OptionNumber);

                        if (card?.User == null)
                        {
                            _logger.LogWarning("Invalid card");
                            continue;
                        }

                        if (option == null)
                        {
                            _logger.LogWarning("Invalid option");
                            continue;
                        }
                        
                        // create or update UserVote record
                        var userVote = db.UserVotes.FirstOrDefault(vote => vote.User == card.User);
                        if (userVote == null)
                        {
                            userVote = new UserVote
                            {
                                Option = option,
                                User = card.User!,
                                DateTime = DateTime.Now
                            };
                            
                            db.UserVotes.Add(userVote);
                        }
                        else
                        {
                            userVote.Option = option;
                            userVote.DateTime = DateTime.Now;
                        }
                        
                        await db.SaveChangesAsync();
                    }
                    
                    await scanner.SendMessage(new SetState
                    {
                        State = ScannerState.OptionSelected
                    });
                }
                
                if (_plexus.PrimaryConnection != null)
                {
                    await _hubContext.Clients.Client(_plexus.PrimaryConnection).ScannerUpdate();
                }
            }
        
            if (result.IsCompleted)
            {
                _logger.LogInformation("Connection {ConnectionId} completed", connection.ConnectionId);
                break;
            }
        
            connection.Transport.Input.AdvanceTo(buffer.End);
        }
        
        _connectionManager.RemoveScanner(scanner);
    }
    
    private static Random _random = new Random();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMnopqrstuvwxyz   ";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
