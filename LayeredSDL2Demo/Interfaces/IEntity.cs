namespace LayeredSDL2Demo.Interfaces
{
    internal interface IEntity
    {
        internal IntPtr TextureA { get; }

        internal IntPtr TextureB { get; }

        internal IntPtr Cry { get; }

        internal int PositionX { get; }

        internal int PositionY { get; }

        internal int Width { get; }

        internal int Height { get; }

        internal IEntity LoadTextures(IntPtr renderer);

        internal IEntity LoadSounds();

        internal void PollEvent(SDL2.SDL.SDL_Event sdlEvent);
        
        internal void Draw(IntPtr renderer);

        internal void Logic();

        internal void CleanUp();
    }
}
