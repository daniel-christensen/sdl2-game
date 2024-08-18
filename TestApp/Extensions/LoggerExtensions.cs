using Serilog;
using TestApp.Messages;

namespace TestApp.Extensions
{
    internal static class LoggerExtensions
    {
        public static void SDLError(this ILogger logger)
        {
            string message = $"{GraphicsLoggingMessage.GraphicsSDLErrorPrefix} {SDL2.SDL.SDL_GetError}";
            logger.Information(message);
        }
    }
}
