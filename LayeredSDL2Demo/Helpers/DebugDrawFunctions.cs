using static SDL2.SDL;

namespace LayeredSDL2Demo.Helpers
{
    /// <summary>
    /// Set of uncategorised debug and test functions used during development in regards to SDL2.
    /// </summary>
    internal static class DebugDrawFunctions
    {
        internal static void DebugDrawRect(IntPtr renderer, SDL_Rect rect, SDL_Color drawColor)
        {
            SDL_SetRenderDrawColor(renderer, drawColor.r, drawColor.g, drawColor.b, drawColor.a);
            SDL_RenderDrawLine(renderer, rect.x, rect.y, rect.x + (rect.w - 1), rect.y);
            SDL_RenderDrawLine(renderer, rect.x + (rect.w - 1), rect.y, rect.x + (rect.w - 1), rect.y + (rect.h - 1));
            SDL_RenderDrawLine(renderer, rect.x + (rect.w - 1), rect.y + (rect.h - 1), rect.x, rect.y + (rect.h - 1));
            SDL_RenderDrawLine(renderer, rect.x, rect.y + (rect.h - 1), rect.x, rect.y);
        }
    }
}