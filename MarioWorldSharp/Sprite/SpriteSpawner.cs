using System;
using System.Collections.Generic;
using System.Text;
using static MarioWorldSharp.Level;

namespace MarioWorldSharp.Sprite
{
    public enum SpriteID
    {
        GreenShellessKoopa,
        Mario
    }
    public class SpriteSpawner
    {
        public static ISprite SpawnSprite(double[] point, SpriteData d, params object[] args)
        {
            return SpawnSprite(point[0], point[1], d, args);
        }
        public static ISprite SpawnSprite(double x, double y, SpriteData d, params object[] args)
        {
            switch (d.ID)
            {
                case (int)SpriteID.GreenShellessKoopa:
                    d.DisposeOffscreen = true;
                    return new ShellessKoopa(x,y,d, 0);
            }

            return null;
        }
    }
}
