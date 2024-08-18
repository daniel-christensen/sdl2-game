namespace TestApp.Interfaces
{
    internal interface IComponent
    {
        internal Game Game { get; }

        internal void Initialise();

        internal void Update();

        internal void CleanUp();
    }
}
