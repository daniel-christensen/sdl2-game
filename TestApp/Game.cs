using TestApp.Helper;
using TestApp.Messages;
using Serilog;
using TestApp.Components;

namespace TestApp
{
    internal class Game
    {
        internal Configuration Configuration;

        internal Graphics Graphics { get; }

        internal EventPoller EventPoller { get; }

        internal GameSystem GameSystem { get; }

        private Game()
        {
            Configuration = new Configuration();
            Graphics = new Graphics(Configuration);
            GameSystem = new GameSystem(Configuration);
            EventPoller = new EventPoller(Configuration, GameSystem);
        }

        internal void Run()
        {
            try
            {
                // Console is not essential for runtime - just nice to have while debugging.
                if (!ConsoleManager.IsConsoleAllocated())
                    Log.Information(GameLoggingMessage.GameInstanceWithoutConsole);

                Initialise();
                while (GameSystem.IsRunning)
                {
                    UpdateLogic();
                    UpdateDisplay();
                }
            }
            catch (Exception e)
            {
                Log.Fatal(GameExceptionMessage.FatalUncaught);
            }
            finally
            {
                CleanUp();
            }
        }

        private void Initialise()
        {
            Configuration.Initialise();
            Graphics.Initialise();
            GameSystem.Initialise();
            EventPoller.Initialise();
        }

        private void UpdateLogic()
        {
            EventPoller.PollEvents();
        }

        private void UpdateDisplay()
        {
            Graphics.Display();
        }

        private void CleanUp()
        {
            Graphics.CleanUp();
            GameSystem.CleanUp();
        }

        internal static Game NewGame()
        {
            return new Game();
        }
    }
}
