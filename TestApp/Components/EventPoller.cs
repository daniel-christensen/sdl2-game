using TestApp.Interfaces;
using static SDL2.SDL;

namespace TestApp.Components
{
    internal class EventPoller : IComponent, IRegisterable  
    {
        public Game Game { get; }

        private SDL_Event _event;

        public EventPoller(Game game)
        {
            Game = game;
        }

        public void Initialise()
        {
            // TODO
        }

        public void Update()
        {
            while (SDL_PollEvent(out _event) == 1)
            {
                if (_event.type == SDL_EventType.SDL_QUIT)
                {
                    Game.IsRunning = false;
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

        public void CleanUp()
        {
            // TODO
        }
    }
}
