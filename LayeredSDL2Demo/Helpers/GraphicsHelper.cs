using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace LayeredSDL2Demo.Helpers
{
    internal static class GraphicsHelper
    {
        // The original width and height of the pokemon png files
        internal const float OriginalWidth = 80;
        internal const float OriginalHeight = 80;

        internal static SDL_Point GetRelativeAndScaledCoordinatesForSurface(
            int scaledSurfaceWidth, 
            int scaledSurfaceHeight,
            int surfaceX,
            int surfaceY,
            int mouseX,
            int mouseY)
        {
            // These are the scales/ratios we'll use to calculate the final coordinates.
            float scaleForX = scaledSurfaceWidth / OriginalWidth;
            float scaleForY = scaledSurfaceHeight / OriginalHeight;

            // Gets the realtive position. This returns the correct coordinates if the surface is of
            // the original height and width of the PNG file. But because we're free to increase the size
            // of the sprites, we need to scale these relative coordinates.
            int relativeX = mouseX - surfaceX;
            int relativeY = mouseY - surfaceY;

            float scaledRelativeX = relativeX / scaleForX;
            float scaledRelativeY = relativeY / scaleForY;

            SDL_Point point = new SDL_Point()
            {
                x = (int)Math.Round(scaledRelativeX),
                y = (int)Math.Round(scaledRelativeY)
            };

            return point;
        }

        // surfaceRect is the SDL_Rect used to draw the object on screen - not to be confused with SDL_Surface.
        internal static bool CanClickOnPixel(IntPtr surfacePtr, SDL_Rect surfaceRect, SDL_Point mouse)
        {
            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
            SDL_Point point = GetRelativeAndScaledCoordinatesForSurface(surfaceRect.w, surfaceRect.h, surfaceRect.x, surfaceRect.y, mouse.x, mouse.y);

            // Check if coordinates are within the bounds of the surface
            if (point.x < 0 || point.y < 0 || point.x >= OriginalWidth || point.y >= OriginalHeight)
                return false;

            // Get SDL_PixelFormat information for surface
            SDL_PixelFormat pixelFormat = Marshal.PtrToStructure<SDL_PixelFormat>(surface.format);

            // Only god knows
            int bpp = pixelFormat.BytesPerPixel;
            IntPtr pixelAddr = IntPtr.Add(surface.pixels, point.y * surface.pitch + point.x * bpp);

            // Assign correct pixel based on bit-wise calculated pixel address
            uint pixel = bpp switch
            {
                //4 => (uint)Marshal.ReadInt32(pixelAddr),
                //3 => BitConverter.IsLittleEndian ?
                //     (uint)(Marshal.ReadByte(pixelAddr) | Marshal.ReadByte(pixelAddr, 1) << 8 | Marshal.ReadByte(pixelAddr, 2) << 16) :
                //     (uint)(Marshal.ReadByte(pixelAddr, 2) | Marshal.ReadByte(pixelAddr, 1) << 8 | Marshal.ReadByte(pixelAddr) << 16),
                //2 => (uint)Marshal.ReadInt16(pixelAddr),
                1 => Marshal.ReadByte(pixelAddr),
                _ => throw new NotSupportedException("Unsupported bit depth.")
            };

            // Acquire RGBA values 
            SDL_Color color;
            SDL_GetRGBA(pixel, surface.format, out color.r, out color.g, out color.b, out color.a);
            return color.a > 0;
        }
    }
}
