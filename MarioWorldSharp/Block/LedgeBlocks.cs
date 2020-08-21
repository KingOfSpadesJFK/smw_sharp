using System;
using System.Collections.Generic;
using System.Text;

namespace MarioWorldSharp.Block
{
    public class Ledge : PassThroughBlock
    {
        public override void Bellow(Player Mario, double x, double y)
        {
            if (Mario.YSpeed > 0 && y % 16.0 <= 4.0)
                Blocks.SOLID_BLOCK.Bellow(Mario, x, y);
        }
    }
}
