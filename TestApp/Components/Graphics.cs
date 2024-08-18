using Serilog;
using TestApp.Messages;
using TestApp.Extensions;
using static SDL2.SDL;

namespace TestApp.Components
{
    internal class Graphics
    {
        internal Configuration Configuration { get; }

        private nint _window;

        private nint _renderer;

        internal Graphics(Configuration configuration)
        {
            Configuration = configuration;
        }

        internal void Initialise()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                Log.Logger.SDLError();
                throw new InvalidOperationException(GraphicsExceptionMessage.SDL2InitError);
            }

            _window = SDL_CreateWindow(
                Configuration.WindowTitle,
                Configuration.WindowXPosition,
                Configuration.WindowYPosition,
                Configuration.WindowXSize,
                Configuration.WindowYSize,
                Configuration.WindowFlags
            );

            if (_window == nint.Zero)
            {
                Log.Logger.SDLError();
                throw new InvalidOperationException(GraphicsExceptionMessage.WindowInitError);
            }

            _renderer = SDL_CreateRenderer(
                _window,
                Configuration.RendererIndex,
                Configuration.RendererFlags
            );

            if (_window == nint.Zero)
            {
                Log.Logger.SDLError();
                throw new InvalidOperationException(GraphicsExceptionMessage.RendererInitError);
            }
        }

        internal void Display()
        {
            SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
            SDL_RenderClear(_renderer);

            // Drawing a blue rectangle
            SDL_Rect rect = new SDL_Rect()
            {
                x = RectToDraw.x,
                y = RectToDraw.y,
                w = Configuration.GridCellXLength,
                h = Configuration.GridCellYLength
            };
            SDL_SetRenderDrawColor(_renderer, 0, 0, 255, 255);
            SDL_RenderFillRect(_renderer, ref rect);

            SDL_RenderPresent(_renderer);
        }

        internal void CleanUp()
        {
            SDL_DestroyRenderer(_renderer);
            SDL_DestroyWindow(_window);
        }
    }
}
