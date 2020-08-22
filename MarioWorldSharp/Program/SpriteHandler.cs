using KdTree;
using KdTree.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MarioWorldSharp.Level;

namespace MarioWorldSharp.Sprite
{
    public class SpawnArgs : EventArgs
    {
        public ISprite[] SpawnArr;
    }
    public class SpriteHandler
    {
        private static KdTree<double, ISprite> SpritesTree = new KdTree<double, ISprite>(2, new DoubleMath());
        private static int Count;

        public static void ProcessSprites()
        {
            SMW.Level.SpawnSpritesOnScroll();
            foreach (ISprite s in ToArray())
                s?.Process();
        }

        public static KdTree<double, ISprite> GetSpriteTree()
        {
            return SpritesTree;
        }

        public static ISprite[] ToArray()
        {
            if (Count < 0)
                return new ISprite[] { null };
            ISprite[] sprites = new ISprite[Count];

            int i = 0;
            foreach (KdTreeNode<double, ISprite> n in SpritesTree)
            {
                if (i >= Count)
                    break;

                sprites[i] = n.Value;
                i++;
            }
            return sprites;
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
        /// Update all the sprites in the sprite tree
        /// </summary>
        public static void UpdateSpriteTree()
        {
            KdTreeNode<double, ISprite>[] Sprites = SpritesTree.ToArray();
            if (Count != 0)
                SpritesTree.Clear();

            foreach (KdTreeNode<double, ISprite> s in Sprites)
                SpritesTree.Add(new[] { s.Value.XPosition, s.Value.XPosition }, s.Value);
            try
            {
                SpritesTree.Balance();
            } 
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Updates a sprite's position in the sprite tree
        /// </summary>
        /// <param name="s">Sprite that should be updated</param>
        /// <param name="remove">Set to true if you want to remove s from the sprite tree</param>
        public static void UpdateSpriteTree(ISprite s, bool remove)
        {
            double[] oldPos = SpritesTree.FindValue(s);
            if (oldPos != null)
                SpritesTree.RemoveAt(oldPos);

            if (!remove)
                SpritesTree.Add(new[] { s.XPosition, s.YPosition }, s);
            else
                Count--;

            try
            {
                SpritesTree.Balance();
            }
            catch (Exception e)
            {

            }
        }

        public static void AddSprites(params ISprite[] sprites)
        {
            foreach (ISprite s in sprites)
            {
                UpdateSpriteTree(s, false);
                Count++;
            }
        }
    }
}
