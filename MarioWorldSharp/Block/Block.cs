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

using MarioWorldSharp.Sprite;

namespace MarioWorldSharp.Block
{
    public abstract class AbstractBlock
    {
        public abstract void Above(Player Mario, double x, double y);
        public abstract void Bellow(Player Mario, double x, double y);
        public abstract void Left(Player Mario, double x, double y);
        public abstract void Right(Player Mario, double x, double y);
        public abstract void TopCorner(Player Mario, double x, double y);
        public abstract void BodyInside(Player Mario, double x, double y);
        public abstract void HeadInside(Player Mario, double x, double y);
        public abstract void WallRun(Player Mario, double x, double y);
        public abstract void Cape(double x, double y);
        public abstract void Fireball(double x, double y);

        public abstract void Above(ISprite sprite, double x, double y);
        public abstract void Bellow(ISprite sprite, double x, double y);
        public abstract void Left(ISprite sprite, double x, double y);
        public abstract void Right(ISprite sprite, double x, double y);
        public abstract void TopCorner(ISprite sprite, double x, double y);
        public abstract void BodyInside(ISprite sprite, double x, double y);
    }
}
