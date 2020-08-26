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
using MarioWorldSharp.Blocks;
using MarioWorldSharp.Sprite;
using KdTree;
using KdTree.Math;
using System.Reflection.Metadata.Ecma335;

namespace MarioWorldSharp.Levels
{
    public class Level
    {
        private Chunk[,] chunks;

        private double _midX;
        private double _midY;
        private double _xPos;
        private double _yPos;
        private int _SpriteCount;
        public double X 
        {
            get { return _xPos; }
            set
            {
                _xPos = value;
                _midX = _xPos + 200.0;
            }
        }
        public double Y
        {
            get { return _yPos; }
            set
            {
                _yPos = value;
                _midY = _yPos + 112.0;
            }
        }
        public KdTree<double, SpriteData> Sprites { get; set; }
        public int SpriteCount { get => _SpriteCount; }

        private Level nextLayer;
        private Level prevLayer;
        private int width;
        private int height;
        private double XScrollMultiplier;
        private double YScrollMultiplier;

        private int scrollingHorz;
        private int scrollingVert;

        public Level()
        {
            chunks = new Chunk[16, 1];
            for (int i = 0; i < chunks.GetLength(0); i++)
                chunks[i, 0] = new Chunk();
            width = chunks.GetLength(0) * 16;
            height = chunks.GetLength(1) * 16;
            Sprites = new KdTree<double, SpriteData>(2, new DoubleMath());
            _SpriteCount = 0;
            X = 0; Y = 0;

            #region Autogenerate Level
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

            Random rand = new Random();
            for (double i = 0; i < width * 16; i += 24)
                AddSprite(i, 60, SpriteID.GreenShellessKoopa);
            Sprites.Balance();

            #endregion
        }

        private void AddSprite(double x, double y, SpriteID s, params object[] args)
        {
            Sprites.Add(new[] { x, y }, new SpriteData { ID = s, Index = SpriteCount, Args = args });
            _SpriteCount++;
        }

        public void Scroll(double playerX, double playerY)
        {
            double newX = X;
            double newY = Y;
            if (playerX - newX >= 200 + 24)
                newX = playerX - 200 - 24;
            else if (playerX - newX <= 200 - 24)
                newX = playerX - 200 + 24;

            if (newX < 0)
                newX = 0;
            if (newY < 0)
                newY = 0;
            if (newX >= width * 16 - 400)
                newX = width * 16 - 401;
            if (newY >= height * 16 - 224)
                newY = height * 16 - 225;

            if (newX > X)
                scrollingHorz = 1;
            else if (newX < X)
                scrollingHorz = -1;
            else
                scrollingHorz = 0;

            if (newY > Y)
                scrollingVert = 1;
            else if (newY < Y)
                scrollingVert = -1;
            else
                scrollingVert = 0;
            X = newX; Y = newY;
        }

        public Level(Chunk[,] Level)
        {
            chunks = Level;
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

        public Block GetMap16(int x, int y)
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

        public Block GetMap16FromPosition(double x, double y)
        {
            return GetMap16((int)x / 16, (int)y / 16);
        }

        public void RemoveSprite(SpriteData s)
        {
            if (s.Index == -1)
                throw new UnindexedSpriteException($"{s} was spawned by other means. You cannot remove this sprite from the level.");

            if (Sprites.TryFindValueReference(s, out double[] point) )
            {
                Sprites.RemoveSpecific(point, s);
                _SpriteCount--;
            }
        }

        public void SpawnSprites()
        {
            KdTreeNode<double, SpriteData>[] sprites = Sprites.RadialSearch(new[] { _midX, _midY }, 264.0);

            foreach (KdTreeNode<double, SpriteData> n in sprites)
            {
                SpriteData d = n.Value;
                if (!d.Spawned)
                {
                    SpriteSpawner.SpawnSprite(n.Point, d);
                }
            }
        }

        public void SpawnSpritesOnScroll()
        {
            if (scrollingHorz == 0 && scrollingVert == 0)
                return;

            double x = 200;
            double y = 112;
            double r = 0;
            switch (scrollingHorz)
            {
                case 1:
                    x = 400;
                    break;
                case -1:
                    x = 0;
                    break;
                case 0:
                    r += 200;
                    break;
            }
            switch (scrollingVert)
            {
                case 1:
                    y = 224;
                    break;
                case -1:
                    y = 0;
                    break;
                case 0:
                    r += 112;
                    break;
            }
            KdTreeNode<double, SpriteData>[] sprites = Sprites.RadialSearch(new[] { X + x, Y + y }, r);

            foreach (KdTreeNode<double, SpriteData> n in sprites)
            {
                SpriteData d = n.Value;
                double x2 = n.Point[0] - X;
                double y2 = n.Point[1] - Y;
                if (!d.Spawned)
                {
                    if ((scrollingHorz < 0 && x2 < 0.0) ||
                        (scrollingHorz > 0 && x2 > 400.0) ||
                        (scrollingVert < 0 && y2 < 0.0) ||
                        (scrollingVert > 0 && y2 > 224.0) )
                    {
                        SpriteSpawner.SpawnSprite(n.Point, d);
                    }
                }
            }
        }

        internal int GetSpriteCount()
        {
            return SpriteCount;
        }
    }
    
    public static class BlockList
    {
        public static Block[] Map16 =
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

            Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK, Blocks.Blocks.LEDGE_BLOCK,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            Blocks.Blocks.SOLID_BLOCK, null, null, Blocks.Blocks.SOLID_BLOCK, Blocks.Blocks.SOLID_BLOCK, Blocks.Blocks.SOLID_BLOCK, Blocks.Blocks.SOLID_BLOCK, null, null, null, null, null, null, null, null, null,
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
