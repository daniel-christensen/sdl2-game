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
            uint pixel = GetPixel(pixelAddr, bpp);

            // Acquire RGBA values 
            SDL_Color color;
            SDL_GetRGBA(pixel, surface.format, out color.r, out color.g, out color.b, out color.a);
            return color.a > 0;
        }

        internal static uint GetPixel(IntPtr pixelAddress, int bytesPerPixel)
        {
            switch (bytesPerPixel)
            {
                case 1:
                    return Marshal.ReadByte(pixelAddress);
                //case 2:
                //    return (uint)Marshal.ReadInt16(pixelAddress);
                //case 3:
                //    return BitConverter.IsLittleEndian ?
                //    (uint)(Marshal.ReadByte(pixelAddress) | Marshal.ReadByte(pixelAddress, 1) << 8 | Marshal.ReadByte(pixelAddress, 2) << 16) :
                //    (uint)(Marshal.ReadByte(pixelAddress, 2) | Marshal.ReadByte(pixelAddress, 1) << 8 | Marshal.ReadByte(pixelAddress) << 16);
                //case 4:
                //    return (uint)Marshal.ReadInt32(pixelAddress);
            }

            throw new NotSupportedException("Unsupported bit depth.");
        }

        internal static SDL_Point LastNonTransparentCoordinatesOriginal(IntPtr surfacePtr)
        {
            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
            SDL_PixelFormat pixelFormat = Marshal.PtrToStructure<SDL_PixelFormat>(surface.format);

            SDL_Point lastPoint = new SDL_Point();

            for (int y = surface.h - 1; y >= 0; y--)
            {
                for (int x = surface.w - 1; x >= 0; x--)
                {
                    int bpp = pixelFormat.BytesPerPixel;
                    IntPtr pixelAddr = IntPtr.Add(surface.pixels, y * surface.pitch + x * bpp);
                    uint pixel = GetPixel(pixelAddr, bpp);
                    SDL_Color color;
                    SDL_GetRGBA(pixel, surface.format, out color.r, out color.g, out color.b, out color.a);
                    if (color.a > 0)
                    {
                        lastPoint.x = x;
                        lastPoint.y = y;
                        return lastPoint;
                    }
                }
            }

            return lastPoint;
        }

        internal static SDL_Point ScaleUpPointToDisplaySize(SDL_Point originalPoint, int scaledWidth, int scaledHeight)
        {
            float widthScale = scaledWidth / OriginalWidth;
            float heightScale = scaledHeight / OriginalHeight;
            float scaledX = originalPoint.x * widthScale;
            float scaledY = originalPoint.y * heightScale;
            return new SDL_Point()
            {
                x = (int)Math.Round(scaledX),
                y = (int)Math.Round(scaledY)
            };
        }

        internal static SDL_Point GetRelativePoint(SDL_Point originPoint, SDL_Rect drawableRectange)
        {
            return new SDL_Point()
            {
                x = originPoint.x + drawableRectange.x,
                y = originPoint.y + drawableRectange.y
            };
        }
    }
}
