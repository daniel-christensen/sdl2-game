using Vanara.PInvoke;
using static SDL2.SDL;

namespace TestApp
{
    public class ProgramREFERENCECOPY
    {
        public static void NotMain(string[] args)
        {
            int desktopWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN);
            int desktopHeight = User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN);

            IntPtr window = SDL_CreateWindow(
                "Transparent Window",
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                desktopWidth,
                desktopHeight,
                SDL_WindowFlags.SDL_WINDOW_BORDERLESS | SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP
            );

            IntPtr renderer = SDL_CreateRenderer(
                window,
                -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED
            );

            SDL_SetRenderDrawColor(renderer, 255, 0, 255, 255);
            SDL_RenderClear(renderer);

            SDL_Rect rect1 = new SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 100,
                h = 100
            };

            SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);
            SDL_RenderFillRect(renderer, ref rect1);

            SDL_Rect rect2 = new SDL_Rect()
            {
                x = desktopWidth / 2,
                y = desktopHeight / 2,
                w = 100,
                h = 100
            };

            SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            SDL_RenderFillRect(renderer, ref rect2);

            SDL_SysWMinfo wmInfo = new SDL_SysWMinfo();
            SDL_VERSION(out wmInfo.version);
            SDL_GetWindowWMInfo(window, ref wmInfo);
            IntPtr hWnd = wmInfo.info.win.window;

            IntPtr dwNewLong = User32.GetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE);

            User32.SetWindowLong(
                hWnd,
                User32.WindowLongFlags.GWL_EXSTYLE,
                dwNewLong | (IntPtr)User32.WindowStylesEx.WS_EX_LAYERED
            );

            User32.SetLayeredWindowAttributes(
                hWnd,
                new COLORREF(255, 0, 255),
                0,
                User32.LayeredWindowAttributes.LWA_COLORKEY
            );

            SDL_RenderPresent(renderer);

            bool quit = false;
            SDL_Event myEvent;
            while (!quit)
            {
                while (SDL_PollEvent(out myEvent) != 0)
                {
                    if (myEvent.type == SDL_EventType.SDL_QUIT)
                    {
                        quit = true;
                    }
                }
            }

            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            Console.ReadKey();
        }
    }
}