using Microsoft.AspNetCore.Connections;
using UbertweakNfcReaderWeb.Messages;

namespace UbertweakNfcReaderWeb.Services;

public class ScannerService
{
    private readonly ILogger<ScannerService> _logger;
    
    private readonly List<Scanner> _scanners = new();

    public ScannerService(ILogger<ScannerService> logger)
    {
        _logger = logger;
    }
    
    public async Task<Scanner> AddScanner(ConnectionContext connection)
    {
        var scanner = new Scanner(connection);
        
        // Check if scanner with this IP already exists
        _scanners.FindAll(s => s.IpAddress == scanner.IpAddress).ForEach(existing => _scanners.Remove(existing));
        
        _scanners.Add(scanner);
        Thread.Sleep(100);
        await UpdateSettings(scanner);

        return scanner;
    }
    
    public void RemoveScanner(Scanner scanner)
    {
        _scanners.Remove(scanner);
    }

    public void RemoveScanner(ConnectionContext connection)
    {
        var scanner = _scanners.Find(s => s.GetConnection() == connection);
        if (scanner != null)
        {
            _scanners.Remove(scanner);            
        }
    }

    public List<Scanner> GetScanners()
    {
        return _scanners;
    }

    public async Task UpdateAllSettings()
    {
        foreach (var scanner in _scanners)
        {
            await UpdateSettings(scanner);
        }
    }

    public async Task UpdateSettings(Scanner scanner)
    {
        using var db = new DatabaseContext();

        var options = db.VoteOptions.OrderBy(option => option.Number).ToList();
        
        await scanner.SendMessage(new SetNumberOfOptions
        {
            OptionCount = options.Count
        });

        foreach (var option in options)
        {
            await scanner.SendMessage(new SetOptionTitle
            {
                OptionNumber = option.Number,
                Text = option.Name
            });

            if (!option.Enabled)
            {
                await scanner.SendMessage(new SetOptionEnabled
                {
                    OptionNumber = option.Number,
                    Enabled = 0
                });
            }
        }
    }
    
    public async Task SendToAll(ServerMessage message)
    {
        foreach (var scanner in _scanners)
        {
            await Task.Run(() => scanner.SendMessage(message, false));
        }
        
        Thread.Sleep(1000);
    }

    public void DisconnectScanner(string connectionId)
    {
        var scanner = _scanners.Find(s => s.ConnectionId == connectionId);

        if (scanner == null)
        {
            return;
        }
        
        scanner.GetConnection().Abort();
        _scanners.Remove(scanner);
    }

    public void DropAll()
    {
        foreach (var scanner in _scanners)
        {
            scanner.GetConnection().Abort();
        }
        
        _scanners.Clear();
    }
}