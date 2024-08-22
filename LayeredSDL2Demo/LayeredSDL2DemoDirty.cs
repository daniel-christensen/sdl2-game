using SDL2;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;

namespace LayeredSDL2DemoDirty
{
    // https://www.pokencyclopedia.info/en/index.php?id=sprites/gen4/spr_platinum
    // https://play.pokemonshowdown.com/audio/cries/

    public static class Program
    {
        public static IntPtr window;

        public static IntPtr renderer;

        public static int x = 0;

        public static int y = 0;

        public static int clickOffsetX = 0;

        public static int clickOffsetY = 0;

        public static IntPtr surface;

        public static IntPtr textureA;

        public static IntPtr textureB;

        public static IntPtr soundEffect;

        public static void NotMain(string[] args)
        {
            // SETUP - Ignoring null/errors



            if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) < 0)
            {
                throw new Exception(SDL_GetError());
            }

            if (SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) < 0)
            {
                throw new Exception(SDL_GetError());
            }

            // get work area and use that as window length and width
            var workArea = new RECT();
            IntPtr workAreaPtr = Marshal.AllocHGlobal(Marshal.SizeOf(workArea));
            Marshal.StructureToPtr(workArea, workAreaPtr, true);
            User32.SystemParametersInfo(User32.SPI.SPI_GETWORKAREA, 0, workAreaPtr, User32.SPIF.None);
            workArea = Marshal.PtrToStructure<RECT>(workAreaPtr);
            Marshal.FreeHGlobal(workAreaPtr);

            //int desktopWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN);
            //int desktopHeight = User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN);

            int desktopWidth = workArea.Right - workArea.Left;
            int desktopHeight = workArea.Bottom - workArea.Top;

            window = SDL_CreateWindow(
                "Transparent Window",
                0,//SDL_WINDOWPOS_CENTERED,
                0,//SDL_WINDOWPOS_CENTERED,
                desktopWidth,
                desktopHeight,
                SDL_WindowFlags.SDL_WINDOW_BORDERLESS | SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP
            );

            // Hide window to avoid a "flicker" from when the entire renderer is painted with a transparent colour
            SDL_HideWindow(window);

            renderer = SDL_CreateRenderer(
                window,
                -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED
            );

            // Allows window focus click to also act as a mouse event :)
            SDL_SetHint("SDL_MOUSE_FOCUS_CLICKTHROUGH", "1");

            SDL_SysWMinfo wmInfo = new SDL_SysWMinfo();
            SDL_VERSION(out wmInfo.version);
            SDL_GetWindowWMInfo(window, ref wmInfo);
            IntPtr hWnd = wmInfo.info.win.window;

            IntPtr dwNewLong = User32.GetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE);

            User32.SetWindowLong(
                hWnd,
                User32.WindowLongFlags.GWL_EXSTYLE,
                (dwNewLong | (IntPtr)User32.WindowStylesEx.WS_EX_LAYERED)
            );

            User32.SetLayeredWindowAttributes(
                hWnd,
                new COLORREF(255, 0, 255),
                0,
                User32.LayeredWindowAttributes.LWA_COLORKEY
            );

            // Now that windows has matched our transparent colour key we can show the window
            // This prevent the user ever seeing the window preperation that happens within a split-second
            SDL_ShowWindow(window);


            // img stuff
            IMG_Init(IMG_InitFlags.IMG_INIT_PNG);
            string path = "Resources/charmander_1.png";
            IntPtr surface = IMG_Load(path);
            if (surface == IntPtr.Zero)
            {
                throw new Exception(SDL_GetError());
            }

            textureA = SDL_CreateTextureFromSurface(renderer, surface);

            path = "Resources/charmander_2.png";
            surface = IMG_Load(path);
            if (surface == IntPtr.Zero)
            {
                throw new Exception(SDL_GetError());
            }

            textureB = SDL_CreateTextureFromSurface(renderer, surface);

            SDL_FreeSurface(surface);

            // audio stuff
            soundEffect = Mix_LoadWAV("Resources/charmander.mp3");
            if (soundEffect == IntPtr.Zero)
            {
                throw new Exception(SDL_GetError());
            }

            // ------------------------------------------------------------------------------------

            // mouse movements
            SDL_Point mouse = new SDL_Point();
            

            //(int, int) centeredCoords = TestApp.Helper.GraphicsHelper.CalculateRelativeCentreCoordinates(desktopWidth, desktopHeight, 200, 200);
            x = 0;
            y = 0;
            
            SDL_Rect rect1 = new SDL_Rect()
            {
                x = x,
                y = y,
                w = 200,
                h = 200
            };

            bool selected = false;

            bool quit = false;
            SDL_Event myEvent;
            while (!quit)
            {
                SDL_PollEvent(out myEvent);

                switch (myEvent.type)
                {
                    case SDL_EventType.SDL_QUIT:
                    {
                        quit = true;
                        break;
                    }

                    case SDL_EventType.SDL_MOUSEMOTION:
                    {
                        mouse.x = myEvent.motion.x;
                        mouse.y = myEvent.motion.y;

                        if (selected)
                        {
                            x = mouse.x - clickOffsetX;
                            y = mouse.y - clickOffsetY;
                        }

                        break;
                    }

                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                    {
                        if (myEvent.button.button == SDL_BUTTON_LEFT)
                        {
                            selected = false;
                        }
                        break;
                    }

                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    {
                        if (myEvent.button.button == SDL_BUTTON_LEFT)
                        {
                            if (SDL_PointInRect(ref mouse, ref rect1) == SDL_bool.SDL_TRUE)
                            {
                                selected = true;
                                clickOffsetX = mouse.x - x;
                                clickOffsetY = mouse.y - y;
                                if (Mix_Playing(1) == 0)
                                    Mix_PlayChannel(1, soundEffect, 0);
                            }
                        }
                        break;
                    }

                    case SDL_EventType.SDL_KEYDOWN:
                    {
                        switch (myEvent.key.keysym.sym)
                        {
                            case SDL_Keycode.SDLK_UP: y -= 10; break;
                            case SDL_Keycode.SDLK_DOWN: y += 10; break;
                            case SDL_Keycode.SDLK_LEFT: x -= 10; break;
                            case SDL_Keycode.SDLK_RIGHT: x += 10; break;
                        }
                        break;
                    }

                    /*if (myEvent.type == SDL_EventType.SDL_WINDOWEVENT)
                    {
                        if (myEvent.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED)
                        {
                            //SDL_DestroyRenderer(renderer);
                            //SDL_DestroyWindow(window);
                            //Create();
                        }
                        else if (myEvent.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED)
                        {
                            //SDL_HideWindow(window);
                            //SDL_DestroyWindow(window);
                            //Create();
                        }
                    }*/
                }

                // Clears the screen with the colour which matches the COLORREF of the 
                SDL_SetRenderDrawColor(renderer, 255, 0, 255, 255);
                SDL_RenderClear(renderer);

                rect1.x = x;
                rect1.y = y;

                //if (selected)
                //    SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);
                //else
                //    SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
                //SDL_RenderFillRect(renderer, ref rect1);

                if (selected)
                    SDL_RenderCopy(renderer, textureB, IntPtr.Zero, ref rect1);
                else
                    SDL_RenderCopy(renderer, textureA, IntPtr.Zero, ref rect1);
                SDL_RenderPresent(renderer);
            }

            Mix_FreeMusic(soundEffect);
            SDL_FreeSurface(surface);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);

            Mix_Quit();
            IMG_Quit();
            SDL_Quit();
        }
    }
}