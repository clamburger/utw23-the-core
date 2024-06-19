using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Connections;
using Spectre.Console;
using UbertweakNfcReaderWeb.Messages;

namespace UbertweakNfcReaderWeb;

public class Scanner
{
    private ConnectionContext _connection;
    public ScannerState? State { get; set; }
    public int? StationId { get; set; }
    public string? CardUid { get; set; }
    public int? SelectedOption { get; set; }
    public string? IpAddress { get; set; }
    public string ConnectionId { get; set; }
    public DateTime LastSeen { get; set; }
    
    public Scanner(ConnectionContext connection)
    {
        _connection = connection;
        if (connection.RemoteEndPoint is IPEndPoint ip)
        {
            IpAddress = ip.Address.ToString();            
        }
        ConnectionId = connection.ConnectionId;
    }

    public void UpdateFromMessage(ClientMessage message)
    {
        StationId = message.StationId;
        LastSeen = DateTime.Now;

        switch (message)
        {
            case Heartbeat heartbeat:
                State = heartbeat.State;
                if (State is ScannerState.Disabled or ScannerState.Ready)
                {
                    CardUid = null;
                    SelectedOption = null;
                }
                break;
            case CardRead cardRead:
                CardUid = cardRead.Uid.Replace(" ", "-");
                SelectedOption = null;
                break;
            case OptionSelected optionSelected:
                CardUid = optionSelected.Uid;
                SelectedOption = optionSelected.OptionNumber;
                break;
        }
    }

    public ConnectionContext GetConnection()
    {
        return _connection;
    }

    public async Task SendMessage(ServerMessage message)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        
        var messageJson = JsonSerializer.Serialize<Message>(message, jsonOptions)
            .Replace(",", ", ");
            
        // Remove type property
        messageJson = Regex.Replace(messageJson, """
                                                 "type":\d+,\s
                                                 """, "");
        
        AnsiConsole.MarkupLineInterpolated($"[blue]{ConnectionId}[/] [yellow]<- {messageJson}[/] [red]{message.GetType().Name}[/]");
            
        // _logger.LogInformation("Sending message to {ConnectionId}: {Message}", scanner.ConnectionId, messageJson);
            
        var buffer = Encoding.UTF8.GetBytes(messageJson + "\n");
        await _connection.Transport.Output.WriteAsync(buffer);
        
        Thread.Sleep(500);
    }
}