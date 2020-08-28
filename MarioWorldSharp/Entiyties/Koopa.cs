using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MarioWorldSharp.Levels;

namespace MarioWorldSharp.Entities
{
    public class ShellessKoopa : Entity
    {
        private readonly double NORMAL_SPEED = 8.0 / 16.0;
        private readonly double YELLOW_SPEED = 12.0 / 16.0;
        public KoopaType KoopaType { get; }
        private bool FacingLeft
        {
            get
            {
                return FacingAngle > 90.0 && FacingAngle < 270.0;
            }
        }
        private Texture2D Box;

        public ShellessKoopa(double x, double y, EntityData d, int type) : base(x, y, d)
        {
            if (this.disposedValue)
                return;

            this.KoopaType = (KoopaType)type;
            collisionBox = new Rectangle((int)x, (int)y, 16, 16);
            this.VertGravity = 1;
            if (SMW.Character?.XPosition - XPosition < 0)
            { FacingAngle = 180.0; }
        }

        public override void Process()
        {
            switch (KoopaType)
            {
                case KoopaType.Yellow:
                    switch (this.FacingLeft)
                    {
                        case true:
                            XSpeed = -YELLOW_SPEED;
                            break;
                        case false:
                            XSpeed = YELLOW_SPEED;
                            break;
                    }
                    break;

                default:
                    switch (this.FacingLeft)
                    {
                        case true:
                            XSpeed = -NORMAL_SPEED;
                            break;
                        case false:
                            XSpeed = NORMAL_SPEED;
                            break;
                    }
                    break;
            }

            UpdateXPosition();
            UpdateYPosition();
            SpriteCollision();
            EnvironmentCollision();
            PlayerCollision();
            OffScreen();
        }

        protected override void SpriteCollision()
        {
            IEntity[] collidingWith = GetCollidedSprites();

            int[] indecies = new int[]
            { 3,5,7 };
            if (FacingLeft)
                for (int i = 0; i < 3; i++)
                    indecies[i]--;

            foreach (int i in indecies)
            {
                if (collidingWith[i] == null)
                    continue;

                if (collidingWith[i].Data.CollideTurnaround)
                {
                    if (collidingWith[i].FacingAngle != this.FacingAngle)
                        collidingWith[i].FacingAngle += 180.0;

                    this.FacingAngle += 180.0;
                    return;
                }
            }
        }

        protected override void EnvironmentCollision()
        {
            base.EnvironmentCollision();
            if (BlockedLeft)
                FacingAngle = 0.0;
            if (BlockedRight)
                FacingAngle = 180.0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Box == null)
            {
                var outlne = new Color(45, 255, 45, 100);
                var fill = new Color(0, 64, 0, 100);
                Rectangle rect = new Rectangle((int)(SMW.Level.X - XPosition), (int)(SMW.Level.Y - YPosition), 16, 16);
                Color[] colColor = new Color[rect.Width * rect.Height];
                for (int i = 0; i < rect.Width; i++)
                {
                    for (int j = 0; j < rect.Height; j++)
                    {
                        if (i == 0 || j == 0 || i == rect.Width - 1 || j == rect.Height - 1)
                            colColor[(j * rect.Width) + i] = outlne;
                        else
                            colColor[(j * rect.Width) + i] = fill;
                    }
                }

                colColor[rect.Width * 7 + 14] = outlne;
                colColor[rect.Width * 7 + 13] = outlne;
                colColor[rect.Width * 7 + 12] = outlne;
                colColor[rect.Width * 8 + 14] = outlne;
                colColor[rect.Width * 8 + 13] = outlne;
                colColor[rect.Width * 8 + 12] = outlne;
                Box = new Texture2D(spriteBatch.GraphicsDevice, rect.Width, rect.Height);
                Box.SetData(colColor);
            }

            spriteBatch.Draw(Box,
                new Rectangle((int)XPosition - (int)SMW.Level.X, (int)YPosition - (int)SMW.Level.Y, Box.Width, Box.Height),
                new Rectangle(0, 0, Box.Width, Box.Height),
                Color.White, 0.0F, Vector2.Zero, FacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1F);
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
