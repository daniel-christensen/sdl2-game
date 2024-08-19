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
                        case SDL_Keycode.SDLK_UP: Game.Player.YPosition -= Game.Configuration.GridCellYLength; break;
                        case SDL_Keycode.SDLK_DOWN: Game.Player.YPosition += Game.Configuration.GridCellYLength; break;
                        case SDL_Keycode.SDLK_LEFT: Game.Player.XPosition -= Game.Configuration.GridCellXLength; break;
                        case SDL_Keycode.SDLK_RIGHT: Game.Player.XPosition += Game.Configuration.GridCellXLength; break;
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
