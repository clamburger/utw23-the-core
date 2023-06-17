using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PCSC;
using PCSC.Exceptions;
using PCSC.Iso7816;
using PCSC.Monitoring;
using PCSC.Reactive.Events;
using PCSC.Utils;
using Spectre.Console;
using System.Formats.Asn1;
using System.Reactive.Linq;

namespace UbertweakNfcReaderWeb.Services
{
    [Flags]
    public enum ReaderBehaviour
    {
        None = 0,
        LedBlinkedOnAccess = 1,
        InterfacePolling = 2,
        InterfaceActivated = 4,
        BeepOnInsertion = 8,
        BeepOnRemoval = 16,
        BeepOnPowerOn = 32,
        ColorSelectGreen = 64,
        ColorSelectRed = 128
    }
    
    public class NfcService
    {
        private const IntPtr PeripheralControlCode = 0x310000 + 3500 * 4;
        private readonly ILogger _logger;
        private IDeviceMonitor? _deviceMonitor { get; set; }
        private ISCardMonitor? _cardMonitor { get; set; }
        private readonly IContextFactory _contextFactory;

        public event EventHandler<CardInsertedEventArgs>? CardInserted;
        public event EventHandler<CardStatusEventArgs>? CardRemoved;

        public NfcService(ILogger<NfcService> logger)
        {
            _logger = logger;
            _contextFactory = ContextFactory.Instance;
        }

        public void Watch()
        {
            using (var ctx = _contextFactory.Establish(SCardScope.System))
            {
                var factory = DeviceMonitorFactory.Instance;

                _deviceMonitor = factory.Create(SCardScope.System);
                _deviceMonitor.StatusChanged += DeviceStatusChanged;
                _deviceMonitor.Initialized += DeviceMonitorInitialized;
                _deviceMonitor.MonitorException += DeviceMonitorException;
                _deviceMonitor.Start();
            }
        }

        private void DeviceStatusChanged(object sender, DeviceChangeEventArgs e)
        {
            foreach (var reader in e.DetachedReaders)
            {
                AnsiConsole.MarkupLine($"[grey]An NFC reader has been disconnected: {reader}[/]");
            }

            foreach (var reader in e.AttachedReaders)
            {
                AnsiConsole.MarkupLine($"[yellow]An NFC reader has been connected: {reader}[/]");
            }

            ReinitializeCardMonitor(e.AllReaders.ToArray());
        }

        private void DeviceMonitorInitialized(object sender, DeviceChangeEventArgs e)
        {
            ReinitializeCardMonitor(e.AllReaders.ToArray());
        }

        private void DeviceMonitorException(object sender, DeviceMonitorExceptionEventArgs e)
        {
            AnsiConsole.MarkupLineInterpolated($"Device monitor encountered an error: {e.Exception.Message}");
        }

        private void VerifyReaderSettings(string readerName)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]Firmware version: {GetFirmwareVersion(readerName)}[/]");
            var behaviour = GetReaderBehaviour(readerName);
            AnsiConsole.MarkupLineInterpolated($"[yellow]Behaviour status: {behaviour}[/]");

            AnsiConsole.MarkupLineInterpolated($"[yellow]Setting desired reader behaviour...[/]");
            behaviour |= ReaderBehaviour.BeepOnPowerOn;
            behaviour |= ReaderBehaviour.BeepOnInsertion;
            behaviour &= ~ReaderBehaviour.BeepOnRemoval;
            UpdateReaderBehaviour(readerName, behaviour);
            
            var newBehaviour = GetReaderBehaviour(readerName);
            AnsiConsole.MarkupLineInterpolated($"[yellow]New behaviour status: {newBehaviour}[/]");
        }

        private void ReinitializeCardMonitor(string[] readers)
        {
            foreach (var reader in readers.Where(name => name.Contains("PICC")))
            {
                VerifyReaderSettings(reader);
            }
            
            if (_cardMonitor != null)
            {
                _cardMonitor.Cancel();
                _cardMonitor.Dispose();
            }

            if (!readers.Any())
            {
                AnsiConsole.MarkupLine("[red slowblink bold][[!!]] No NFC readers detected![/]");
                return;
            }

            _cardMonitor = new SCardMonitor(_contextFactory, SCardScope.System);

            // Point the callback function(s) to the anonymous & static defined methods below.
            _cardMonitor.CardInserted += CardInsertedInternal;
            _cardMonitor.CardRemoved += CardRemovedInternal;
            _cardMonitor.MonitorException += CardMonitorException;
            _cardMonitor.Start(readers);

            AnsiConsole.MarkupLine("[white]NFC reader detected. Ready to read cards.[/]");
        }

        private void CardInsertedInternal(object sender, CardStatusEventArgs e)
        {
            var uid = GetCardUid(e.ReaderName);

            CardInserted?.Invoke(this, new CardInsertedEventArgs
            {
                CardStatusEventArgs = e,
                Uid = uid
            });

            AnsiConsole.MarkupLineInterpolated($"[lime]Card detected: {(uid == null ? "unknown" : uid)}[/]");
        }

        public string GetFirmwareVersion(string readerName)
        {
            using var ctx = _contextFactory.Establish(SCardScope.System);
            using var reader = new SCardReader(ctx);
            reader.Connect(readerName, SCardShareMode.Direct, SCardProtocol.Unset);

            // 5.4.1. Get Firmware Version
            var sendBytes = new byte[] { 0xE0, 0x00, 0x00, 0x18, 0x00 };
            var receiveBytes = new byte[255];

            reader.Control(PeripheralControlCode, sendBytes, ref receiveBytes);
            
            var responseContent = receiveBytes.Skip(5).Take(receiveBytes[4]).ToArray();
            
            return System.Text.Encoding.Default.GetString(responseContent);
        }

        public ReaderBehaviour GetReaderBehaviour(string readerName)
        {
            using var ctx = _contextFactory.Establish(SCardScope.System);
            using var reader = new SCardReader(ctx);
            reader.Connect(readerName, SCardShareMode.Direct, SCardProtocol.Unset);

            // 5.4.6. Read LED and Buzzer Status Indicator Behavior for PICC Interface
            var sendBytes = new byte[] { 0xE0, 0x00, 0x00, 0x21, 0x00 };
            var receiveBytes = new byte[6];

            reader.Control(PeripheralControlCode, sendBytes, ref receiveBytes);

            var behaviour = receiveBytes.Skip(5).First();
            
            return (ReaderBehaviour) behaviour;
        }

        public void UpdateReaderBehaviour(string readerName, ReaderBehaviour behaviour)
        {
            using var ctx = _contextFactory.Establish(SCardScope.System);
            using var reader = new SCardReader(ctx);
            reader.Connect(readerName, SCardShareMode.Direct, SCardProtocol.Unset);
            
            // 5.4.5. Set LED and Buzzer Status Indicator Behavior for PICC Interface
            var sendBytes = new byte[] { 0xE0, 0x00, 0x00, 0x21, 0x01, (byte)behaviour };
            var receiveBytes = new byte[6];
            
            reader.Control(PeripheralControlCode, sendBytes, ref receiveBytes);
        }

        public string? GetCardUid(string readerName)
        {
            using var ctx = _contextFactory.Establish(SCardScope.System);
            using var isoReader = new IsoReader(ctx, readerName, SCardShareMode.Shared, SCardProtocol.Any, false);

            var apdu = new CommandApdu(IsoCase.Case2Short, isoReader.ActiveProtocol)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.GetData,
                P1 = 0x00,
                P2 = 0x00,
                Le = 0
            };

            var response = isoReader.Transmit(apdu);
            return response.HasData ? BitConverter.ToString(response.GetData()) : null;
        }

        private void CardRemovedInternal(object sender, CardStatusEventArgs e)
        {
            CardRemoved?.Invoke(this, e);

            AnsiConsole.MarkupLine("[grey]Card removed.[/]");
        }

        private static void CardMonitorException(object sender, PCSCException ex)
        {
            AnsiConsole.MarkupLineInterpolated($"Card monitor exited due an error: {SCardHelper.StringifyError(ex.SCardError)}");
        }

        public void StopWatch()
        {
            _deviceMonitor?.Cancel();
            _deviceMonitor?.Dispose();

            _cardMonitor?.Cancel();
            _cardMonitor?.Dispose();
        }
    }

    public class CardInsertedEventArgs
    {
        public required CardStatusEventArgs CardStatusEventArgs { get; set; }
        public string? Uid { get; set; }
    }
}
