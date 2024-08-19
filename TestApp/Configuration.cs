using static SDL2.SDL;

namespace TestApp
{
    internal class Configuration
    {
        // GAME
        internal int GridCellXLength { get; private set; }
        internal int GridCellYLength { get; private set; }

        // GRAPHICS
        internal string WindowTitle { get; }
        internal int WindowXPosition { get; }
        internal int WindowYPosition { get; }
        internal int WindowXSize { get; }
        internal int WindowYSize { get; }
        internal SDL_WindowFlags WindowFlags { get; }
        internal int RendererIndex { get; }
        internal SDL_RendererFlags RendererFlags { get; }

        internal Configuration()
        {
            GridCellXLength = DefaultValue.DefaultGridCellXLength;
            GridCellYLength = DefaultValue.DefaultGridCellYLength;
            WindowTitle = DefaultValue.DefaultWindowTitle;
            WindowXPosition = DefaultValue.DefaultWindowXPosition;
            WindowYPosition = DefaultValue.DefaultWindowYPosition;
            WindowXSize = DefaultValue.DefaultWindowXSize;
            WindowYSize = DefaultValue.DefaultWindowYSize;
            WindowFlags = DefaultValue.DefaultWindowFlags;
            RendererIndex = DefaultValue.DefaultRendererIndex;
            RendererFlags = DefaultValue.DefaultRendererFlags;
        }

        private class DefaultValue
        {
            internal const int DefaultGridCellXLength = 32;
            internal const int DefaultGridCellYLength = 32;
            internal const string DefaultWindowTitle = "My Test SDL2 Application!";
            internal const int DefaultWindowXPosition = SDL_WINDOWPOS_UNDEFINED;
            internal const int DefaultWindowYPosition = SDL_WINDOWPOS_UNDEFINED;
            internal const int DefaultWindowXSize = 640;
            internal const int DefaultWindowYSize = 480;
            internal const SDL_WindowFlags DefaultWindowFlags = 0;
            internal const int DefaultRendererIndex = -1;
            internal const SDL_RendererFlags DefaultRendererFlags = SDL_RendererFlags.SDL_RENDERER_ACCELERATED;
        }
    }
}
