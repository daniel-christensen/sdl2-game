﻿namespace LayeredSDL2Demo.Entities
{
    internal class Charmander : Pokemon
    {
        internal Charmander(
            Player player,
            int positionX,
            int positionY,
            int width,
            int height)
        : base(
            player,
            positionX,
            positionY,
            width,
            height,
            DefaultTextureALocation,
            DefaultTextureBLocation,
            DefaultCryLocation)
        {
        }

        private const string DefaultTextureALocation = "Resources/charmander_A.png";

        private const string DefaultTextureBLocation = "Resources/charmander_B.png";

        private const string DefaultCryLocation = "Resources/charmander.mp3";
    }
}
