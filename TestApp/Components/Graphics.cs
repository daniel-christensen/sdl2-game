using Serilog;
using TestApp.Messages;
using TestApp.Extensions;
using static SDL2.SDL;
using TestApp.Interfaces;

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
            SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
            SDL_RenderClear(_renderer);

            // Drawing a blue rectangle
            SDL_Rect rect = new SDL_Rect()
            {
                x = RectToDraw.x,
                y = RectToDraw.y,
                w = Game.Configuration.GridCellXLength,
                h = Game.Configuration.GridCellYLength
            };
            SDL_SetRenderDrawColor(_renderer, 0, 0, 255, 255);
            SDL_RenderFillRect(_renderer, ref rect);

            SDL_RenderPresent(_renderer);
        }

        public void CleanUp()
        {
            SDL_DestroyRenderer(_renderer);
            SDL_DestroyWindow(_window);
        }
    }
}
