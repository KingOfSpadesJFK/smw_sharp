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
        public override void Above(Player p, double x, double y)
        {
            if (p.BlockedAbove)
                return;

            if (p.YSpeed < 0)
            {
                p.YSpeed = 0;
                p.BlockedAbove = true;
                p.YPosition = p.YPosition + 16.0 - (p.YPosition % 16.0);
            }
        }

        public override void Above(ISprite s, double x, double y)
        {
            if (s.BlockedAbove)
                return;

            if (s.YSpeed < 0)
            {
                s.YSpeed = 0;
                s.BlockedAbove = true;
                s.YPosition = s.YPosition + 16.0 - (s.YPosition % 16.0);
            }
        }

        public override void Bellow(Player p, double x, double y)
        {
            if (p.BlockedBellow)
                return;

            if (p.YSpeed > 0)
            {
                p.YSpeed = 0;
                p.BlockedBellow = true;
                p.YPosition -= (p.YPosition % 16.0);
            }
        }

        public override void Bellow(ISprite s, double x, double y)
        {
            if (s.BlockedBellow)
                return;

            if (s.YSpeed > 0)
            {
                s.YSpeed = 0;
                s.BlockedBellow = true;
                s.YPosition -= (s.YPosition % 16.0);
            }
        }

        public override void Left(Player p, double x, double y)
        {
            if (p.BlockedLeft)
                return;

            if (p.XSpeed < 0)
            {
                p.XSpeed = 0;
                p.BlockedLeft = true;
                p.XPosition = p.XPosition + 16.0 - (p.XPosition % 16.0) - Player.SideHorizCollisionOffset;
            }
        }

        public override void Left(ISprite s, double x, double y)
        {
            if (s.BlockedLeft)
                return;

            if (s.XSpeed < 0)
            {
                s.XSpeed = 0;
                s.BlockedLeft = true;
                s.XPosition = s.XPosition + 16.0 - (s.XPosition % 16.0);
            }
        }

        public override void Right(Player p, double x, double y)
        {
            if (p.BlockedRight)
                return;

            if (p.XSpeed > 0)
            {
                p.XSpeed = 0;
                p.BlockedRight = true;
                p.XPosition = p.XPosition - (p.XPosition % 16.0) + Player.SideHorizCollisionOffset;
            }
        }

        public override void Right(ISprite s, double x, double y)
        {
            if (s.BlockedRight)
                return;

            if (s.XSpeed > 0)
            {
                s.XSpeed = 0;
                s.BlockedRight = true;
                s.XPosition -= (s.XPosition % 16.0);
            }
        }
    }

    public class PassThroughBlock : AbstractBlock
    {
    }
}
