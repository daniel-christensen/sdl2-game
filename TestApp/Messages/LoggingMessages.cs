namespace TestApp.Messages
{
    internal static class GameLoggingMessage
    {
        internal const string GameInstanceWithoutConsole = "A game instance has been launched with console enabled. All logging will continue to be logged into the configured log file(s).";
    }

    internal static class GraphicsLoggingMessage
    {
        internal const string GraphicsSDLErrorPrefix = "SDL ERROR:";
    }
}
