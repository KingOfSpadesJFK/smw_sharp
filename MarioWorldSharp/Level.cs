using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using MarioWorldSharp.Block;

namespace MarioWorldSharp
{
    public class Level
    {
        private Chunk[,] chunks;
        public double X { get; set; }
        public double Y { get; set; }
        private Level nextLayer;
        private Level prevLayer;
        private int width;
        private int height;
        private double XScrollMultiplier;
        private double YScrollMultiplier;

        public Level()
        {
            chunks = new Chunk[16, 1];
            for (int i = 0; i < chunks.GetLength(0); i++)
                chunks[i, 0] = new Chunk();
            width = chunks.GetLength(0) * 16;
            height = chunks.GetLength(1) * 16;
            short[,] chunk = chunks[1, 0].GetMap16Array();
            chunk[4, 9] = 0x133;
            chunk[5, 9] = 0x134;
            chunk[4, 10] = 0x135;
            chunk[5, 10] = 0x136;
            chunk[1, 7] = 0x130;
            chunk[0, 7] = 0x130;
            chunk[1, 6] = 0x130;
            chunk[1, 5] = 0x130;
            chunk = chunks[2, 0].GetMap16Array();
            chunk[3, 7] = 0x101;
            chunk[4, 7] = 0x100;
            chunk[5, 7] = 0x100;
            chunk[6, 7] = 0x103;
            chunk[3, 8] = 0x40;
            chunk[4, 8] = 0x3F;
            chunk[5, 8] = 0x3F;
            chunk[6, 8] = 0x41;
            chunk[3, 9] = 0x40;
            chunk[4, 9] = 0x3F;
            chunk[5, 9] = 0x3F;
            chunk[6, 9] = 0x41;
            chunk[3, 10] = 0x40;
            chunk[4, 10] = 0x3F;
            chunk[5, 10] = 0x3F;
            chunk[6, 10] = 0x41;
            chunk[10, 10] = 0x130;
            chunk[11, 9] = 0x130;
        }

        public void Scroll(double playerX, double playerY)
        {
            if (playerX - X >= 200 + 24)
                X = playerX - 200 - 24;
            else if (playerX - X <= 200 - 24)
                X = playerX - 200 + 24;

            if (X < 0)
                X = 0;
            if (Y < 0)
                Y = 0;
            if (X >= width * 16 - 400)
                X = width * 16 - 401;
            if (Y >= height * 16 - 224)
                Y = height * 16 - 225;
        }

        public Level(Chunk[,] level)
        {
            chunks = level;
        }

        public Level GetNextLayer()
        {
            return nextLayer;
        }

        public short[,] GetCameraSelection()
        {
            int xStart = (int) X / 16;
            int yStart = (int) Y / 16;
            int xEnd = xStart + 27;
            int yEnd = xStart + 16;

            short[,] ret = new short[xEnd - xStart, yEnd - yStart];
            for (int i = 0; i < ret.GetLength(0); i++)
            {
                for (int j = 0; j < ret.GetLength(1); j++)
                {
                    int getX = xStart + i;
                    int getY = yStart + j;

                    if (getX < 0)
                        getX = 0;
                    if (getY < 0)
                        getY = 0;

                    if (getX >= width)
                        getX = width - 1;
                    if (getY >= height)
                        getY = height - 1;

                    if (chunks[getX / 16, getY / 16] == null)
                        ret[i, j] = 0x25;
                    else
                        ret[i, j] = chunks[getX / 16, getY / 16].GetMap16Short(getX % 16, getY % 16);
                }
            }
            return ret;
        }

        public AbstractBlock GetMap16(int x, int y)
        {
            if (x >= width)
                x = width - 1;
            if (y >= height)
                y = height - 1;

            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            return chunks[x / 16, y / 16].GetMap16(x % 16, y % 16);
        }

        public AbstractBlock GetMap16FromPosition(double x, double y)
        {
            return GetMap16((int)x / 16, (int)y / 16);
        }
    }
    
    public static class BlockList
    {
        public static AbstractBlock[] Map16 =
        {
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,

            Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK, Blocks.LEDGE_BLOCK,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            Blocks.SOLID_BLOCK, null, null, Blocks.SOLID_BLOCK, Blocks.SOLID_BLOCK, Blocks.SOLID_BLOCK, Blocks.SOLID_BLOCK, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
        };
    }
}
