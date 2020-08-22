using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using static MarioWorldSharp.Level;

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
    }

    public class SpriteData
    {
        public SpriteID ID;
        public int Index;
        public bool Spawned;
        public bool DisposeOffscreen;
        public int DespawnThresh = 16;
    }
    public abstract class Sprite : IDisposable, ISprite
    {
        private double _xPos;
        private double _yPos;
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

        protected virtual void UpdateXPosition()
        {
            double gravity = HorizGravity * .375;
            if (XSpeed < 64.0 / 16.0)
                XSpeed += gravity;
            XPosition += XSpeed;
            if (XSpeed != 0.0)
                SpriteHandler.UpdateSpriteTree(this, false);
        }

        protected virtual void UpdateYPosition()
        {
            double gravity = VertGravity * .375;
            if (YSpeed < 64.0 / 16.0)
                YSpeed += gravity;
            YPosition += YSpeed;
            if (YSpeed != 0.0)
                SpriteHandler.UpdateSpriteTree(this, false);
        }

        protected virtual void UpdateXYPosition()
        {
            double gravity = HorizGravity * .375;
            if (XSpeed < 64.0 / 16.0)
                XSpeed += gravity;
            XPosition += XSpeed;

            gravity = VertGravity * .375;
            if (YSpeed < 64.0 / 16.0)
                YSpeed += gravity;
            YPosition += YSpeed;

            if (YSpeed != 0.0 || XSpeed != 0.0)
                SpriteHandler.UpdateSpriteTree(this, false);
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
            //TODO: Implement
        }
        protected virtual void SpriteCollision()
        {
            //TODO: Implement
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
        protected virtual void Kill()
        {
            SMW.Level.RemoveSprite(Data);
            this.Dispose();
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (Status != SpriteStatus.NonExistent)
                    {
                        Status = SpriteStatus.NonExistent;
                        SMW.Level.HideSprite(Data);
                        SpriteHandler.UpdateSpriteTree(this, true);
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

    }
}
