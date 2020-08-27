using System;
using System.Collections.Generic;
using System.Text;

namespace MarioWorldSharp.Blocks
{
    public class BlockList
    {
        public static readonly Block LEDGE_BLOCK = new Ledge();
        public static readonly Block SOLID_BLOCK = new SolidBlock();
        public static readonly Block EMPTY_BLOCK = new PassThroughBlock();
    }
}
