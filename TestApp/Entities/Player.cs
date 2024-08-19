using TestApp.Interfaces;
using Vanara.PInvoke;

namespace TestApp.Entities
{
    internal class Player : IEntity, IRegisterable
    {
        public Game Game { get; }

        public int XPosition { get; set; }

        public int YPosition { get; set; }

        public bool Visible { get; set; }

        public bool Enabled { get; set; }

        public Player(Game game, int xPos, int yPos, bool visible, bool enabled)
        {
            Game = game;
            XPosition = xPos;
            YPosition = yPos;
            Visible = visible;
            Enabled = enabled;
        }
    }
}
