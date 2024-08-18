using System.Runtime.InteropServices;

namespace TestApp.Components
{
    internal class GameSystem
    {
        internal Configuration Configuration { get; }

        internal bool IsRunning { get; private set; }

        internal GameSystem(Configuration configuration)
        {
            Configuration = configuration;
            IsRunning = true;
        }

        internal void Initialise()
        {
            // TODO
        }

        internal void Quit()
        {
            IsRunning = false;
        }

        internal void CleanUp()
        {
            SDL2.SDL.SDL_Quit();
        }
    }
}
