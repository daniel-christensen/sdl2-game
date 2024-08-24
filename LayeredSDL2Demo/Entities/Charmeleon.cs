﻿namespace LayeredSDL2Demo.Entities
{
    internal class Charmeleon : Pokemon
    {
        internal Charmeleon(
            Player player,
            IntPtr renderer,
            int positionX,
            int positionY,
            int width,
            int height)
        : base(
            player,
            renderer,
            positionX,
            positionY,
            width,
            height,
            DefaultTextureALocation,
            DefaultTextureBLocation,
            DefaultCryLocation)
        {
        }

        private const string DefaultTextureALocation = "Resources/charmeleon_A.png";

        private const string DefaultTextureBLocation = "Resources/charmeleon_B.png";

        private const string DefaultCryLocation = "Resources/charmeleon.mp3";
    }
}
