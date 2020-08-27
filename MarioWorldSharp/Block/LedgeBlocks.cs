using MarioWorldSharp.Sprite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarioWorldSharp.Blocks
{
    public class Ledge : PassThroughBlock
    {
        public override void Bellow(Player p, double x, double y)
        {
            if (p.YSpeed > 0 && y % 16.0 <= 8.0)
                BlockList.SOLID_BLOCK.Bellow(p, x, y);
        }
        public override void Bellow(ISprite p, double x, double y)
        {
            if (p.YSpeed > 0 && y % 16.0 <= 8.0)
                BlockList.SOLID_BLOCK.Bellow(p, x, y);
        }
    }


}
