using MarioWorldSharp.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarioWorldSharp.Levels
{
    public interface ILevelObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public object[] Properties { get; set; }
        public short[,] Build();
    }

    public class StoneObject : ILevelObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public object[] Properties { get; set; }

        public short[,] Build()
        {
            short[,] build = new short[Width, Height];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                    build[j, i] = 0x130;
            }
            return build;
        }
    }

    public class Ledge : ILevelObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public object[] Properties { get; set; }

        public short[,] Build()
        {
            short[,] build = new short[Width, Height];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    short tile = 0x100;
                    if (i != 0)
                        tile = 0x3F;
                    build[j, i] = tile;
                }
            }
            return build;
        }
    }
    public class DirectMap16 : ILevelObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public object[] Properties { get; set; }

        public short[,] Build()
        {
            short[,] build = new short[Width, Height];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                    build[j, i] = (short)Properties[0];
            }
            return build;
        }
    }
}
