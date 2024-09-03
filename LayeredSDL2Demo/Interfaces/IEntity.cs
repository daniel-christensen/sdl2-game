namespace LayeredSDL2Demo.Interfaces
{
    internal interface IEntity
    {
        internal IntPtr TextureA { get; }

        internal IntPtr TextureB { get; }

        internal IntPtr Cry { get; }

        internal int TextureX { get; }

        internal int TextureY { get; }

        internal int TextureWidth { get; }

        internal int TextureHeight { get; }

        internal int ContentX { get; }

        internal int ContentY { get; }

        internal int ContentWidth { get; }

        internal int ContentHeight { get; }

        internal IEntity CreateContentRect();

        internal IEntity LoadTextures(IntPtr renderer);

        internal IEntity LoadSounds();

        internal void PollEvent(SDL2.SDL.SDL_Event sdlEvent);
        
        internal void Draw(IntPtr renderer);

        internal void UpdateLogic();

        internal void CleanUp();
    }
}
