using LayeredSDL2Demo.Interfaces;

namespace LayeredSDL2Demo
{
    internal class EntityManager : List<IEntity>
    {
        internal void PollEvents(SDL2.SDL.SDL_Event sdlEvent)
        {
            // Entities polled should be in opposite order to entites drawn
            // This is because the final entity drawn will be "on top", so its events should be polled first
            for (int i = Count - 1; i >= 0; i--)
            {
                this[i].PollEvent(sdlEvent);
            }
        }

        internal void Logic(IntPtr window)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                this[i].Logic(window);
            }
        }

        internal void Draw(IntPtr renderer)
        {
            foreach (IEntity entity in this)
            {
                entity.Draw(renderer);
            }
        }

        internal void CleanUp()
        {
            foreach (IEntity entity in this)
            {
                entity.CleanUp();
            }
        }
    }
}
