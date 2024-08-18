using TestApp.Messages;
using Vanara.PInvoke;

namespace TestApp.Helper
{
    /// <summary>
    /// Provides utilities for managing the console window and allocation.
    /// </summary>
    internal static class ConsoleManager
    {
        /// <summary>
        /// Initialises the console window for the current process if not already allocated.
        /// Throws an exception if attempting to allocate the console multiple times.
        /// </summary>
        public static void Initialise()
        {
            if (!IsConsoleAllocated())
            {
                Kernel32.AllocConsole();
                return;
            }

            throw new InvalidOperationException(ConsoleExceptionMessage.DoubleAllocError);
        }

        /// <summary>
        /// Shows the console window associated with the current process.
        /// </summary>
        public static void ShowConsoleWindow()
        {
            HWND handle = Kernel32.GetConsoleWindow();
            User32.ShowWindow(handle, ShowWindowCommand.SW_SHOW);
        }

        /// <summary>
        /// Hides the console window associated with the current process.
        /// </summary>
        public static void HideConsoleWindow()
        {
            HWND handle = Kernel32.GetConsoleWindow();
            User32.ShowWindow(handle, ShowWindowCommand.SW_HIDE);
        }

        /// <summary>
        /// Checks if a console window is allocated to the current process.
        /// </summary>
        /// <returns>true if a console window is allocated; otherwise, false.</returns>
        public static bool IsConsoleAllocated()
        {
            HWND handle = Kernel32.GetConsoleWindow();
            return handle != HWND.NULL;
        }
    }
}
