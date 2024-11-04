using LayeredSDL2Demo.Entities;

namespace LayeredSDL2Demo.Events
{
    internal static class MouseMotionEvents
    {
        internal static void HandleDragAndDrop(object sender, MouseMotionEventArgs e)
        {
            var pokemon = (Pokemon)sender;
            //pokemon.

            if (e.SdlEvent.type != SDL2.SDL.SDL_EventType.SDL_MOUSEMOTION)
                return;

            if (e.Selected)
            {
                e.TextureX = e.MouseX - e.TextureClickOffsetX;
                e.TextureY = e.MouseY - e.TextureClickOffsetY;
                e.ContentX = e.MouseX - e.ContentClickOffsetX;
                e.ContentY = e.MouseY - e.ContentClickOffsetY;
            }
        }
    }
}
