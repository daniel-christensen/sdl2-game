namespace TestApp.Interfaces
{
    internal interface IEntity
    {
        internal int XPosition { get; set; }

        internal int YPosition { get; set; }

        internal bool Visible { get; set; }

        internal bool Enabled { get; set; }
    }
}
