using LayeredSDL2Demo.Helpers;
using LayeredSDL2Demo.Interfaces;
using System.Runtime.InteropServices;
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

        public int PositionX
        {
            get => _rectangle.x;
            set => _rectangle.x = value;
        }

        public int PositionY
        {
            get => _rectangle.y;
            set => _rectangle.y = value;
        }

        public int Width => _rectangle.w;

        public int Height => _rectangle.h;

        internal Pokemon(
            Player player,
            IntPtr renderer,
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

            LoadTextures(renderer);
            LoadSounds();
            CalculateOriginFloorPoint();
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

        // Point which represents the bottom of the VISIBLE sprite.
        // From bottom right to upper left. So Y takes priority with the row containing the first displayable pixels
        // form the bottom. And only then the first X displayable pixel from the right
        //
        // Note the origin floor point is reference to a relative position.
        // Meaning this point represents 0x0 to the width and height of the displayable rectangle, not the screen
        private SDL_Point _originScaledFloorPoint;

        private void CalculateOriginFloorPoint()
        {
            SDL_Point _originFloorPoint = GraphicsHelper.LastNonTransparentCoordinatesOriginal(_surfaceA);
            _originScaledFloorPoint = GraphicsHelper.ScaleUpPointToDisplaySize(_originFloorPoint, _rectangle.w, _rectangle.h);
        }

        public void Logic(IntPtr window)
        {
            if (Mix_Playing(_audioChannel) == 0 && _audioPlaying)
            {
                _audioPlaying = false;
            }

            SDL_Point relativeFloorPoint = GraphicsHelper.GetRelativePoint(_originScaledFloorPoint, _rectangle);

            int windowWidth, windowHeight;
            SDL_GetWindowSize(window, out windowWidth, out windowHeight);
            int hopSize = 8;
            bool isWithinHopSize = (windowHeight - 1) - relativeFloorPoint.y >= hopSize;
            // Only hop if there is another space for a full hop
            if (relativeFloorPoint.y < windowHeight - 1 && isWithinHopSize && !_selected)
                PositionY = PositionY + hopSize;

            // If there isn't enough space for a full hop (because we're near the floor) - calculate the
            // remaining hop size. This prevents the sprite hoping offscreen then sliding back into frame
            if (relativeFloorPoint.y < windowHeight - 1 && !isWithinHopSize && !_selected)
                PositionY = PositionY + ((windowHeight - 1) - relativeFloorPoint.y);

            // Slide back into frame if dragged off screen
            if (relativeFloorPoint.y > windowHeight - 1 && !_selected)
                PositionY--;
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

            // Debug Lines            
            SDL_SetRenderDrawColor(renderer, 255, 1, 255, 255);
            SDL_RenderDrawLine(renderer, PositionX, PositionY, PositionX + (Width - 1), PositionY);
            SDL_RenderDrawLine(renderer, PositionX + (Width - 1), PositionY, PositionX + (Width - 1), PositionY + (Height - 1));
            SDL_RenderDrawLine(renderer, PositionX + (Width - 1), PositionY + (Height - 1), PositionX, PositionY + (Height - 1));
            SDL_RenderDrawLine(renderer, PositionX, PositionY + (Height - 1), PositionX, PositionY);
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
