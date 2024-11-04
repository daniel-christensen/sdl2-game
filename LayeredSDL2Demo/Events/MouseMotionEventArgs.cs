namespace LayeredSDL2Demo.Events
{
    internal class MouseMotionEventArgs : EventArgs
    {
        internal SDL2.SDL.SDL_Event SdlEvent { get; set; }

        internal int TextureX { get; set; }

        internal int TextureY { get; set; }

        internal int ContentX { get; set; }

        internal int ContentY { get; set; }

        internal int TextureClickOffsetX { get; set; }

        internal int TextureClickOffsetY { get; set; }

        internal int ContentClickOffsetX { get; set; }

        internal int ContentClickOffsetY { get; set; }

        internal bool Selected { get; set; }

        internal int MouseX { get; set; }

        internal int MouseY { get; set; }
    }
}
