using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using MarioWorldSharp.Levels;

namespace MarioWorldSharp.Sprite
{
    public enum SpriteStatus
    {
        NonExistent = 0,
        Init = 1,               //Will probably won't be used. Left over from original SMW
        FallOffScreen = 2,
        Smushed = 3,
        SpinKill = 4,
        Sink = 5,
        LevelEndCoin = 6,
        YoshiMouth = 7,
        Normal = 8,
        Stationary = 9,
        Kicked = 10,
        Carried = 11,
        LevelEndPower = 12
    }

    public interface ISprite
    {
        public double XPosition { get; set; }
        public double YPosition { get; set; }
        public double XSpeed { get; set; }
        public double YSpeed { get; set; }
        public double FacingAngle { get; set; }
        public byte VertGravity { get; set; }
        public byte HorizGravity { get; set; }
        public bool BlockedBellow { get; set; }
        public bool BlockedAbove { get; set; }
        public bool BlockedLeft { get; set; }
        public bool BlockedRight { get; set; }
        public SpriteStatus Status { get; set; }
        public SpriteData Data { get; }
        public void Process();
        public void Draw(SpriteBatch spriteBatch);
        public Rectangle GetCollisionBox();
        public void Kill();
    }

    /// <summary>
    /// A class containing the properties of a sprite, but not the sprite itself.
    /// This should be used when generating a sprite.
    /// </summary>
    public class SpriteData
    {
        public SpriteID ID;
        public object[] Args;
        public bool DisposeOffscreen = true;
        public int DespawnThresh = 16;
        public int Index = -1;
        public bool Spawned = false;
        public bool InteractWithSprites = true;
        public bool CollideTurnaround = true;

        public override string ToString()
        {
            return $"{ID.ToString()}, #{Index}";
        }
    }
    public abstract class Sprite : IDisposable, ISprite
    {
        private double _xPos;
        private double _yPos;
        private double _angle;
        protected Rectangle collisionBox;
        public double XPosition
        {
            get => _xPos;
            set
            {
                _xPos = value;
                if (collisionBox != null)
                    collisionBox.X = (int)value;
            }
        }
        public double YPosition
        {
            get => _yPos;
            set
            {
                _yPos = value;
                if (collisionBox != null)
                    collisionBox.Y = (int)value;
            }
        }
        public double XSpeed { get; set; }
        public double YSpeed { get; set; }
        public double FacingAngle
        {
            get => _angle;
            set
            {
                _angle = value % 360.0;
            }
        }
        public byte VertGravity { get; set; }
        public byte HorizGravity { get; set; }
        public bool BlockedBellow { get; set; }
        public bool BlockedAbove { get; set; }
        public bool BlockedLeft { get; set; }
        public bool BlockedRight { get; set; }
        public SpriteStatus Status { get; set; }
        public SpriteData Data { get; set; }

        public Sprite(double x, double y, SpriteData d)
        {
            this.XPosition = x; this.YPosition = y;
            if (d.DisposeOffscreen)
            {
                OffScreen(d.DespawnThresh, d.DisposeOffscreen);
                if (this.disposedValue)
                    return;
            }
            d.Spawned = true;
            this.collisionBox = Rectangle.Empty;
            this.Status = SpriteStatus.Normal;
            this.Data = d;
            SpriteHandler.AddSprites(this);
        }

        public abstract void Process();

        public Rectangle GetCollisionBox()
        {
            return collisionBox;
        }

        public virtual void Kill()
        {
            if (Data.Index != -1)
                SMW.Level.RemoveSprite(Data);
            this.Dispose();
        }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        #region Various Methods
        protected virtual void UpdateXPosition()
        {
            double gravity = HorizGravity * .375;
            if (XSpeed < 64.0 / 16.0)
                XSpeed += gravity;
            XPosition += XSpeed;
        }

        protected virtual void UpdateYPosition()
        {
            double gravity = VertGravity * .375;
            if (YSpeed < 64.0 / 16.0)
                YSpeed += gravity;
            YPosition += YSpeed;
        }

        private static readonly int SideVertColisionOffset = 5;
        private static readonly int SideHorizCollisionOffset = 3;
        private static readonly int TopBotHorizCollisionOffset = 8;
        protected virtual void EnvironmentCollision()
        {
            BlockedAbove = false;
            BlockedBellow = false;
            BlockedLeft = false;
            BlockedRight = false;

            //Check left collision
            SMW.Level.GetMap16FromPosition(collisionBox.Left + SideHorizCollisionOffset, collisionBox.Top + TopBotHorizCollisionOffset).Left(this, collisionBox.Left + SideHorizCollisionOffset, collisionBox.Top + TopBotHorizCollisionOffset);
            SMW.Level.GetMap16FromPosition(collisionBox.Left + SideHorizCollisionOffset, collisionBox.Bottom - TopBotHorizCollisionOffset).Left(this, collisionBox.Left + SideHorizCollisionOffset, collisionBox.Bottom - TopBotHorizCollisionOffset);

            //Check right collision
            SMW.Level.GetMap16FromPosition(collisionBox.Right - SideHorizCollisionOffset, collisionBox.Top + TopBotHorizCollisionOffset).Right(this, collisionBox.Right - SideHorizCollisionOffset, collisionBox.Top + TopBotHorizCollisionOffset);
            SMW.Level.GetMap16FromPosition(collisionBox.Right - SideHorizCollisionOffset, collisionBox.Bottom - TopBotHorizCollisionOffset).Right(this, collisionBox.Right - SideHorizCollisionOffset, collisionBox.Bottom - TopBotHorizCollisionOffset);

            //Check bottom collision
            SMW.Level.GetMap16FromPosition(collisionBox.Left + SideVertColisionOffset, collisionBox.Bottom).Bellow(this, collisionBox.Left + SideVertColisionOffset, collisionBox.Bottom);
            SMW.Level.GetMap16FromPosition(collisionBox.Right - SideVertColisionOffset, collisionBox.Bottom).Bellow(this, collisionBox.Right - SideVertColisionOffset, collisionBox.Bottom);

            //Check top collision
            SMW.Level.GetMap16FromPosition(collisionBox.Left + SideVertColisionOffset, collisionBox.Top).Above(this, collisionBox.Left + SideVertColisionOffset, collisionBox.Top);
            SMW.Level.GetMap16FromPosition(collisionBox.Right - SideVertColisionOffset, collisionBox.Top).Above(this, collisionBox.Right - SideVertColisionOffset, collisionBox.Top);
        }

        protected virtual void PlayerCollision()
        {
            bool[] collidedSides = GetSidesOfPlayerCollision();
            var p = SMW.Character;
            if (collidedSides[0])
            {
                if (p.GetCollisionBox().Bottom < this.collisionBox.Bottom - 4)
                {
                    if (!p.SpinJumping)
                        p.YSpeed = (-80.0 - (640.0 * Math.Abs(p.XSpeed * 1.5) / 256.0)) / 16.0;
                    else
                        p.YSpeed = 0.0;
                    this.Kill();
                    return;
                }
            }
        }

        protected virtual bool[] GetSidesOfPlayerCollision()
        {
            bool[] collidedSides = new bool[8];
            var p = SMW.Character;

            collidedSides[0] = p.GetCollisionBox().Contains(this.collisionBox.Left + this.collisionBox.Width / 2.0F, this.collisionBox.Top);
            collidedSides[1] = p.GetCollisionBox().Contains(this.collisionBox.Left + this.collisionBox.Width / 2.0F, this.collisionBox.Bottom);
            collidedSides[2] = p.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Top + this.collisionBox.Height / 2.0F);
            collidedSides[3] = p.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Top + this.collisionBox.Height / 2.0F);

            collidedSides[4] = p.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Top);
            collidedSides[5] = p.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Top);
            collidedSides[6] = p.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Bottom);
            collidedSides[7] = p.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Bottom);

            return collidedSides;
        }

        protected virtual void SpriteCollision()
        {
        }

        protected bool IsCollidingWithSprites(out ISprite[] nearestNeighbors)
        {
            nearestNeighbors = SpriteHandler.GetNearestNeighbors(new[] { XPosition, YPosition }, 10);
            foreach(ISprite s in nearestNeighbors)
            {
                if (IsCollidingWithSprite(s))
                    return true;
            }

            return false;
        }

        protected bool IsCollidingWithSprite(ISprite s, out SpriteCollisionSide coll)
        {
            coll = 0;
            //Ignore if s is this sprite
            if (Object.ReferenceEquals(this, s))
                return false;

            //Top = 0
            if (s.GetCollisionBox().Contains(this.collisionBox.Left + this.collisionBox.Width / 2.0F, this.collisionBox.Top))
            { coll = SpriteCollisionSide.Top; return true; }
            //Bottom = 1
            if (s.GetCollisionBox().Contains(this.collisionBox.Left + this.collisionBox.Width / 2.0F, this.collisionBox.Bottom))
            { coll = SpriteCollisionSide.Bottom; return true; }
            //Left = 2
            if (s.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Top + this.collisionBox.Height / 2.0F))
            { coll = SpriteCollisionSide.Left; return true; }
            //Right = 3
            if (s.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Top + this.collisionBox.Height / 2.0F))
            { coll = SpriteCollisionSide.Right; return true; }

            //Top Left = 4
            if (s.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Top))
            { coll = SpriteCollisionSide.TopLeft; return true; }
            //Top Right = 5
            if (s.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Top))
            { coll = SpriteCollisionSide.TopRight; return true; }
            //Bottom Left = 6
            if (s.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Bottom))
            { coll = SpriteCollisionSide.BottomLeft; return true; }
            //Bottom Right = 7
            if (s.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Bottom))
            { coll = SpriteCollisionSide.BottomRight; return true; }

            return false;
        }

        public enum SpriteCollisionSide : byte
        {
            Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight
        }

        protected bool IsCollidingWithSprite(ISprite s)
        {
            return IsCollidingWithSprite(s, out _);
        }

        protected ISprite[] GetCollidedSprites()
        {
            ISprite[] sprites = new ISprite[8];

            ISprite[] nearestNeighbors = SpriteHandler.GetNearestNeighbors(new[] { XPosition, YPosition }, 10);
            foreach (ISprite s in nearestNeighbors)
            {
                //Ignore if s is this sprite
                if (Object.ReferenceEquals(this, s))
                    continue;

                //Top
                if (s.GetCollisionBox().Contains(this.collisionBox.Left + this.collisionBox.Width / 2.0F, this.collisionBox.Top))
                    sprites[0] = s;
                //Bottom
                if (s.GetCollisionBox().Contains(this.collisionBox.Left + this.collisionBox.Width / 2.0F, this.collisionBox.Bottom))
                    sprites[1] = s;
                //Left
                if (s.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Top + this.collisionBox.Height / 2.0F))
                    sprites[2] = s;
                //Right
                if (s.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Top + this.collisionBox.Height / 2.0F))
                    sprites[3] = s;

                //Top Left
                if (s.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Top))
                    sprites[4] = s;
                //Top Right
                if (s.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Top))
                    sprites[5] = s;
                //Bottom Left
                if (s.GetCollisionBox().Contains(this.collisionBox.Left, this.collisionBox.Bottom))
                    sprites[6] = s;
                //Bottom Right
                if (s.GetCollisionBox().Contains(this.collisionBox.Right, this.collisionBox.Bottom))
                    sprites[7] = s;
            }
            return sprites;
        }

        protected virtual void OffScreen()
        {
            OffScreen(Data.DespawnThresh, Data.DisposeOffscreen);
        }

        protected bool[] OffScreen(double borderSize, bool dispose)
        {
            //TODO: Implement
            bool[] off =  new[]
            {
                XPosition < SMW.Level.X - borderSize,
                XPosition > SMW.Level.X + 400 + borderSize,
                YPosition < SMW.Level.Y - borderSize,
                YPosition > SMW.Level.Y + 224 + borderSize
            };

            if (!dispose)
                return off;

            foreach (bool b in off)
                if (b)
                    this.Dispose();

            return off;
        }
        #endregion

        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (this.Status != SpriteStatus.NonExistent)
                    {
                        this.Status = SpriteStatus.NonExistent;
                        this.Data.Spawned = false;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BlankSprite() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion

        public override string ToString()
        {
            return $"{Data}";
        }

    }
    public class TestSprite : Sprite
    {
        private Texture2D Box;
        private readonly Random rand = new Random();
        private double speed;
        private bool facingLeft;

        public TestSprite(double x, double y, SpriteData d) : base(x, y, d)
        {
            if (this.disposedValue)
                return;
            this.collisionBox = new Rectangle((int)x, (int)y, 16, 16);
            if (this.XPosition % 64 == 0 && d.Index != -1)
            {
                SpriteSpawner.SpawnSprite(this.XPosition, this.YPosition, new SpriteData { ID = SpriteID.GreenShellessKoopa }).YSpeed = -80.0 / 16.0;
            }

            speed = 2.0 + rand.NextDouble() * 2.0;
            facingLeft = SMW.Character.XPosition < this.XPosition;
        }

        public override void Process()
        {
            Float();
            UpdateYPosition();
            UpdateXPosition();
            OffScreen();
        }

        private void Bounce()
        {
            this.VertGravity = 1;
            if (this.BlockedBellow)
            {
                this.YSpeed = rand.NextDouble();
                this.YSpeed *= this.YSpeed * -2.5;
                this.YSpeed -= 6.0;
                facingLeft = SMW.Character.XPosition < this.XPosition;
            }

            if (facingLeft)
                this.XSpeed = speed * -1.0;
            else
                this.XSpeed = speed * 1.0;

            EnvironmentCollision();
        }

        private void Float()
        {
            Player p = SMW.Character;
            this.VertGravity = 0;
            double[] dis = new[]
            {
                p.XPosition - this.XPosition,
                p.YPosition - this.YPosition,
                0.0
            };

            if (dis[0] < 0)
                this.XSpeed = Math.Clamp(this.XSpeed - (2.0 / 16.0), -speed, speed);
            else 
                this.XSpeed = Math.Clamp(this.XSpeed + (2.0 / 16.0), -speed, speed);

            if (dis[1] < 0)
                this.YSpeed = Math.Clamp(this.YSpeed - (2.0 / 16.0), -speed, speed);
            else
                this.YSpeed = Math.Clamp(this.YSpeed + (2.0 / 16.0), -speed, speed);
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
            if (this.Box == null)
            {
                Rectangle rect = new Rectangle((int)(SMW.Level.X - this.XPosition), (int)(SMW.Level.Y - this.YPosition), 16, 16);
                Color[] colColor = new Color[rect.Width * rect.Height];
                for (int i = 0; i < rect.Width; i++)
                {
                    for (int j = 0; j < rect.Height; j++)
                    {
                        if (i == 0 || j == 0 || i == rect.Width - 1 || j == rect.Height - 1)
                            colColor[(j * rect.Width) + i] = new Color(225, 225, 225, 100);
                        else
                            colColor[(j * rect.Width) + i] = new Color(64, 64, 64, 100);
                    }
                }
                this.Box = new Texture2D(spriteBatch.GraphicsDevice, rect.Width, rect.Height);
                this.Box.SetData(colColor);
            }

            spriteBatch.Draw(Box,
                new Rectangle((int)XPosition - (int)SMW.Level.X, (int)YPosition - (int)SMW.Level.Y, Box.Width, Box.Height),
                new Rectangle(0, 0, Box.Width, Box.Height),
                Color.White, 0.0F, Vector2.Zero, SpriteEffects.None, 1F);
        }

        public new void Dispose()
        {
            this.Box.Dispose();
            base.Dispose();
        }
    }
}
