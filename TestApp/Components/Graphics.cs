using Serilog;
using TestApp.Messages;
using TestApp.Extensions;
using static SDL2.SDL;
using TestApp.Interfaces;
using TestApp.Helper;

namespace TestApp.Components
{
    internal class Graphics : IComponent, IRegisterable
    {
        public Game Game { get; }

        private nint _window;

        private nint _renderer;

        public Graphics(Game game)
        {
            Game = game;
        }

        public void Initialise()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                Log.Logger.SDLError();
                throw new InvalidOperationException(GraphicsExceptionMessage.SDL2InitError);
            }

            _window = SDL_CreateWindow(
                Game.Configuration.WindowTitle,
                Game.Configuration.WindowXPosition,
                Game.Configuration.WindowYPosition,
                Game.Configuration.WindowXSize,
                Game.Configuration.WindowYSize,
                Game.Configuration.WindowFlags
            );

            if (_window == nint.Zero)
            {
                Log.Logger.SDLError();
                throw new InvalidOperationException(GraphicsExceptionMessage.WindowInitError);
            }

            _renderer = SDL_CreateRenderer(
                _window,
                Game.Configuration.RendererIndex,
                Game.Configuration.RendererFlags
            );

            if (_window == nint.Zero)
            {
                Log.Logger.SDLError();
                throw new InvalidOperationException(GraphicsExceptionMessage.RendererInitError);
            }
        }

        public void Update()
        {
            // Black screen
            SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
            SDL_RenderClear(_renderer);

            // Test Map
            Game.TestMap.Draw(_renderer);

            // Line Marker
            SDL_SetRenderDrawColor(_renderer, 0, 0, 255, 255);            
            SDL_RenderDrawLine(_renderer, (Game.Configuration.WindowXSize / 2) - 1, 0, (Game.Configuration.WindowXSize / 2) - 1, Game.Configuration.WindowYSize - 1);
            SDL_RenderDrawLine(_renderer, Game.Configuration.WindowXSize / 2, 0, Game.Configuration.WindowXSize / 2, Game.Configuration.WindowYSize - 1);
            SDL_RenderDrawLine(_renderer, 0, (Game.Configuration.WindowYSize / 2) - 1, Game.Configuration.WindowXSize - 1, (Game.Configuration.WindowYSize / 2) - 1);
            SDL_RenderDrawLine(_renderer, 0, Game.Configuration.WindowYSize / 2, Game.Configuration.WindowXSize - 1, Game.Configuration.WindowYSize / 2);

            // Present
            SDL_RenderPresent(_renderer);
        }

        public void CleanUp()
        {
            SDL_DestroyRenderer(_renderer);
            SDL_DestroyWindow(_window);
        }
    }
}
