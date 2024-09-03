using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace LayeredSDL2Demo.Helpers
{
    internal class TextureBoundsAnalyser
    {
        internal int TextureX { get; private set; }

        internal int TextureY { get; private set; }

        internal int TextureWidth { get; private set; }

        internal int TextureHeight { get; private set; }

        internal int OriginTextureWidth { get; }

        internal int OriginTextureHeight { get; }

        internal float ScaleForX => (float)TextureWidth / OriginTextureWidth;

        internal float ScaleForY => (float)TextureHeight / OriginTextureHeight;

        internal IntPtr SurfacePointer { get; }
        
        internal TextureBoundsAnalyser(int textureX, int textureY, int textureWidth, int textureHeight,
            int originTextureWidth, int originTextureHeight)
        {
            TextureX = textureX;
            TextureY = textureY;
            TextureWidth = textureWidth;
            TextureHeight = textureHeight;
            OriginTextureWidth = originTextureWidth;
            OriginTextureHeight = originTextureHeight;
        }

        private SDL_Point ScaleDownPoint(int sourceX, int sourceY, float scaleForX, float scaleForY)
        {
            float scaledX = sourceX / scaleForX;
            float scaledY = sourceY / scaleForY;
            SDL_Point scaledPoint = new SDL_Point()
            {
                x = (int)Math.Round(scaledX),
                y = (int)Math.Round(scaledY)
            };

            return scaledPoint;
        }

        private SDL_Point ScaleUpPoint(int sourceX, int sourceY, float scaleForX, float scaleForY)
        {
            float scaledX = sourceX * scaleForX;
            float scaledY = sourceY * scaleForY;
            SDL_Point scaledPoint = new SDL_Point()
            {
                x = (int)Math.Round(scaledX),
                y = (int)Math.Round(scaledY)
            };

            return scaledPoint;
        }

        private SDL_Point GetScaledTexturePoint(int sourceTextureX, int sourceTextureY, float scaleForX, float scaleForY)
        {
            return ScaleDownPoint(sourceTextureX, sourceTextureY, scaleForX, scaleForY);
        }

        private SDL_Point GetScaledRelativeMousePoint(int sourceMouseX, int sourceMouseY, float scaleForX, float scaleForY, int sourceTextureX, int sourceTextureY)
        {
            SDL_Point scaledTexturePoint = GetScaledTexturePoint(sourceTextureX, sourceTextureY, scaleForX, scaleForY);
            SDL_Point scaledMousePoint = ScaleDownPoint(sourceMouseX, sourceMouseY, scaleForX, scaleForY);
            SDL_Point scaledRelativeMousePoint = new SDL_Point()
            {
                x = scaledMousePoint.x - scaledTexturePoint.x,
                y = scaledMousePoint.y - scaledTexturePoint.y
            };

            return scaledRelativeMousePoint;
        }

        private SDL_Color GetColorInformationFromPixel(uint pixel, IntPtr format)
        {
            SDL_Color color;
            SDL_GetRGBA(pixel, format, out color.r, out color.g, out color.b, out color.a);
            return color;
        }

        internal bool IsPixelClickable(SDL_Surface displayedSurface, SDL_Point sourceMouse)
        {
            SDL_Point scaledRelativeMousePoint = GetScaledRelativeMousePoint(
                sourceMouse.x,
                sourceMouse.y,
                ScaleForX,
                ScaleForY,
                TextureX,
                TextureY);

            // Check if scaled and relative mouse point is outside of origin size bounds
            if (scaledRelativeMousePoint.x < 0 || scaledRelativeMousePoint.y < 0 ||
                scaledRelativeMousePoint.x >= OriginTextureWidth || scaledRelativeMousePoint.y >= OriginTextureHeight)
                return false;

            SDL_PixelFormat pixelFormat = Marshal.PtrToStructure<SDL_PixelFormat>(displayedSurface.format);

            IntPtr pixelAddress = GetPixelAddress(
                pixelFormat.BytesPerPixel,
                displayedSurface.pixels,
                displayedSurface.pitch,
                scaledRelativeMousePoint.x,
                scaledRelativeMousePoint.y);

            uint pixel = GetPixel(pixelAddress, pixelFormat.BytesPerPixel);

            SDL_Color color = GetColorInformationFromPixel(pixel, displayedSurface.format);

            return color.a > 0;
        }

        internal IntPtr GetPixelAddress(byte bytesPerPixel, nint pixels, int pitch, int x, int y)
        {
            return IntPtr.Add(pixels, y * pitch + x * bytesPerPixel);
        }

        internal static uint GetPixel(IntPtr pixelAddress, int bytesPerPixel)
        {
            switch (bytesPerPixel)
            {
                case 1:
                    return Marshal.ReadByte(pixelAddress);
                case 2:
                    throw new NotImplementedException("To be tested");
                    return (uint)Marshal.ReadInt16(pixelAddress);
                case 3:
                    throw new NotImplementedException("To be tested");
                    return BitConverter.IsLittleEndian ?
                    (uint)(Marshal.ReadByte(pixelAddress) | Marshal.ReadByte(pixelAddress, 1) << 8 | Marshal.ReadByte(pixelAddress, 2) << 16) :
                    (uint)(Marshal.ReadByte(pixelAddress, 2) | Marshal.ReadByte(pixelAddress, 1) << 8 | Marshal.ReadByte(pixelAddress) << 16);
                case 4:
                    throw new NotImplementedException("To be tested");
                    return (uint)Marshal.ReadInt32(pixelAddress);
            }

            throw new NotSupportedException("Unsupported bit depth.");
        }

        // Finds the first pixel with an alpha above 0 scanning from bottom-right to top-left
        // Based on the simulated bitmap below: the returned point would be (4, 7)
        // X 0 1 2 3 4 5 6 7 8 9
        // 0 - - - - - - - - - -
        // 1 - - - - - - - - - -
        // 2 - - - X X - - - - -
        // 3 - - X X X X - - - -
        // 4 - X X X X X X X - -
        // 5 - - X X X X X X - -
        // 6 - - X X X X X X - -
        // 7 - - - X X - - - - -
        // 8 - - - - - - - - - -
        // 9 - - - - - - - - - -
        private SDL_Point PixelScanBottomRightToLeft(SDL_Surface surface, SDL_PixelFormat pixelFormat)
        {
            SDL_Point bottomPoint = new SDL_Point();
            for (int y = OriginTextureHeight - 1; y >= 0; y--)
            {
                for (int x = OriginTextureWidth - 1; x >= 0; x--)
                {
                    IntPtr pixelAddress = GetPixelAddress(pixelFormat.BytesPerPixel, surface.pixels, surface.pitch, x, y);
                    uint pixel = GetPixel(pixelAddress, pixelFormat.BytesPerPixel);
                    SDL_Color color = GetColorInformationFromPixel(pixel, surface.format);
                    if (color.a > 0)
                    {
                        bottomPoint.x = x;
                        bottomPoint.y = y;
                        return bottomPoint;
                    }
                }
            }

            throw new InvalidOperationException("Could not find coloured pixel");
        }

        // Finds the first pixel with an alpha above 0 scanning from top-right to bottom-left
        // Based on the simulated bitmap below: the returned point would be (7, 4)
        // X 0 1 2 3 4 5 6 7 8 9
        // 0 - - - - - - - - - -
        // 1 - - - - - - - - - -
        // 2 - - - X X - - - - -
        // 3 - - X X X X - - - -
        // 4 - X X X X X X X - -
        // 5 - - X X X X X X - -
        // 6 - - X X X X X X - -
        // 7 - - - X X - - - - -
        // 8 - - - - - - - - - -
        // 9 - - - - - - - - - -
        private SDL_Point PixelScanTopRightToBottom(SDL_Surface surface, SDL_PixelFormat pixelFormat)
        {
            SDL_Point rightPoint = new SDL_Point();
            for (int x = OriginTextureHeight - 1; x >= 0; x--)
            {
                for (int y = 0; y < OriginTextureHeight - 1; y++)
                {
                    IntPtr pixelAddress = GetPixelAddress(pixelFormat.BytesPerPixel, surface.pixels, surface.pitch, x, y);
                    uint pixel = GetPixel(pixelAddress, pixelFormat.BytesPerPixel);
                    SDL_Color color = GetColorInformationFromPixel(pixel, surface.format);
                    if (color.a > 0)
                    {
                        rightPoint.x = x;
                        rightPoint.y = y;
                        return rightPoint;
                    }
                }
            }

            throw new InvalidOperationException("Could not find coloured pixel");
        }

        // Finds the first pixel with an alpha above 0 scanning from top-left to bottom-right
        // Based on the simulated bitmap below: the returned point would be (3, 2)
        // X 0 1 2 3 4 5 6 7 8 9
        // 0 - - - - - - - - - -
        // 1 - - - - - - - - - -
        // 2 - - - X X - - - - -
        // 3 - - X X X X - - - -
        // 4 - X X X X X X X - -
        // 5 - - X X X X X X - -
        // 6 - - X X X X X X - -
        // 7 - - - X X - - - - -
        // 8 - - - - - - - - - -
        // 9 - - - - - - - - - -
        private SDL_Point PixelScanTopLeftToRight(SDL_Surface surface, SDL_PixelFormat pixelFormat)
        {
            SDL_Point topPoint = new SDL_Point();
            for (int y = 0; y < OriginTextureHeight - 1; y++)
            {
                for (int x = 0; x < OriginTextureWidth - 1; x++)
                {
                    IntPtr pixelAddress = GetPixelAddress(pixelFormat.BytesPerPixel, surface.pixels, surface.pitch, x, y);
                    uint pixel = GetPixel(pixelAddress, pixelFormat.BytesPerPixel);
                    SDL_Color color = GetColorInformationFromPixel(pixel, surface.format);
                    if (color.a > 0)
                    {
                        topPoint.x = x;
                        topPoint.y = y;
                        return topPoint;
                    }
                }
            }

            throw new InvalidOperationException("Could not find coloured pixel");
        }

        // Finds the first pixel with an alpha above 0 scanning from bottom-left to top-right
        // Based on the simulated bitmap below: the returned point would be (1, 4)
        // X 0 1 2 3 4 5 6 7 8 9
        // 0 - - - - - - - - - -
        // 1 - - - - - - - - - -
        // 2 - - - X X - - - - -
        // 3 - - X X X X - - - -
        // 4 - X X X X X X X - -
        // 5 - - X X X X X X - -
        // 6 - - X X X X X X - -
        // 7 - - - X X - - - - -
        // 8 - - - - - - - - - -
        // 9 - - - - - - - - - -
        private SDL_Point PixelScanBottomLeftToTop(SDL_Surface surface, SDL_PixelFormat pixelFormat)
        {
            SDL_Point leftPoint = new SDL_Point();
            for (int x = 0; x < OriginTextureWidth - 1; x++)
            {
                for (int y = OriginTextureWidth - 1; y >= 0; y--)
                {
                    IntPtr pixelAddress = GetPixelAddress(pixelFormat.BytesPerPixel, surface.pixels, surface.pitch, x, y);
                    uint pixel = GetPixel(pixelAddress, pixelFormat.BytesPerPixel);
                    SDL_Color color = GetColorInformationFromPixel(pixel, surface.format);
                    if (color.a > 0)
                    {
                        leftPoint.x = x;
                        leftPoint.y = y;
                        return leftPoint;
                    }
                }
            }

            throw new InvalidOperationException("Could not find coloured pixel");
        }

        internal SDL_Rect GetContentRect(SDL_Surface surface, SDL_PixelFormat pixelFormat)
        {
            // Acquire the most top, most left, most right and most bottom point where the first coloured pixel exists
            SDL_Point topPoint = PixelScanTopLeftToRight(surface, pixelFormat);
            SDL_Point leftPoint = PixelScanBottomLeftToTop(surface, pixelFormat);
            SDL_Point rightPoint = PixelScanTopRightToBottom(surface, pixelFormat);
            SDL_Point bottomPoint = PixelScanBottomRightToLeft(surface, pixelFormat);

            // Increase right and bottom point so that the border is after and accounts for (-1) offset
            rightPoint.x++;
            bottomPoint.y++;

            // Scale points to meet texture rectangle size
            SDL_Point scaledTopPoint = ScaleUpPoint(topPoint.x, topPoint.y, ScaleForX, ScaleForY);
            SDL_Point scaledLeftPoint = ScaleUpPoint(leftPoint.x, leftPoint.y, ScaleForX, ScaleForY);
            SDL_Point scaledRightPoint = ScaleUpPoint(rightPoint.x, rightPoint.y, ScaleForX, ScaleForY);
            SDL_Point scaledBottomPoint = ScaleUpPoint(bottomPoint.x, bottomPoint.y, ScaleForX, ScaleForY);

            // Create content rectangle
            SDL_Rect contentRect = new SDL_Rect();
            contentRect.x = scaledLeftPoint.x;
            contentRect.y = scaledTopPoint.y;
            contentRect.w = scaledRightPoint.x - contentRect.x;
            contentRect.h = scaledBottomPoint.y - contentRect.y;

            // Offset the content rectangle to the current position of the texture rectangle
            contentRect.x += TextureX;
            contentRect.y += TextureY;

            return contentRect;
        }
    }
}
