using LayeredSDL2Demo.Helpers;
using LayeredSDL2Demo.Interfaces;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;

namespace LayeredSDL2Demo.Entities
{
    internal abstract class Pokemon : IEntity
    {
        internal Player Player { get; }

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
            Player player,
            int positionX,
            int positionY,
            int width,
            int height,
            string textureALocation,
            string textureBLocation,
            string cryLocation)
        {
            Player = player;
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
            _surfaceA = IMG_Load(TextureALocation);
            if (_surfaceA == IntPtr.Zero) throw new Exception(SDL_GetError());
            TextureA = SDL_CreateTextureFromSurface(renderer, _surfaceA);
            
            // Load texture B into memory
            _surfaceB = IMG_Load(TextureBLocation);
            if (_surfaceB == IntPtr.Zero) throw new Exception(SDL_GetError());
            TextureB = SDL_CreateTextureFromSurface(renderer, _surfaceB);

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

        private int _clickOffsetX = 0;

        private int _clickOffsetY = 0;

        private SDL_Rect _rectangle;

        private IntPtr _surfaceA;

        private IntPtr _surfaceB;

        private int _audioChannel = 0;

        private bool _audioPlaying = false;

        public void PollEvent(SDL_Event sdlEvent)
        {
            switch (sdlEvent.type)
            {
                case SDL_EventType.SDL_MOUSEMOTION:
                {
                    if (_selected)
                    {
                        _rectangle.x = Player.Mouse.x - _clickOffsetX;
                        _rectangle.y = Player.Mouse.y - _clickOffsetY;
                    }

                    break;
                }

                case SDL_EventType.SDL_MOUSEBUTTONUP:
                {
                    if (sdlEvent.button.button == SDL_BUTTON_LEFT)
                    {
                        _selected = false;
                        Player.HandIsFree = true;
                    }
                    break;
                }

                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                {
                    if (sdlEvent.button.button == SDL_BUTTON_LEFT)
                    {
                        bool cursorInSurfaceRect = SDL_PointInRect(ref Player.Mouse, ref _rectangle) == SDL_bool.SDL_TRUE;
                        bool playerHandIsFree = Player.HandIsFree;
                        bool pixelIsNotTransparent = GraphicsHelper.CanClickOnPixel(_surfaceA, _rectangle, Player.Mouse);

                        if (cursorInSurfaceRect && playerHandIsFree && pixelIsNotTransparent)
                        {
                            _selected = true;
                            Player.HandIsFree = false;
                            _clickOffsetX = Player.Mouse.x - PositionX;
                            _clickOffsetY = Player.Mouse.y - PositionY;
                            if (!_audioPlaying)
                            {
                                _audioChannel = Mix_PlayChannel(-1, Cry, 0);
                                _audioPlaying = true;
                            }
                        }
                    }
                    break;
                }
            }
        }

        public void Logic()
        {
            if (Mix_Playing(_audioChannel) == 0 && _audioPlaying)
            {
                _audioPlaying = false;
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
            SDL_FreeSurface(_surfaceA);
            SDL_FreeSurface(_surfaceB);
            SDL_DestroyTexture(TextureA);
            SDL_DestroyTexture(TextureB);
            Mix_FreeChunk(Cry);
        }
    }
}
