using static SDL2.SDL;
using static Vanara.PInvoke.User32;

namespace LayeredSDL2Demo
{
    /// <summary>
    /// Really just represents data relating to the mouse cursor in relation to the video-game logic.
    /// </summary>
    internal class Player
    {
        internal bool HandIsFree = true;

        internal SDL_Point Mouse;

        internal Player()
        {
            Mouse = new SDL_Point();
        }

        internal void PollEvent(SDL_Event sdlEvent)
        {
            if (sdlEvent.type == SDL_EventType.SDL_MOUSEMOTION)
            {
                Mouse.x = sdlEvent.motion.x;
                Mouse.y = sdlEvent.motion.y;
            }
        }
    }
}
