using static SDL2.SDL;
using Vanara.PInvoke;
using System.Runtime.InteropServices;

namespace LayeredSDL2Demo.Helpers
{
    /// <summary>
    /// Helper class which allows SDL2 applications to interact with WIN32 API.
    /// Focuses more on interactions with "Layered" extended window style.
    /// </summary>
    internal static class LayeredWindowHelper
    {
        /// <summary>
        /// Using the WIN32 API, this method finds the rectangle which encompases the work area.
        /// The work area is define as the part of the screen that doesn't include the task bar.
        /// </summary>
        /// <returns>SDL2 rectangle object defining the work area.</returns>
        internal static SDL_Rect GetWorkArea()
        {
            // Create a new instance of the desired struct and Marshal a pointer for it
            var workArea = new RECT();
            IntPtr workAreaPtr = Marshal.AllocHGlobal(Marshal.SizeOf(workArea));
            Marshal.StructureToPtr(workArea, workAreaPtr, true);

            // Call WIN32 API to acquire desired information
            User32.SystemParametersInfo(User32.SPI.SPI_GETWORKAREA, 0, workAreaPtr, User32.SPIF.None);

            // Marshal the pointer back into a useable structure and free the allocated pointer memory
            workArea = Marshal.PtrToStructure<RECT>(workAreaPtr);
            Marshal.FreeHGlobal(workAreaPtr);

            // Convert struct information into a clean SDL structure
            SDL_Rect rect = new SDL_Rect
            {
                x = workArea.Left,
                y = workArea.Top,
                w = workArea.Width,
                h = workArea.Height
            };

            return rect;
        }

        internal static void SetWindowExStyleLayered(IntPtr window)
        {
            // Retrieve system information on window, including native WIN32 window handler
            SDL_SysWMinfo wmInfo = new SDL_SysWMinfo();
            SDL_VERSION(out wmInfo.version);
            SDL_GetWindowWMInfo(window, ref wmInfo);
            IntPtr hWnd = wmInfo.info.win.window;

            // Acquire the system long which describes characteristics of our window
            IntPtr dwNewLong = User32.GetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE);

            // Bit shift the long to enabled our Extended Layered Style
            // From what I understand, this is the only style that lets us create transparent backgrounds
            dwNewLong = dwNewLong | (IntPtr)User32.WindowStylesEx.WS_EX_LAYERED;

            // Now set that long back onto the same window handler with reference to our Extended Style
            User32.SetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE, dwNewLong);

            // Now that our window is set to the Extended Style of "Layered" we can now use Layered-specific methods
            // What we are saying here is that any pixel drawn to the window matching the given COLORREF will turn transparent
            COLORREF transparentColorKey = new COLORREF(255, 0, 255);
            User32.SetLayeredWindowAttributes(hWnd, transparentColorKey, 0, User32.LayeredWindowAttributes.LWA_COLORKEY);
        }
    }
}
