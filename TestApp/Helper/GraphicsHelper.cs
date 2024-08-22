namespace TestApp.Helper
{
    internal static class GraphicsHelper
    {
        /// <summary>
        /// Objects drawn to the screen via SDL2 will draw to the right and down from where their coordiante is.
        /// This means that the exact centre coordinates are not enough.
        /// We must understand the following:
        /// 1. Even numbers (such as resolutions) do not have a 'middle'.
        /// 2. The size of the object to draw.
        /// 3. The window x and y size.
        /// </summary>
        /// <returns>the exact coordinate where to draw the object to perfectly centre it on screen.</returns>
        internal static (int, int) CalculateRelativeCentreCoordinates(int windowXSize, int windowYSize, int objectXSize, int objectYSize)
        {
            int xCentre = windowXSize / 2;
            int yCentre = windowYSize / 2;
            int xAdjusted = xCentre - (objectXSize / 2);
            int yAdjusted = yCentre - (objectYSize / 2);

            return (xAdjusted, yAdjusted);
        }
    }
}
