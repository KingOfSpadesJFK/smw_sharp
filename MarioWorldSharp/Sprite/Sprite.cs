using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;

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

        //Index to spawn order. Set to -1 if not indexed to a spawn order, like if it was spawned from another sprite
        public int Index { get; }  
        public void Process();
        public Rectangle GetCollisionBox();
    }

    public abstract class Sprite : IDisposable, ISprite
    {
        private double _xPos;
        private double _yPos;
        protected Rectangle _collisionBox;
        public double XPosition
        {
            get => _xPos;
            set
            {
                _xPos = value;
                if (_collisionBox != null)
                    _collisionBox.X = (int)value;
            }
        }
        public double YPosition
        {
            get => _yPos;
            set
            {
                _yPos = value;
                if (_collisionBox != null)
                    _collisionBox.Y = (int)value;
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
        public int Index { get; }

        public Sprite(double x, double y, int index)
        {
            this._collisionBox = Rectangle.Empty;
            this.XPosition = x; this.YPosition = y;
            this.Status = SpriteStatus.Normal;
            this.Index = index;
        }

        public abstract void Process();

        public Rectangle GetCollisionBox()
        {
            return _collisionBox;
        }

        protected void UpdateXPositionition()
        {
            double gravity = HorizGravity * .375;
            if (XSpeed < 64.0 / 16.0)
                XSpeed += gravity;
            XPosition += XSpeed;
        }

        protected void UpdateYPositionition()
        {
            double gravity = VertGravity * .375;
            if (YSpeed < 64.0 / 16.0)
                YSpeed += gravity;
            YPosition += YSpeed;
        }
        protected void EnvironmentCollision()
        {
            //TODO: Implement
        }

        protected void PlayerCollision()
        {
            //TODO: Implement
        }
        protected void SpriteCollision()
        {
            //TODO: Implement
        }

        protected void OffScreen(byte sub)
        {
            //TODO: Implement
            if (XPosition < MarioWorld.level.X)
                this.Dispose();
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
                    Status = SpriteStatus.NonExistent;
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
