namespace TestApp.Messages
{
    internal static class GameExceptionMessage
    {
        internal const string FatalUncaught = "An unknown error has occurred which has caused the program to quit.";
    }

    internal static class ConsoleExceptionMessage
    {
        internal const string DoubleAllocError = "Program tried to allocate a console when one already exists.";
    }

    internal static class GraphicsExceptionMessage
    {
        internal const string SDL2InitError = "A problem has occurred when attempting to initialise SDL2.";

        internal const string WindowInitError = "A problem has occurred when attempting to initialise a SDL2 window.";

        internal const string RendererInitError = "A problem has occurred when attemping to initialise a SDL2 renderer.";
    }
}
