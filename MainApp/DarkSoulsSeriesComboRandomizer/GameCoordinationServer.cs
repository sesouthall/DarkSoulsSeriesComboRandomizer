using System.IO;
using System.IO.Pipes;

namespace DarkSoulsSeriesComboRandomizer
{
    public class GameCoordinationServer(List<SoulsGameWrapper> gameWrappers, CrossGameMappings crossGameMappings) : IDisposable
    {
        private bool disposedValue;
        private readonly List<Task> warpTasks = [];
        private readonly CancellationTokenSource cancellationTokenSource = new();

        public SoulsGameWrapper ActiveGame { get; private set; } = gameWrappers[0];

        public async Task Start(SoulsGame firstGame)
        {
            StartPipeServers();

            foreach (var game in gameWrappers)
            {
                await game.Start();
                game.Pause();
            }

            foreach (var game in gameWrappers)
            {
                // This has to be after each game has been started so we don't try to send items to a game that doesn't exist yet
                game.OnModItemPickUp += SendItemToCorrectGame;
            }

            var firstGameWrapper = gameWrappers.FirstOrDefault(game => game.Game == firstGame) ??
                throw new ArgumentOutOfRangeException(nameof(firstGame), $"{firstGame} is not a known SoulsGame");
            firstGameWrapper.Resume();
            ActiveGame = firstGameWrapper;
        }

        public async Task Stop()
        {
            foreach (var gameWrapper in gameWrappers)
            {
                gameWrapper.OnModItemPickUp -= SendItemToCorrectGame;
            }

            foreach(var gameWrapper in gameWrappers)
            {
                await gameWrapper.WaitForShutdown();
            }
        }

        private void StartPipeServers()
        {
            foreach(var gameWrapper in gameWrappers)
            {
                warpTasks.Add(Task.Factory.StartNew(async () =>
                {
                    var server = new NamedPipeServerStream(
                        $"DarkSoulsSeriesComboRandomizer{gameWrapper.Game}",
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Message);
                    await server.WaitForConnectionAsync(cancellationTokenSource.Token);
                    StreamReader reader = new(server);
                    while (server.IsConnected)
                    {
                        var line = reader.ReadLine()?.Trim()?.Trim('\0', '\v', '\b');
                        if (int.TryParse(line, out var destinationBonfire))
                        {
                            SwitchGame(destinationBonfire);
                        }
                        Thread.Sleep(500);
                    }
                }));
            }
        }

        private void SwitchGame(int destinationBonfire)
        {
            ActiveGame.Pause();

            Thread.Sleep(500);

            ActiveGame = gameWrappers.FirstOrDefault(game => game.ContainsBonfireId(destinationBonfire)) ??
                throw new ArgumentOutOfRangeException(nameof(destinationBonfire), $"{destinationBonfire} is not recognized as a bonfire id in any running game");
            ActiveGame.Resume();
            ActiveGame.Warp(destinationBonfire);
        }

        private void SendItemToCorrectGame(SoulsGame sourceGame, int itemId, int quantity)
        {
            var originalItem = crossGameMappings.GetSourceItem(itemId, sourceGame);

            var originalGame = gameWrappers.FirstOrDefault(game => game.Game == originalItem.Game) ??
                throw new ArgumentOutOfRangeException(nameof(itemId), $"Item with id {itemId} from {sourceGame} should map to {originalItem.Id} in {originalItem.Game}, but that game isn't being randomized.");
            originalGame.GiveItem(originalItem.Type, originalItem.Id, quantity);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var task in warpTasks)
                    {
                        cancellationTokenSource.Cancel();
                        task.Wait();
                        task.Dispose();
                    }
                    warpTasks.Clear();
                    foreach (var game in gameWrappers)
                    {
                        game.Dispose();
                        // Give each game a second to shut down.
                        // Occasionally, DS3 will hang when resuming.
                        Thread.Sleep(500);
                    }
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