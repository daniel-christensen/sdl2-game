using TestApp.Map;

namespace TestApp.Interfaces
{
    internal interface IMap
    {
        internal int LengthX { get; }

        internal int LengthY { get; }

        internal int CellLengthX { get; }

        internal int CellLengthY { get; }

        internal Cell[,] MapData { get; set; }

        internal void Draw(IntPtr renderer);
    }
}
