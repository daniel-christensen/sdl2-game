using TestApp.Interfaces;

namespace TestApp.Components
{
    internal class Logic : IComponent, IRegisterable
    {
        public Game Game { get; }

        public Logic(Game game)
        {
            Game = game;
        }

        public void Initialise()
        {
            
        }

        public void Update()
        {
            
        }

        public void CleanUp()
        {
            
        }
    }
}
