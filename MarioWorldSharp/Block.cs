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
    public abstract class Block : IDisposable
    {
        public abstract void Above(Player Mario, double x, double y);
        public abstract void Above(ISprite sprite, double x, double y);
        public abstract void Bellow(Player Mario, double x, double y);
        public abstract void Bellow(ISprite sprite, double x, double y);
        public abstract void Left(Player Mario, double x, double y);
        public abstract void Left(ISprite sprite, double x, double y);
        public abstract void Right(Player Mario, double x, double y);
        public abstract void Right(ISprite sprite, double x, double y);
        public abstract void TopCorner(Player Mario, double x, double y);
        public abstract void TopCorner(ISprite sprite, double x, double y);
        public abstract void BodyInside(Player Mario, double x, double y);
        public abstract void BodyInside(ISprite sprite, double x, double y);
        public abstract void HeadInside(Player Mario, double x, double y);
        public abstract void WallRun(Player Mario, double x, double y);

        public abstract void Cape(double x, double y);
        public abstract void Fireball(double x, double y);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Block() {
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
