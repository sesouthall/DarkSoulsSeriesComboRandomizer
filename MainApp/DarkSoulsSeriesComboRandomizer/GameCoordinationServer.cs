using System.IO;
using System.IO.Pipes;

namespace DarkSoulsSeriesComboRandomizer
{
    public class GameCoordinationServer(List<SoulsGameWrapper> gameWrappers, CrossGameMappings crossGameMappings, Action<string> sendMessageToPlayer) : IDisposable
    {
        private bool disposedValue;
        private readonly List<Task> warpTasks = [];
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private bool allGamesStarted = false;

        public event EventHandler<EventArgs>? GameClosed;
        public SoulsGameWrapper ActiveGame { get; private set; } = gameWrappers[0];

        public async Task Start(SoulsGame firstGame)
        {
            StartPipeServers();

            foreach (var game in gameWrappers)
            {
                await game.Start();
                game.Exited += OnGameClosed;
                game.Pause();
                await Task.Delay(500);
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
            allGamesStarted = true;
        }

        public async Task Stop()
        {
            allGamesStarted = false;

            foreach (var gameWrapper in gameWrappers)
            {
                gameWrapper.OnModItemPickUp -= SendItemToCorrectGame;
            }

            foreach(var gameWrapper in gameWrappers)
            {
                await gameWrapper.WaitForShutdown();
            }
        }

        private void OnGameClosed(SoulsGameWrapper exitedWrapper)
        {
            GameClosed?.Invoke(this, new EventArgs());
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
                        if (!allGamesStarted)
                        {
                            sendMessageToPlayer("Cannot warp until characters are loaded in all three games and the start menu has been opened for each of them.");
                        }
                        else if (int.TryParse(line, out var destinationBonfire))
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