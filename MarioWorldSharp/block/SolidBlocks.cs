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
using MonoGame.Utilities;

namespace MarioWorldSharp.block
{
    public class SolidBlock : Block
    {
        public override void Above(Player Mario, double x, double y)
        {
            if (Mario.GetYSpeed() < 0)
            {
                Mario.SetYSpeed(0);
                Mario.SetYAccel(0);
            }
            Mario.SetBlockedAbove(true);
            Mario.SetY(Mario.GetY() + 16.0 - (Mario.GetY() % 16.0));
        }

        public override void Above(Sprite sprite, double x, double y)
        {
        }

        public override void Bellow(Player Mario, double x, double y)
        {
            if (Mario.GetYSpeed() > 0)
            {
                Mario.SetYSpeed(0);
                Mario.SetYAccel(0);
            }
            Mario.SetBlockedBellow(true);
            Mario.SetY(Mario.GetY() - (Mario.GetY() % 16.0));
        }

        public override void Bellow(Sprite sprite, double x, double y)
        {
        }

        public override void Left(Player Mario, double x, double y)
        {
            if (Mario.GetXSpeed() < 0)
            {
                Mario.SetXSpeed(0);
                Mario.SetXAccel(0);
            }
            Mario.SetBlockedLeft(true);
            Mario.SetX(Mario.GetX() + 16.0 - (Mario.GetX() % 16.0));
        }

        public override void Left(Sprite sprite, double x, double y)
        {
        }

        public override void Right(Player Mario, double x, double y)
        {
            if (Mario.GetXSpeed() > 0)
            {
                Mario.SetXSpeed(0);
                Mario.SetXAccel(0);
            }
            Mario.SetBlockedRight(true);
            Mario.SetX(Mario.GetX() - (Mario.GetX() % 16.0));
        }

        public override void Right(Sprite sprite, double x, double y)
        {
        }

        public override void BodyInside(Player Mario, double x, double y)
        {
        }

        public override void BodyInside(Sprite sprite, double x, double y)
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

        public override void TopCorner(Sprite sprite, double x, double y)
        {
        }

        public override void WallRun(Player Mario, double x, double y)
        {
        }
    }

    public class Ledge : SolidBlock
    {
        public override void Above(Player Mario, double x, double y)
        {
        }
        public override void Left(Player Mario, double x, double y)
        {
        }
        public override void Right(Player Mario, double x, double y)
        {
        }
    }

    public class PipeTopLeft : SolidBlock
    {
    }
    public class PipeTopRight : SolidBlock
    {
    }
    public class PipeLeft : SolidBlock
    {
    }
    public class PipeRight : SolidBlock
    {
    }

    public class PassThroughBlock : Block
    {
        public override void Above(Player Mario, double x, double y)
        {
        }

        public override void Above(Sprite sprite, double x, double y)
        {
        }

        public override void Bellow(Player Mario, double x, double y)
        {
        }

        public override void Bellow(Sprite sprite, double x, double y)
        {
        }

        public override void Left(Player Mario, double x, double y)
        {
        }

        public override void Left(Sprite sprite, double x, double y)
        {
        }

        public override void Right(Player Mario, double x, double y)
        {
        }

        public override void Right(Sprite sprite, double x, double y)
        {
        }

        public override void BodyInside(Player Mario, double x, double y)
        {
        }

        public override void BodyInside(Sprite sprite, double x, double y)
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

        public override void TopCorner(Sprite sprite, double x, double y)
        {
        }

        public override void WallRun(Player Mario, double x, double y)
        {
        }
    }
}
