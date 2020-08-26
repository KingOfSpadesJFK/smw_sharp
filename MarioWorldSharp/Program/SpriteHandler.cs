using KdTree;
using KdTree.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarioWorldSharp.Levels;

namespace MarioWorldSharp.Sprite
{
    public class SpawnArgs : EventArgs
    {
        public ISprite[] SpawnArr;
    }
    public class SpriteHandler
    {
        private static KdTree<double, ISprite> SpritesTree = new KdTree<double, ISprite>(2, new DoubleMath());
        public static List<ISprite> SpriteList { get; set; }
        public static ISprite SpriteLastSpawned { get; set; }
        public static ISprite SpriteLastDepawned { get; set; }
        public static int UpdateCalls
        {
            get
            {
                if (!SMW.SecondPassed)
                    return _Updated2;

                _Updated2 = _Updated;
                return _Updated;
            }
        }
        public static int SpriteCount { get => Count; }

        private static int Count;
        private static int _Updated;
        private static int _Updated2;

        public static void ProcessSprites()
        {
            if (SMW.SecondPassed)
                _Updated = 0;
            SMW.Level.SpawnSpritesOnScroll();
            foreach (ISprite s in SpriteList.ToArray())
            {
                if (s != null)
                {
                    double[] oldPos = new double[] { s.XPosition, s.YPosition };
                    s.Process();
                    if ((s.Data.InteractWithSprites) &&
                        (s.XPosition != oldPos[0] || s.YPosition != oldPos[1] || s.Status == SpriteStatus.NonExistent))
                    { UpdateCollisionTree(s, oldPos, s.Status == SpriteStatus.NonExistent); _Updated++; }
                    else if (s.Status == SpriteStatus.NonExistent)
                    { Count--; SpriteList.Remove(s); SpriteLastDepawned = s; }
                }
            }
        }
        /// <summary>
        /// Returns the collision KdTree of the sprites on-screen. 
        /// Useful for searching for on-screen sprites by distance from each other
        /// </summary>
        /// <returns></returns>
        public static KdTree<double, ISprite> GetSpriteTree()
        {
            return SpritesTree;
        }

        public static ISprite[] GetNearestNeighbors(double[] point, int count)
        {
            KdTreeNode<double, ISprite>[] Sprites = SpritesTree.GetNearestNeighbours(point, count);
            ISprite[] ret = new ISprite[Sprites.Length];
            int i = 0;
            foreach (KdTreeNode<double, ISprite> s in Sprites)
            {
                if (i >= Count)
                    break;

                ret[i] = s.Value;
                i++;
            }

            return ret;
        }

        /// <summary>
        /// Update all the sprites in the collision tree
        /// </summary>
        public static void UpdateSpriteTree()
        {
            KdTreeNode<double, ISprite>[] Sprites = SpritesTree.ToArray();
            if (Count != 0)
                SpritesTree.Clear();

            foreach (KdTreeNode<double, ISprite> s in Sprites)
                SpritesTree.Add(new[] { s.Value.XPosition, s.Value.XPosition }, s.Value);

            if (Count != 0)
                SpritesTree.Balance();
        }

        public static void KillSprites(object sender, EventArgs e)
        {
            foreach (ISprite s in SpriteList.ToArray())
                s.Kill();
        }

        private static void UpdateCollisionTree(ISprite s, double[] oldPos, bool remove)
        {
            SpritesTree.RemoveSpecific(oldPos, s);

            if (!remove)
            { SpritesTree.Add(new[] { s.XPosition, s.YPosition }, s); }
            else
            { Count--; SpriteList.Remove(s); SpriteLastDepawned = s; }

            //if (Count != 0)
                //SpritesTree.Balance();
        }

        /// <summary>
        /// Adds the sprite to the processing list
        /// </summary>
        /// <param name="sprites"></param>
        public static void AddSprites(params ISprite[] sprites)
        {
            foreach (ISprite s in sprites)
            {
                if (SpriteList == null)
                    SpriteList = new List<ISprite>();
                if (s.Data.InteractWithSprites)
                    SpritesTree.Add(new[] { s.XPosition, s.YPosition }, s);
                SpriteList.Add(s);
                Count++;
                SpriteLastSpawned = s;
            }
        }
    }

    public enum SpriteID
    {
        GreenShellessKoopa,
        Test
    }
    public class SpriteSpawner
    {
        public static ISprite SpawnSprite(double[] point, SpriteData d, params object[] args)
        {
            return SpawnSprite(point[0], point[1], d, args);
        }
        public static ISprite SpawnSprite(double x, double y, SpriteData d, params object[] args)
        {
            switch (d.ID)
            {
                case SpriteID.GreenShellessKoopa:
                    return new ShellessKoopa(x, y, d, 0);
                case SpriteID.Test:
                    d.InteractWithSprites = false;
                    return new TestSprite(x, y, d);
                default:
                    return null;
            }
        }
    }

    public class UnlistedSpriteException : Exception
    {
        public UnlistedSpriteException(string message) : base(message)
        {
        }
    }
    public class UnindexedSpriteException : Exception
    {
        public UnindexedSpriteException(string message) : base(message)
        {
        }
    }
}
