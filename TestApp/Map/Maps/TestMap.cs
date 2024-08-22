using TestApp.Interfaces;
using TestApp.Helper;
using static SDL2.SDL;

namespace TestApp.Map.Maps
{
    internal class TestMap : IMap, IRegisterable
    {
        public Game Game { get; }

        public int LengthX => CellLengthX * MapData.GetLength(0);

        public int LengthY => CellLengthY * MapData.GetLength(1);

        public int CellLengthX => Game.Configuration.GridCellXLength;

        public int CellLengthY => Game.Configuration.GridCellYLength;

        public Cell[,] MapData { get; set; }

        public TestMap(Game game, Cell[,] mapData)
        {
            Game = game;
            MapData = mapData;
        }

        public void Draw(IntPtr renderer)
        {
            (int, int) brush = GraphicsHelper.CalculateRelativeCentreCoordinates(
                Game.Configuration.WindowXSize,
                Game.Configuration.WindowYSize,
                LengthX,
                LengthY
            );

            //brush = (0, 0);

            int startY = brush.Item2;

            for (int i = 0; i < MapData.GetLength(0); i++)
            {
                for (int j = 0; j < MapData.GetLength(1); j++)
                {
                    byte red = MapData[i, j].Red;
                    byte green = MapData[i, j].Green;
                    byte blue = MapData[i, j].Blue;

                    SDL_Rect rect = new SDL_Rect()
                    {
                        x = brush.Item1,
                        y = brush.Item2,
                        w = MapData[i, j].LengthX,
                        h = MapData[i, j].LengthY
                    };
                    
                    SDL_SetRenderDrawColor(renderer, red, green, blue, 255);
                    SDL_RenderFillRect(renderer, ref rect);

                    brush.Item2 = brush.Item2 + Game.Configuration.GridCellYLength;
                }
                brush.Item1 = brush.Item1 + Game.Configuration.GridCellYLength;
                brush.Item2 = startY;
            }
        }

        internal static Cell[,] GetSelfMapData(Game game)
        {
            int xsize = game.Configuration.GridCellXLength;
            int ysize = game.Configuration.GridCellYLength;

            return new Cell[,]
            {
                { new Cell(xsize, ysize, 255, 0, 0), new Cell(xsize, ysize, 155, 0, 0), new Cell(xsize, ysize, 55, 0, 0) },
                { new Cell(xsize, ysize, 0, 255, 0), new Cell(xsize, ysize, 0, 155, 0), new Cell(xsize, ysize, 0, 55, 0) },
                { new Cell(xsize, ysize, 0, 0, 255), new Cell(xsize, ysize, 0, 0, 155), new Cell(xsize, ysize, 0, 0, 55) }
            };
        }
    }
}
