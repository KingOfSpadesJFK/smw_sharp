using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarioWorldSharp.Blocks;

namespace MarioWorldSharp.Levels
{
    public class Chunk
    {
        private short[,] map16;
        public int X { get; set; }
        public int Y { get; set; }

        readonly short EmptyBlock = 0x25;

        public Chunk() : this(0) {}

        public Chunk(short[,] chunk)
        {
            if (chunk.GetLength(0) == 16 && chunk.GetLength(1) == 16)
                map16 = chunk;
            else
                throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Generates a new chunk
        /// </summary>
        /// <param name="type">0: Empty chunk, 1: Ledge chunk</param>
        public Chunk(int type)
        {
            map16 = new short[16, 16];
            switch (type)
            {
                case 1:
                    for (int i = 0; i < map16.GetLength(1); i++)
                    {
                        for (int j = 0; j < map16.GetLength(0); j++)
                        {
                            if (i < 11)
                                map16[j, i] = EmptyBlock;
                            else if (i == 11)
                                map16[j, i] = 0x100;
                            else
                                map16[j, i] = 0x3F;
                        }
                    }
                    break;

                default:
                    for (int i = 0; i < map16.GetLength(1); i++)
                        for (int j = 0; j < map16.GetLength(0); j++)
                            map16[j, i] = EmptyBlock;
                    break;
            }
        }

        public bool IsEmptyChunk()
        {
            return map16 == null;
        }

        public short[,] GetMap16Array()
        {
            return map16;
        }

        public Block GetMap16(int x, int y)
        {
            if (map16 == null)
                return Blocks.BlockList.EMPTY_BLOCK;
            if (x < 0 || x >= 16)
                throw new IndexOutOfRangeException();
            if (y < 0 || y >= 16)
                throw new IndexOutOfRangeException();

            Block ret = Map16BlockPointers.Map16[map16[x, y]];
            if (ret == null)
                return Blocks.BlockList.EMPTY_BLOCK;
            return ret;
        }

        public short GetMap16Short(int x, int y)
        {
            if (map16 == null)
                return 0x25;
            if (x < 0 || x >= 16)
                throw new IndexOutOfRangeException();
            if (y < 0 || y >= 16)
                throw new IndexOutOfRangeException();

            return map16[x, y];
        }
    }
}
