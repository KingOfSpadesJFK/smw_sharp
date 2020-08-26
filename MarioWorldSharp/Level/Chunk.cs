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

        public Chunk()
        {
            map16 = new short[16, 16];
            for (int i = 0; i < map16.GetLength(1); i++)
            {
                for (int j = 0; j < map16.GetLength(0); j++)
                {
                    if (i < 11)
                        map16[j, i] = 0x25;
                    else if (i == 11)
                        map16[j, i] = 0x100;
                    else
                        map16[j, i] = 0x3F;
                }
            }
        }

        public Chunk(short[,] chunk)
        {
            map16 = chunk;
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
                return Blocks.Blocks.EMPTY_BLOCK;
            if (x < 0 || x >= 16)
                throw new IndexOutOfRangeException();
            if (y < 0 || y >= 16)
                throw new IndexOutOfRangeException();

            Block ret = BlockList.Map16[map16[x, y]];
            if (ret == null)
                return Blocks.Blocks.EMPTY_BLOCK;
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
