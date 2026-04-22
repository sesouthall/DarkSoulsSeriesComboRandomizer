using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using System.IO;
using System.IO.Pipes;

namespace DarkSoulsSeriesComboRandomizer
{
    public class GameCoordinationServer(string dsrExePath, string ds2ExePath, string ds3ExePath) : IDisposable
    {
        private readonly DSRWrapper dsrWrapper = new(dsrExePath);
        private readonly DS2SotFSWrapper ds2Wrapper = new(ds2ExePath);
        private readonly DS3Wrapper ds3Wrapper = new(ds3ExePath);
        private bool disposedValue;

        public SoulsGame ActiveGame { get; private set; }

        public void Start(SoulsGame firstGame)
        {
            StartPipeServers();

            dsrWrapper.Start();
            dsrWrapper.Pause();
            ds2Wrapper.Start();
            ds2Wrapper.Pause();
            ds3Wrapper.Start();
            ds3Wrapper.Pause();

            dsrWrapper.OnModItemPickUp += SendItemToCorrectGame;
            ds2Wrapper.OnModItemPickUp += SendItemToCorrectGame;
            ds3Wrapper.OnModItemPickUp += SendItemToCorrectGame;

            switch (firstGame)
            {
                case SoulsGame.DSR:
                    dsrWrapper.Resume();
                    break;
                case SoulsGame.DS2S:
                    ds2Wrapper.Resume();
                    break;
                case SoulsGame.DS3:
                    ds3Wrapper.Resume();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(firstGame), $"{firstGame} is not a known SoulsGame");
            }
            ActiveGame = firstGame;
        }

        public async Task Stop()
        {
            dsrWrapper.OnModItemPickUp -= SendItemToCorrectGame;
            ds2Wrapper.OnModItemPickUp -= SendItemToCorrectGame;
            ds3Wrapper.OnModItemPickUp -= SendItemToCorrectGame;

            await dsrWrapper.WaitForShutdown();
            await ds2Wrapper.WaitForShutdown();
            await ds3Wrapper.WaitForShutdown();
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
                        SwitchGame(SoulsGame.DSR, destinationBonfire);
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
                        SwitchGame(SoulsGame.DS2S, destinationBonfire);
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
                        SwitchGame(SoulsGame.DS3, destinationBonfire);
                    }
                }
            });
        }

        private void SwitchGame(SoulsGame currentGame, int destinationBonfire)
        {
            switch (currentGame)
            {
                case SoulsGame.DSR:
                    dsrWrapper.Pause();
                    break;
                case SoulsGame.DS2S:
                    ds2Wrapper.Pause();
                    break;
                case SoulsGame.DS3:
                    ds3Wrapper.Pause();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentGame), $"{currentGame} is not a known SoulsGame");
            }

            if (destinationBonfire >= 1002960 && destinationBonfire <= 1812961)
            {
                ActiveGame = SoulsGame.DSR;
                dsrWrapper.Resume();
                dsrWrapper.Warp(destinationBonfire);
            }
            else if (destinationBonfire >= 2650 && destinationBonfire <= 37685)
            {
                ActiveGame = SoulsGame.DS2S;
                ds2Wrapper.Resume();
                ds2Wrapper.Warp(destinationBonfire);
            }
            else if (destinationBonfire >= 3002950 && destinationBonfire <= 5112951)
            {
                ActiveGame = SoulsGame.DS3;
                ds3Wrapper.Resume();
                ds3Wrapper.Warp(destinationBonfire);
            }
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