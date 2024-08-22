using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Map
{
    internal class Cell
    {
        internal int LengthX { get; }

        internal int LengthY { get; }

        internal byte Red { get; }

        internal byte Green { get; }

        internal byte Blue { get; }

        public Cell(int lengthX, int lengthY, byte red, byte green, byte blue)
        {
            LengthX = lengthX;
            LengthY = lengthY;
            Red = red;
            Green = green;
            Blue = blue;
        }
    }
}
