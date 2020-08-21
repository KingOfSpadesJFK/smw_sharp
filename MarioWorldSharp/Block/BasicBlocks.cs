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
    public class SolidBlock : AbstractBlock
    {
        public override void Above(Player Mario, double x, double y)
        {
            if (Mario.BlockedAbove)
                return;

            if (Mario.YSpeed < 0)
            {
                Mario.YSpeed = 0;
            }
            Mario.BlockedAbove = true;
            Mario.YPosition = Mario.YPosition + 16.0 - (Mario.YPosition % 16.0);
        }

        public override void Above(ISprite Sprite, double x, double y)
        {
            if (Sprite.BlockedAbove)
                return;

            if (Sprite.YSpeed < 0)
            {
                Sprite.YSpeed = 0;
            }
            Sprite.BlockedAbove = true;
            Sprite.YPosition = Sprite.YPosition + 16.0 - (Sprite.YPosition % 16.0);
        }

        public override void Bellow(Player Mario, double x, double y)
        {
            if (Mario.BlockedBellow)
                return;

            if (Mario.YSpeed > 0)
            {
                Mario.YSpeed = 0;
            }
            Mario.BlockedBellow = true;
            Mario.YPosition = Mario.YPosition - (Mario.YPosition % 16.0);
        }

        public override void Bellow(ISprite Sprite, double x, double y)
        {
            if (Sprite.BlockedBellow)
                return;

            if (Sprite.YSpeed > 0)
            {
                Sprite.YSpeed = 0;
            }
            Sprite.BlockedBellow = true;
            Sprite.YPosition = Sprite.YPosition - (Sprite.YPosition % 16.0);
        }

        public override void Left(Player Mario, double x, double y)
        {
            if (Mario.BlockedLeft)
                return;

            if (Mario.XSpeed < 0)
            {
                Mario.XSpeed = 0;
            }
            Mario.BlockedLeft = true;
            Mario.XPosition = Mario.XPosition + 16.0 - (Mario.XPosition % 16.0) - Player.HorizCollisionOffset;
        }

        public override void Left(ISprite Sprite, double x, double y)
        {
            if (Sprite.BlockedLeft)
                return;

            if (Sprite.XSpeed < 0)
            {
                Sprite.XSpeed = 0;
            }
            Sprite.BlockedLeft = true;
            Sprite.XPosition = Sprite.XPosition + 16.0 - (Sprite.XPosition % 16.0) - Player.HorizCollisionOffset;
        }

        public override void Right(Player Mario, double x, double y)
        {
            if (Mario.BlockedRight)
                return;

            if (Mario.XSpeed > 0)
            {
                Mario.XSpeed = 0;
            }
            Mario.BlockedRight = true;
            Mario.XPosition = Mario.XPosition - (Mario.XPosition % 16.0) + Player.HorizCollisionOffset;
        }

        public override void Right(ISprite Sprite, double x, double y)
        {
            if (Sprite.BlockedRight)
                return;

            if (Sprite.XSpeed > 0)
            {
                Sprite.XSpeed = 0;
            }
            Sprite.BlockedRight = true;
            Sprite.XPosition = Sprite.XPosition - (Sprite.XPosition % 16.0) + Player.HorizCollisionOffset;
        }

        public override void BodyInside(Player Mario, double x, double y)
        {
        }

        public override void BodyInside(ISprite Sprite, double x, double y)
        {
        }

        public override void Cape(double x, double y)
        {
        }

        public override void Fireball(double x, double y)
        {
        }

        public override void HeadInside(Player Mario, double x, double y)
        {
        }

        public override void TopCorner(Player Mario, double x, double y)
        {
        }

        public override void TopCorner(ISprite Sprite, double x, double y)
        {
        }

        public override void WallRun(Player Mario, double x, double y)
        {
        }
    }

    public class PassThroughBlock : AbstractBlock
    {
        public override void Above(Player Mario, double x, double y)
        {
        }

        public override void Above(ISprite Sprite, double x, double y)
        {
        }

        public override void Bellow(Player Mario, double x, double y)
        {
        }

        public override void Bellow(ISprite Sprite, double x, double y)
        {
        }

        public override void Left(Player Mario, double x, double y)
        {
        }

        public override void Left(ISprite Sprite, double x, double y)
        {
        }

        public override void Right(Player Mario, double x, double y)
        {
        }

        public override void Right(ISprite Sprite, double x, double y)
        {
        }

        public override void BodyInside(Player Mario, double x, double y)
        {
        }

        public override void BodyInside(ISprite Sprite, double x, double y)
        {
        }

        public override void Cape(double x, double y)
        {
        }

        public override void Fireball(double x, double y)
        {
        }

        public override void HeadInside(Player Mario, double x, double y)
        {
        }

        public override void TopCorner(Player Mario, double x, double y)
        {
        }

        public override void TopCorner(ISprite Sprite, double x, double y)
        {
        }

        public override void WallRun(Player Mario, double x, double y)
        {
        }
    }
}
