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

        public string TextureAFilePath { get; }

        public string TextureBFilePath { get; }

        public IntPtr TextureA { get; private set; }

        public IntPtr TextureB { get; private set; }

        public string CryFilePath { get; }

        public IntPtr Cry { get; private set; }

        private SDL_Rect _textureRect;
        public int TextureX
        {
            get => _textureRect.x;
            set => _textureRect.x = value;
        }
        public int TextureY
        {
            get => _textureRect.y;
            set => _textureRect.y = value;
        }
        public int TextureWidth => _textureRect.w;
        public int TextureHeight => _textureRect.h;

        private SDL_Rect _contentRect;
        public int ContentX
        {
            get => _contentRect.x;
            set => _contentRect.x = value;
        }
        public int ContentY
        {
            get => _contentRect.y;
            set => _contentRect.y = value;
        }
        public int ContentWidth => _contentRect.w;
        public int ContentHeight => _contentRect.h;

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
            TextureAFilePath = textureALocation;
            TextureBFilePath = textureBLocation;
            CryFilePath = cryLocation;

            CreateTextureRect(positionX, positionY, width, height);
        }

        private IEntity CreateTextureRect(int x, int y, int width, int height)
        {
            _textureRect = new SDL_Rect()
            {
                x = x,
                y = y,
                w = width,
                h = height
            };

            return this;
        }

        public IEntity CreateContentRect()
        {
            TextureBoundsAnalyser boundsAnalyser = new TextureBoundsAnalyser(
                _textureRect.x,
                _textureRect.y,
                _textureRect.w,
                _textureRect.h,
                Marshal.PtrToStructure<SDL_Surface>(_surfaceA).w,
                Marshal.PtrToStructure<SDL_Surface>(_surfaceA).h);

            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(_surfaceA);
            SDL_PixelFormat pixelFormat = Marshal.PtrToStructure<SDL_PixelFormat>(surface.format);

            _contentRect = boundsAnalyser.GetContentRect(surface, pixelFormat);
            return this;
        }

        public IEntity LoadTextures(IntPtr renderer)
        {
            // Load texture A into memory
            _surfaceA = IMG_Load(TextureAFilePath);
            if (_surfaceA == IntPtr.Zero) throw new Exception(SDL_GetError());
            TextureA = SDL_CreateTextureFromSurface(renderer, _surfaceA);

            // Load texture B into memory
            _surfaceB = IMG_Load(TextureBFilePath);
            if (_surfaceB == IntPtr.Zero) throw new Exception(SDL_GetError());
            TextureB = SDL_CreateTextureFromSurface(renderer, _surfaceB);

            return this;
        }

        public IEntity LoadSounds()
        {
            // Load cry into memory
            Cry = Mix_LoadWAV(CryFilePath);
            if (Cry == IntPtr.Zero) throw new Exception(SDL_GetError());

            return this;
        }

        private bool _selected = false;

        private int _textureClickOffsetX = 0;

        private int _textureClickOffsetY = 0;

        private int _contentClickOffsetX = 0;

        private int _contentClickOffsetY = 0;

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
                        TextureX = Player.Mouse.x - _textureClickOffsetX;
                        TextureY = Player.Mouse.y - _textureClickOffsetY;
                        ContentX = Player.Mouse.x - _contentClickOffsetX;
                        ContentY = Player.Mouse.y - _contentClickOffsetY;
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
                        bool cursorInSurfaceRect = SDL_PointInRect(ref Player.Mouse, ref _textureRect) == SDL_bool.SDL_TRUE;
                        bool playerHandIsFree = Player.HandIsFree;

                        TextureBoundsAnalyser boundsAnalyser = new TextureBoundsAnalyser(
                            _textureRect.x,
                            _textureRect.y,
                            _textureRect.w,
                            _textureRect.h,
                            Marshal.PtrToStructure<SDL_Surface>(_surfaceA).w,
                            Marshal.PtrToStructure<SDL_Surface>(_surfaceA).h);

                        bool pixelIsNotTransparent = boundsAnalyser.IsPixelClickable(
                             Marshal.PtrToStructure<SDL_Surface>(_surfaceA),
                             Player.Mouse);

                        if (cursorInSurfaceRect && playerHandIsFree && pixelIsNotTransparent)
                        {
                            _selected = true;
                            Player.HandIsFree = false;
                            _textureClickOffsetX = Player.Mouse.x - TextureX;
                            _textureClickOffsetY = Player.Mouse.y - TextureY;
                            _contentClickOffsetX = Player.Mouse.x - ContentX;
                            _contentClickOffsetY = Player.Mouse.y - ContentY;
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

        //private void CalculateOriginFloorPoint()
        //{
        //    SDL_Point _originFloorPoint = GraphicsHelper.LastNonTransparentCoordinatesOriginal(_surfaceA);
        //    _originScaledFloorPoint = GraphicsHelper.ScaleUpPointToDisplaySize(_originFloorPoint, _rectangle.w, _rectangle.h);
        //}

        /*internal const double Gravity = 500;
        internal const double Elasticity = 0.75;
        internal const double EnhancedGravity = 1000;
        internal const double DeltaTime = 1.0 / 60;

        private double _velocity = 0;
        private double _upVelocity = 0;
        private bool _belowGround = false;*/

        public void UpdateLogic()
        {
            if (Mix_Playing(_audioChannel) == 0 && _audioPlaying)
            {
                _audioPlaying = false;
            }

            /*SDL_Point relativeFloorPoint = GraphicsHelper.GetRelativePoint(_originScaledFloorPoint, _rectangle);

            int windowWidth, windowHeight;
            SDL_GetWindowSize(window, out windowWidth, out windowHeight);

            if (relativeFloorPoint.y < windowHeight - 200 && !_selected)
            {
                _upVelocity = 0;
                _velocity += Gravity * DeltaTime;
                PositionY += (int)Math.Round(_velocity * DeltaTime);
            }

            if (relativeFloorPoint.y == windowHeight - 200 && !_selected)
            {
                PositionY = PositionY + ((windowHeight - 200) - relativeFloorPoint.y);
            }

            if (relativeFloorPoint.y > windowHeight - 200 && !_selected)
            {
                _velocity = 0;
                _upVelocity += Gravity * DeltaTime;
                PositionY -= (int)Math.Round(_upVelocity * DeltaTime);
            }

            /*if (relativeFloorPoint.y < windowHeight - 1 && !_selected)
            {
                _acceleration = _force / Mass;
                _velocity += _acceleration * DeltaTime;
                _velocity *= Damping;
            }

            int hopSize = (int)Math.Round(_velocity * DeltaTime);
            bool isWithinHopSize = (windowHeight - 1) - relativeFloorPoint.y >= hopSize;
            // Only hop if there is another space for a full hop
            if (relativeFloorPoint.y < windowHeight - 1 && isWithinHopSize && !_selected)
                PositionY = PositionY + hopSize;

            // If there isn't enough space for a full hop (because we're near the floor) - calculate the
            // remaining hop size. This prevents the sprite hoping offscreen then sliding back into frame
            //if (relativeFloorPoint.y < windowHeight - 1 && !isWithinHopSize && !_selected)
            //    PositionY = PositionY + ((windowHeight - 1) - relativeFloorPoint.y);

            // Bounce back into frame if under floor
            if (relativeFloorPoint.y >= windowHeight - 1)
            {
                _velocity = -_velocity * Elasticity;
                _velocity *= Damping;

                if (Math.Abs(_velocity) < 1)
                {
                    _velocity = 0;  // Stop the movement if the bounce is too weak
                    PositionY = PositionY + ((windowHeight - 1) - relativeFloorPoint.y);  // Ensure the sprite stays at ground level
                }
            }

            // Slide back into frame if dragged off screen
            //if (relativeFloorPoint.y > windowHeight - 1 && !_selected)
            //    PositionY--;

            // Set velocity to 0 if on floor
            //if (relativeFloorPoint.y == windowHeight - 1 && !_selected)
            //    _velocity = 0;*/
        }

        public void Draw(nint renderer)
        {
            // Debug Lines            
            SDL_SetRenderDrawColor(renderer, 255, 1, 255, 255);
            DebugDrawRect(renderer, _textureRect, new SDL_Color() { r = 255, g = 1, b = 255, a = 255 });
            DebugDrawRect(renderer, _contentRect, new SDL_Color() { r = 100, g = 50, b = 200, a = 255 });

            if (!_selected)
                SDL_RenderCopy(renderer, TextureA, IntPtr.Zero, ref _textureRect);
            else
                SDL_RenderCopy(renderer, TextureB, IntPtr.Zero, ref _textureRect);
        }

        private void DebugDrawRect(IntPtr renderer, SDL_Rect rect, SDL_Color drawColor)
        {
            SDL_SetRenderDrawColor(renderer, drawColor.r, drawColor.g, drawColor.b, drawColor.a);
            SDL_RenderDrawLine(renderer, rect.x, rect.y, rect.x + (rect.w - 1), rect.y);
            SDL_RenderDrawLine(renderer, rect.x + (rect.w - 1), rect.y, rect.x + (rect.w - 1), rect.y + (rect.h - 1));
            SDL_RenderDrawLine(renderer, rect.x + (rect.w - 1), rect.y + (rect.h - 1), rect.x, rect.y + (rect.h - 1));
            SDL_RenderDrawLine(renderer, rect.x, rect.y + (rect.h - 1), rect.x, rect.y);
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
