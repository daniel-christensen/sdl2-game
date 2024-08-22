using LayeredSDL2Demo.Interfaces;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;

namespace LayeredSDL2Demo.Entities
{
    internal abstract class Pokemon : IEntity
    {
        public string TextureALocation { get; }

        public string TextureBLocation { get; }

        public string CryLocation { get; }

        public IntPtr TextureA { get; private set; }

        public IntPtr TextureB { get; private set; }

        public IntPtr Cry { get; private set; }

        public int PositionX => _rectangle.x;

        public int PositionY => _rectangle.y;

        public int Width => _rectangle.w;

        public int Height => _rectangle.h;

        internal Pokemon(
            int positionX,
            int positionY,
            int width,
            int height,
            string textureALocation,
            string textureBLocation,
            string cryLocation)
        {
            TextureALocation = textureALocation;
            TextureBLocation = textureBLocation;
            CryLocation = cryLocation;

            // Create Rectangle
            _rectangle = new SDL_Rect()
            {
                x = positionX,
                y = positionY,
                w = width,
                h = height
            };
        }

        public IEntity LoadTextures(IntPtr renderer)
        {
            // Load texture A into memory
            IntPtr surface = IMG_Load(TextureALocation);
            if (surface == IntPtr.Zero) throw new Exception(SDL_GetError());
            TextureA = SDL_CreateTextureFromSurface(renderer, surface);

            // Load texture B into memory
            surface = IMG_Load(TextureBLocation);
            if (surface == IntPtr.Zero) throw new Exception(SDL_GetError());
            TextureB = SDL_CreateTextureFromSurface(renderer, surface);

            // Finished with surface
            SDL_FreeSurface(surface);

            return this;
        }

        public IEntity LoadSounds()
        {
            // Load cry into memory
            Cry = Mix_LoadWAV(CryLocation);
            if (Cry == IntPtr.Zero) throw new Exception(SDL_GetError());

            return this;
        }

        private bool _selected = false;

        private SDL_Point _mouse = new SDL_Point();

        private int _clickOffsetX = 0;

        private int _clickOffsetY = 0;

        private SDL_Rect _rectangle;

        public void PollEvent(SDL_Event sdlEvent)
        {
            switch (sdlEvent.type)
            {
                case SDL_EventType.SDL_MOUSEMOTION:
                {
                    _mouse.x = sdlEvent.motion.x;
                    _mouse.y = sdlEvent.motion.y;

                    if (_selected)
                    {
                        _rectangle.x = _mouse.x - _clickOffsetX;
                        _rectangle.y = _mouse.y - _clickOffsetY;
                    }

                    break;
                }

                case SDL_EventType.SDL_MOUSEBUTTONUP:
                {
                    _selected = !(sdlEvent.button.button == SDL_BUTTON_LEFT);
                    break;
                }

                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                {
                    if (sdlEvent.button.button == SDL_BUTTON_LEFT)
                    {
                        if (SDL_PointInRect(ref _mouse, ref _rectangle) == SDL_bool.SDL_TRUE)
                        {
                            _selected = true;
                            _clickOffsetX = _mouse.x - PositionX;
                            _clickOffsetY = _mouse.y - PositionY;
                            if (Mix_Playing(1) == 0)
                                Mix_PlayChannel(1, Cry, 0);
                        }
                    }
                    break;
                }
            }
        }

        public void Draw(nint renderer)
        {
            if (!_selected)
            {
                SDL_RenderCopy(renderer, TextureA, IntPtr.Zero, ref _rectangle);
            }
            else
            {
                SDL_RenderCopy(renderer, TextureB, IntPtr.Zero, ref _rectangle);
            }
        }

        public void CleanUp()
        {
            SDL_DestroyTexture(TextureA);
            SDL_DestroyTexture(TextureB);
            Mix_FreeChunk(Cry);
        }
    }
}
