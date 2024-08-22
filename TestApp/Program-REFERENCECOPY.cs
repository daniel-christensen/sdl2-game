using System.Runtime.InteropServices;
using Vanara.PInvoke;
using static SDL2.SDL;

namespace TestApp
{
    public class ProgramREFERENCECOPY
    {
        public static IntPtr window;

        public static IntPtr renderer;

        public static int x = 0;

        public static int y = 0;

        public static int clickOffsetX = 0;

        public static int clickOffsetY = 0;

        public static void Create()
        {
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
        }

        public static void Main(string[] args)
        {
            Create();

            // mouse movements
            bool leftMouseButtonDown = false;
            SDL_Point mouse = new SDL_Point();

            SDL_Rect rect1 = new SDL_Rect()
            {
                x = x,
                y = y,
                w = 100,
                h = 100
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

                if (selected)
                    SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);
                else
                    SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
                SDL_RenderFillRect(renderer, ref rect1);
                SDL_RenderPresent(renderer);
            }

            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
        }
    }
}