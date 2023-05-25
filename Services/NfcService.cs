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
    public class NfcService
    {
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
                AnsiConsole.MarkupLine($"[grey]An NFC reader has been connected: {reader}[/]");
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

        private void ReinitializeCardMonitor(string[] readers)
        {
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
