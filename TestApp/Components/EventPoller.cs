using static SDL2.SDL;

namespace TestApp.Components
{
    internal class EventPoller
    {
        internal Configuration Configuration { get; }

        internal GameSystem GameSystem { get; }

        private SDL_Event _event;

        internal EventPoller(Configuration configuration, GameSystem gameSystem)
        {
            Configuration = configuration;
            GameSystem = gameSystem;
        }

        internal void Initialise()
        {
            // TODO
        }

        internal void PollEvents()
        {
            while (SDL_PollEvent(out _event) == 1)
            {
                if (_event.type == SDL_EventType.SDL_QUIT)
                {
                    GameSystem.Quit();
                }

                if (_event.type == SDL_EventType.SDL_KEYDOWN)
                {
                    switch (_event.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_UP: RectToDraw.y -= 10; break;
                        case SDL_Keycode.SDLK_DOWN: RectToDraw.y += 10; break;
                        case SDL_Keycode.SDLK_LEFT: RectToDraw.x -= 10; break;
                        case SDL_Keycode.SDLK_RIGHT: RectToDraw.x += 10; break;
                    }
                }
            }
        }
    }
}
