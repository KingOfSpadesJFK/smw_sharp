using System;
using System.Collections.Generic;
using System.Text;

namespace MarioWorldSharp.Block
{
    public class Blocks
    {
        public static readonly AbstractBlock LEDGE_BLOCK = new Ledge();
        public static readonly AbstractBlock SOLID_BLOCK = new SolidBlock();
        public static readonly AbstractBlock EMPTY_BLOCK = new PassThroughBlock();
    }
}
