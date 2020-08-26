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

namespace MarioWorldSharp.Blocks
{
    public abstract class Block
    {
        public virtual void Above(Player p, double x, double y) {}
        public virtual void Bellow(Player p, double x, double y) {}
        public virtual void Left(Player p, double x, double y) {}
        public virtual void Right(Player p, double x, double y) {}
        public virtual void TopCorner(Player p, double x, double y) {}
        public virtual void BodyInside(Player p, double x, double y) {}
        public virtual void HeadInside(Player p, double x, double y) {}
        public virtual void WallRun(Player p, double x, double y) {}
        public virtual void Cape(double x, double y) {}
        public virtual void Fireball(double x, double y) {}

        public virtual void Above(ISprite s, double x, double y) {}
        public virtual void Bellow(ISprite s, double x, double y) {}
        public virtual void Left(ISprite s, double x, double y) {}
        public virtual void Right(ISprite s, double x, double y) {}
        public virtual void TopCorner(ISprite s, double x, double y) {}
        public virtual void BodyInside(ISprite s, double x, double y) {}
    }
}
