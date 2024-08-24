﻿namespace LayeredSDL2Demo.Entities
{
    internal class Charizard : Pokemon
    {
        internal Charizard(
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

        private const string DefaultTextureALocation = "Resources/charizard_A.png";

        private const string DefaultTextureBLocation = "Resources/charizard_B.png";

        private const string DefaultCryLocation = "Resources/charizard.mp3";
    }
}
