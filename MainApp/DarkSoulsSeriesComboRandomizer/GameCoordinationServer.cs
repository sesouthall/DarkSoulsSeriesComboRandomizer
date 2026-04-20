using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using System.IO.Pipes;

namespace DarkSoulsSeriesComboRandomizer
{
    public class GameCoordinationServer : IDisposable
    {
        private readonly DSRWrapper dsrWrapper;
        private readonly DS2SotFSWrapper ds2Wrapper;
        private readonly DS3Wrapper ds3Wrapper;
        private bool disposedValue;

        public GameCoordinationServer(string dsrExePath, string ds2ExePath, string ds3ExePath)
        {
            this.dsrWrapper = new DSRWrapper(dsrExePath);
            this.ds2Wrapper = new DS2SotFSWrapper(ds2ExePath);
            this.ds3Wrapper = new DS3Wrapper(ds3ExePath);
        }

        public void Start(CancellationToken cancellationToken)
        {
            StartPipeServers();

            dsrWrapper.Start();
            Thread.Sleep(15000);
            dsrWrapper.Pause();
            ds2Wrapper.Start();
            Thread.Sleep(15000);
            ds2Wrapper.Pause();
            ds3Wrapper.Start();
            Thread.Sleep(15000);
            ds3Wrapper.Pause();

            dsrWrapper.OnModItemPickUp += SendItemToCorrectGame;
            ds2Wrapper.OnModItemPickUp += SendItemToCorrectGame;
            ds3Wrapper.OnModItemPickUp += SendItemToCorrectGame;

            dsrWrapper.Resume();

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(10000);
            }

            dsrWrapper.OnModItemPickUp -= SendItemToCorrectGame;
            ds2Wrapper.OnModItemPickUp -= SendItemToCorrectGame;
            ds3Wrapper.OnModItemPickUp -= SendItemToCorrectGame;

            dsrWrapper.Resume();
            ds2Wrapper.Resume();
            ds3Wrapper.Resume();
        }

        private void StartPipeServers()
        {
            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream("DarkSoulsSeriesComboRandomizerDS1", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                while (true)
                {
                    var line = reader.ReadLine()?.Trim()?.Trim('\0', '\v', '\b');
                    if (int.TryParse(line, out var destinationBonfire))
                    {
                        dsrWrapper.Pause();

                        if (destinationBonfire >= 1002960 && destinationBonfire <= 1812961)
                        {
                            dsrWrapper.Resume();
                            dsrWrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 2650 && destinationBonfire <= 37685)
                        {
                            ds2Wrapper.Resume();
                            ds2Wrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 3002950 && destinationBonfire <= 5112951)
                        {
                            ds3Wrapper.Resume();
                            ds3Wrapper.Warp(destinationBonfire);
                        }
                    }
                }
            });

            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream("DarkSoulsSeriesComboRandomizerDS2", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                while (true)
                {
                    var line = reader.ReadLine()?.Trim()?.Trim('\0', '\v', '\b');
                    if (int.TryParse(line, out var destinationBonfire))
                    {
                        ds2Wrapper.Pause();

                        if (destinationBonfire >= 1002960 && destinationBonfire <= 1812961)
                        {
                            dsrWrapper.Resume();
                            dsrWrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 2650 && destinationBonfire <= 37685)
                        {
                            ds2Wrapper.Resume();
                            ds2Wrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 3002950 && destinationBonfire <= 5112951)
                        {
                            ds3Wrapper.Resume();
                            ds3Wrapper.Warp(destinationBonfire);
                        }
                    }
                }
            });

            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream("DarkSoulsSeriesComboRandomizerDS3", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                while (true)
                {
                    var line = reader.ReadLine()?.Trim()?.Trim('\0', '\v', '\b');
                    if (int.TryParse(line, out var destinationBonfire))
                    {
                        Thread.Sleep(500);
                        ds3Wrapper.Pause();

                        if (destinationBonfire >= 1002960 && destinationBonfire <= 1812961)
                        {
                            dsrWrapper.Resume();
                            dsrWrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 2650 && destinationBonfire <= 37685)
                        {
                            ds2Wrapper.Resume();
                            ds2Wrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 3002950 && destinationBonfire <= 5112951)
                        {
                            ds3Wrapper.Resume();
                            ds3Wrapper.Warp(destinationBonfire);
                        }
                    }
                }
            });
        }

        private void SendItemToCorrectGame(SoulsGame sourceGame, int itemId, int quantity)
        {
            var originalItem = CrossGameMappings.GetSourceItem(itemId, sourceGame);

            switch (originalItem.Game)
            {
                case SoulsGame.DSR:
                    dsrWrapper.GiveItem(originalItem.ItemType, originalItem.OriginalId, quantity);
                    break;
                case SoulsGame.DS2S:
                    ds2Wrapper.GiveItem(originalItem.OriginalId, (short)quantity);
                    break;
                case SoulsGame.DS3:
                    ds3Wrapper.GiveItem(originalItem.ItemType, originalItem.OriginalId, quantity);
                    break;
                default:
                    throw new NotImplementedException();
            }
            ;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dsrWrapper.Dispose();
                    ds2Wrapper.Dispose();
                    ds3Wrapper.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}