using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using static MarioWorldSharp.Level;

namespace MarioWorldSharp.Sprite
{
    public class ShellessKoopa : Sprite
    {
        private readonly double NORMAL_SPEED = 8.0 / 16.0;
        private readonly double YELLOW_SPEED = 10.0 / 16.0;
        public KoopaType KoopaType { get; }
        private bool facingLeft;
        private Texture2D Box;

        public ShellessKoopa(double x, double y, SpriteData d, int type) : base(x, y, d)
        {
            this.KoopaType = (KoopaType)type;
            collisionBox = new Rectangle((int)x, (int)y, 16, 16);
            this.VertGravity = 1;
            facingLeft = SMW.Character?.XPosition - XPosition < 0;
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

            UpdateXYPosition();
            EnvironmentCollision();
            OffScreen(32, true);
        }

        protected override void EnvironmentCollision()
        {
            base.EnvironmentCollision();
            if (BlockedLeft)
                facingLeft = false;
            if (BlockedRight)
                facingLeft = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Box == null)
            {
                Rectangle rect = new Rectangle((int)(SMW.Level.X - XPosition), (int)(SMW.Level.Y - YPosition), 16, 16);
                Color[] colColor = new Color[rect.Width * rect.Height];
                for (int i = 0; i < rect.Width; i++)
                {
                    for (int j = 0; j < rect.Height; j++)
                    {
                        if (i == 0 || j == 0 || i == rect.Width - 1 || j == rect.Height - 1)
                            colColor[(j * rect.Width) + i] = new Color(255, 45, 45, 100);
                        else
                            colColor[(j * rect.Width) + i] = new Color(64, 0, 0, 100);
                    }
                }
                Box = new Texture2D(spriteBatch.GraphicsDevice, rect.Width, rect.Height);
                Box.SetData(colColor);
            }

            spriteBatch.Draw(Box,
                new Rectangle((int)XPosition - (int)SMW.Level.X, (int)YPosition - (int)SMW.Level.Y, Box.Width, Box.Height),
                new Rectangle(0, 0, Box.Width, Box.Height),
                Color.White, 0.0F, Vector2.Zero, SpriteEffects.None, 1F);
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
