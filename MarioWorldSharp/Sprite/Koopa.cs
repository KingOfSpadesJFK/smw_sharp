using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarioWorldSharp.Sprite
{
    public class ShellessKoopa : Sprite
    {
        private readonly double NORMAL_SPEED = 8.0;
        private readonly double YELLOW_SPEED = 10.0;
        public KoopaType KoopaType { get; }
        private bool facingLeft;
        public ShellessKoopa(double x, double y, int i, KoopaType type) : base(x, y, i)
        {
            this.KoopaType = type;
            _collisionBox = new Rectangle((int)x, (int)y, 16, 16);
            facingLeft = MarioWorld.Mario.XPosition < 0;
        }
        public override void Process()
        {
            switch (KoopaType)
            {
                case KoopaType.Yellow:
                    if (facingLeft)
                        XSpeed = -YELLOW_SPEED;
                    else
                        XSpeed = YELLOW_SPEED;
                    break;

                default:
                    if (facingLeft)
                        XSpeed = -NORMAL_SPEED;
                    else
                        XSpeed = NORMAL_SPEED;
                    break;
            }

            UpdateXPositionition();
            UpdateYPositionition();
        }
    }

    public enum KoopaType : byte
    {
        Green = 0,
        Red = 1,
        Blue = 2,
        Yellow = 3
    }
}
