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
using MarioWorldSharp.Entities;
using KdTree;
using KdTree.Math;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

namespace MarioWorldSharp.Levels
{
    public class Level
    {
        private Chunk[,] chunks;

        private double _midX;
        private double _midY;
        private double _xPos;
        private double _yPos;
        private int _EntityCount;
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
        public KdTree<double, EntityData> Entities { get; set; }
        public int EntityCount { get => _EntityCount; }

        private Level nextLayer;
        private Level prevLayer;
        private int _width;
        private int _height;
        public int Width { get => _width; }
        public int Height { get => _height; }

        private double XScrollMultiplier;
        private double YScrollMultiplier;

        private int scrollingHorz;
        private int scrollingVert;

        public Level()
        {
            #region Autogenerate Level
            FormChunks(new Ledge() 
            { 
                X = 0,
                Y = 11, 
                Width = 256, 
                Height = 4 
            });

            _width = chunks.GetLength(0) * 16;
            _height = chunks.GetLength(1) * 16;
            Entities = new KdTree<double, EntityData>(2, new DoubleMath());
            _EntityCount = 0;
            X = 0; Y = 0;

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
            for (double i = 0; i < _width * 16; i += 128)
                AddEntity(i, 60, EntityID.GreenShellessKoopa);
            Entities.Balance();

            #endregion
        }

        public Level(string levelDataPath)
        {

        }

        private void AddEntity(double x, double y, EntityID s, params object[] args)
        {
            Entities.Add(new[] { x, y }, new EntityData { ID = s, Index = EntityCount, Args = args });
            _EntityCount++;
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
            if (newX >= _width * 16 - 400)
                newX = _width * 16 - 401;
            if (newY >= _height * 16 - 224)
                newY = _height * 16 - 225;

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

        private void FormChunks(params ILevelObject[] objects)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int totalWidth = 0;
            int totalHeight = 0;

            foreach(ILevelObject o in objects)
            {
                minX = o.X < minX ? o.X : minX;
                minY = o.Y < minY ? o.Y : minY;
            }
            foreach(ILevelObject o in objects)
            {
                int rX = o.X - minX;
                int rY = o.Y - minY;
                totalWidth = rX + o.Width > totalWidth ? rX + o.Width : totalWidth;
                totalHeight = rY + o.Height > totalHeight ? rY + o.Height : totalHeight;
            }

            int chunkArrayWidth = (int)Math.Ceiling( (double)(minX + totalWidth) / 16.0 );
            int chunkArrayHeight = (int)Math.Ceiling( (double)(minY + totalHeight) / 16.0 );
            if (chunks == null)
                chunks = new Chunk[chunkArrayWidth, chunkArrayHeight];
            else if (chunks.GetLength(0) < chunkArrayWidth || chunks.GetLength(1) < chunkArrayHeight)
            {
                Chunk[,] newChunks = new Chunk[chunkArrayWidth, chunkArrayHeight];
                chunks.CopyTo(newChunks, 0);
                chunks = newChunks;
            }

            foreach (ILevelObject o in objects)
            {
                short[,] build = o.Build();
                for (int i = 0; i < build.GetLength(1); i++)
                {
                    for (int j = 0; j < build.GetLength(0); j++)
                    {
                        int x = j + o.X;
                        int y = i + o.Y;

                        if (chunks[x / 16, y / 16] == null)
                            chunks[x / 16, y / 16] = new Chunk() { X = x / 16, Y = y / 16 };

                        chunks[x / 16, y / 16].GetMap16Array()[x % 16, y % 16] = build[j, i];
                    }
                }
            }
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

                    if (getX >= _width)
                        getX = _width - 1;
                    if (getY >= _height)
                        getY = _height - 1;

                    if (chunks[getX / 16, getY / 16] == null)
                        ret[i, j] = 0x25;
                    else
                        ret[i, j] = chunks[getX / 16, getY / 16].GetMap16(getX % 16, getY % 16);
                }
            }
            return ret;
        }

        public Block GetBlock(int x, int y)
        {
            if (x >= _width)
                x = _width - 1;
            if (y >= _height)
                y = _height - 1;

            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            if (chunks[x / 16, y / 16] == null)
                return BlockList.EMPTY_BLOCK;

            return chunks[x / 16, y / 16].GetBlock(x % 16, y % 16);
        }

        public Block GetBlockFromPosition(double x, double y)
        {
            return GetBlock((int)x / 16, (int)y / 16);
        }

        public void RemoveEntity(EntityData s)
        {
            if (s.Index == -1)
                throw new UnindexedEntityException($"{s} was spawned by other means. You cannot remove this sprite from the level.");

            if (Entities.TryFindValueReference(s, out double[] point) )
            {
                Entities.RemoveSpecific(point, s);
                _EntityCount--;
            }
        }

        public void SpawnEntities()
        {
            KdTreeNode<double, EntityData>[] sprites = Entities.RadialSearch(new[] { _midX, _midY }, 264.0);

            foreach (KdTreeNode<double, EntityData> n in sprites)
            {
                EntityData d = n.Value;
                if (!d.Spawned)
                {
                    EntitySpawner.SpawnEntity(n.Point, d);
                }
            }
        }

        public void SpawnEntitiesOnScroll()
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
            KdTreeNode<double, EntityData>[] sprites = Entities.RadialSearch(new[] { X + x, Y + y }, r);

            foreach (KdTreeNode<double, EntityData> n in sprites)
            {
                EntityData d = n.Value;
                double x2 = n.Point[0] - X;
                double y2 = n.Point[1] - Y;
                if (!d.Spawned)
                {
                    if ((scrollingHorz < 0 && x2 < 0.0) ||
                        (scrollingHorz > 0 && x2 > 400.0) ||
                        (scrollingVert < 0 && y2 < 0.0) ||
                        (scrollingVert > 0 && y2 > 224.0) )
                    {
                        EntitySpawner.SpawnEntity(n.Point, d);
                    }
                }
            }
        }

        internal int GetSpriteCount()
        {
            return EntityCount;
        }
    }
    
    public static class Map16BlockPointers
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

            Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK, Blocks.BlockList.LEDGE_BLOCK,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            Blocks.BlockList.SOLID_BLOCK, null, null, Blocks.BlockList.SOLID_BLOCK, Blocks.BlockList.SOLID_BLOCK, Blocks.BlockList.SOLID_BLOCK, Blocks.BlockList.SOLID_BLOCK, null, null, null, null, null, null, null, null, null,
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
