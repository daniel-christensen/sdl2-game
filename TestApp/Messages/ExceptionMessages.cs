namespace TestApp.Messages
{
    internal static class GameExceptionMessage
    {
        internal const string FatalUncaught = "An unknown error has occurred which has caused the program to quit.";
    }

    internal static class RegistryExceptionMessage
    {
        internal const string DoubleRegister = "The program tried to register the same type twice.";
        internal const string RegisterableInterfaceNotImplemented = "The program tried to register a type that does not implement IRegisterable.";
        internal const string SubKeyInterfaceNotImplemented = "The program tried to register a type that does not implement the required subkey interface.";
        internal const string NonExistentRegistry = "The program tried to handle a non-existent registry.";
        internal const string NonExistentRegistration = "The program tried to create an instance of an object that is not registered with the given keys.";
        internal const string NullInstance = "The program tried to create an instance from a registry but it ended up null.";
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

    internal static class CollectionExceptionMessage
    {
        internal const string ComponentDicInvalidKey = "An incorrect key was used when trying to create a new ComponentDictionary.";
    }
}
