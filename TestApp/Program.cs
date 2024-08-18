using static SDL2.SDL;
using TestApp.Helper;

namespace TestApp
{
    // TO REMOVE
    internal static class RectToDraw
    {
        internal static int x = 0;
        internal static int y = 0;
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            // CODING GUIDLINES
            // 1. "Game" will be the base class, encapsulating all componenets of a game instance
            // 2. "Game" should not directly talk to SDL2, WIN32 or any other library
            // 3. All calls to SDL2 and other libraries should be encapsulated into other classes, such as "Graphics"
            // 4. Throw exceptions and let the outer try method catch them if the intention is to cause a FATAL error
            // 5. The extension exception Log.Logger.SDLError() should be used instead of SDL_GetError()

            ConsoleManager.Initialise();
            LogHelper.Initialise();
            Game.NewGame().Run();
        }
    }
}
