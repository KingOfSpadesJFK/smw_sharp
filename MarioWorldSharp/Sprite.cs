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

namespace MarioWorldSharp
{
    public enum SpriteStatus
    {
        NonExistent = 0,
        Init = 1,
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

    public interface Sprite
    {
        void Process();
        void UpdateXPosition();
        void UpdateYPosition();
        double GetX();
        double GetY();
        void SetX(double xPos);
        void SetY(double yPos);
        void EnvironmentCollision();
        void PlayerCollision();
        Rectangle GetCollisionBox();
        bool[] GetBlockedStatus();
    }

    public abstract class BlankSprite : IDisposable, Sprite
    {
        private SpriteStatus stat;
        private double xPos;
        private double yPos;
        private double xSpeed;
        private double ySpeed;
        private double xAccel;
        private double yAccel;
        private byte vGravity;
        private byte hGravity;
        private Rectangle collisionBox;
        private bool blockedBellow;
        private bool blockedAbove;
        private bool blockedLeft;
        private bool blockedRight;

        public BlankSprite(double xPos, double yPos)
        {
            this.xPos = xPos; this.yPos = yPos;
        }

        public abstract void Process();

        public void UpdateXPosition()
        {
        }
        public void UpdateYPosition()
        {
        }
        public void EnvironmentCollision()
        {
        }
        public void PlayerCollision()
        {
        }

        public double GetX() { return xPos; }
        public double GetY() { return yPos; }
        public void SetX(double newX) { xPos = newX; }
        public void SetY(double newY) { yPos = newY; }

        public Rectangle GetCollisionBox()
        {
            return collisionBox;
        }

        public bool[] GetBlockedStatus()
        {
            bool[] ret = new bool[4];
            ret[0] = blockedAbove;
            ret[1] = blockedBellow;
            ret[2] = blockedLeft;
            ret[3] = blockedRight;
            return ret;
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
                    stat = SpriteStatus.NonExistent;
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
