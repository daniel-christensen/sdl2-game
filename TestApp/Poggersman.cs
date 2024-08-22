using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;

namespace TestApp
{
    internal static class Poggersman
    {
        internal static void aaaaaMain(string[] args)
        {
            const string className = "mylayeredwindowclass";
            HINSTANCE hInstance = Kernel32.GetModuleHandle(null);

            var wc = new User32.WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf(typeof(User32.WNDCLASSEX)),
                style = User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
                lpfnWndProc = WndProc,
                hInstance = hInstance,
                hCursor = User32.LoadCursor(IntPtr.Zero, Macros.MAKEINTRESOURCE(32512)),
                hbrBackground = User32.GetSysColorBrush(SystemColorIndex.COLOR_WINDOW + 1),
                lpszClassName = className
            };

            if (User32.RegisterClassEx(in wc) == Kernel32.ATOM.INVALID_ATOM)
            {
                Console.WriteLine("Failed to register window class.");
                return;
            }

            User32.SafeHWND hWnd = User32.CreateWindowEx(
                User32.WindowStylesEx.WS_EX_LAYERED | User32.WindowStylesEx.WS_EX_TRANSPARENT, // Extended styles
                className,
                "Layered Window Example",
                User32.WindowStyles.WS_OVERLAPPEDWINDOW, // Regular styles
                300, 300, 500, 500,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero
            );

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("Failed to create window.");
                return;
            }

            User32.SetLayeredWindowAttributes(hWnd, 0xFF00FF, // Magenta as the transparent color
                                  0, User32.LayeredWindowAttributes.LWA_COLORKEY);

            User32.ShowWindow(hWnd, ShowWindowCommand.SW_SHOW);
            User32.UpdateWindow(hWnd);

            bool running = true;
            while (running)
            {
                if (User32.GetMessage(out MSG msg, IntPtr.Zero, 0, 0) == 1)
                {
                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                }
                else
                {
                    running = false;
                }
            }
        }

        public static IntPtr WndProc(HWND hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case (uint)User32.WindowMessage.WM_DESTROY:
                    User32.PostQuitMessage(0); // Signals to end the application
                    return IntPtr.Zero;

                default:
                    return User32.DefWindowProc(hWnd, msg, wParam, lParam); // Default handling for all other messages
            }
        }
    }
}
