using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;

namespace MarioWorldSharp
{
    /// <summary>
    /// A class with a 8-bit color indexed texture where each pixel points to a specific color passed into the texture
    /// </summary>
    public class IndexedTexture
    {
        public byte[] IndexData { get; set; }
        public Color[] Palette { get;  set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Color[] GetColorData(Color[] p = null, int w = -1, int h = -1, int offX = 0, int offY = 0)
        {
            if (p == null)
                p = Palette;
            if (w < 0)
                w = Width;
            if (h < 0)
                h = Height;

            Color[] textureData = new Color[w * h];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    textureData[i * w + j] = p[IndexData[(i + offY) * w + j + offX]];
                }
            }
            return textureData;
        }

        public Texture2D GetTexture2D(SpriteBatch spriteBatch, Color[] p = null, int w = -1, int h = -1, int offX = 0, int offY = 0)
        {
            if (p == null)
                p = Palette;
            if (w < 0)
                w = Width;
            if (h < 0)
                h = Height;

            Color[] textureData = GetColorData(p, w, h, offX, offY);
            Texture2D t = new Texture2D(spriteBatch.GraphicsDevice, w, h);
            t.SetData(textureData);
            return t;
        }
    }

    public class GraphicsHandler
    {
        private static Texture2D[] _smallPlayer = new Texture2D[70];
        public static Texture2D[] SmallPlayerGraphics { get => _smallPlayer; }

        /// <summary>
        /// Used to import graphics with 8-bit indexed colors (Same format as 8bpp Mode 7 SNES graphics)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="indexedGFX"></param>
        public static void ImportIndexedPlayerGraphics(SpriteBatch spriteBatch, byte[] indexedGFX)
        {
            IndexedTexture[] textures = new IndexedTexture[0xB800 / 64];
            //8x8 tile, y, x
            for (int i = 0; i < 0xB800; i += 64)
            {
                byte[] colorSet = new byte[64];
                for (int j = 0; j < 64; j += 8)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        byte b = indexedGFX[i + j + k];
                        colorSet[j + k] = b;
                    }
                }
                textures[i / 64] = new IndexedTexture() { IndexData = colorSet, Width = 8, Height = 8 };
            }

            //Store the player tiles
            //  
            //  Idle
            _smallPlayer[0] = CombineIndexedTextures(2, 3, null,
                textures[0xB0], textures[0xB1], 
                textures[0xE2], textures[0xE3],
                textures[0xF2], textures[0xF3]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Run
            _smallPlayer[1] = CombineIndexedTextures(2, 3, null,
                textures[0xB0], textures[0xB1],
                textures[0xC0], textures[0xC1],
                textures[0xD0], textures[0xD1]).GetTexture2D(spriteBatch, MarioPalettes, 16, 23, 0, 1);
            //  Looking up
            _smallPlayer[2] = CombineIndexedTextures(2, 3, null,
                textures[0x212], textures[0x213],
                textures[0x222], textures[0x223],
                textures[0x232], textures[0x233]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Fast running 1
            _smallPlayer[3] = CombineIndexedTextures(2, 3, null,
                textures[0xB0], textures[0xB1],
                textures[0x128], textures[0x129],
                textures[0x138], textures[0x139]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Fast running 2
            _smallPlayer[4] = CombineIndexedTextures(2, 3, null,
                textures[0xB0], textures[0xB1],
                textures[0x12C], textures[0x12D],
                textures[0x13C], textures[0x13D]).GetTexture2D(spriteBatch, MarioPalettes, 16, 23, 0, 1);
            //  Jump
            _smallPlayer[5] = CombineIndexedTextures(2, 3, null,
                textures[0x256], textures[0x257],
                textures[0x266], textures[0x267],
                textures[0x276], textures[0x277]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Fast Jump
            _smallPlayer[6] = CombineIndexedTextures(2, 3, null,
                textures[0xB0], textures[0xB1],
                textures[0x168], textures[0x169],
                textures[0x178], textures[0x179]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Skidding
            _smallPlayer[7] = CombineIndexedTextures(2, 3, null,
                textures[0x25A], textures[0x25B],
                textures[0x26A], textures[0x26B],
                textures[0x27A], textures[0x27B]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Facing Forward
            _smallPlayer[8] = CombineIndexedTextures(2, 3, null,
                textures[0x1BA], textures[0x1BB],
                textures[0x1CA], textures[0x1CB],
                textures[0x1DA], textures[0x1DB]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Falling
            _smallPlayer[9] = CombineIndexedTextures(2, 3, null,
                textures[0x258], textures[0x259],
                textures[0x268], textures[0x269],
                textures[0x278], textures[0x279]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Facing Backward
            _smallPlayer[10] = CombineIndexedTextures(2, 3, null,
                textures[0x17C], textures[0x17D],
                textures[0x18C], textures[0x18D],
                textures[0x19C], textures[0x19D]).GetTexture2D(spriteBatch, MarioPalettes);
            //  Ducking
            _smallPlayer[11] = CombineIndexedTextures(2, 3, null,
                textures[0x16C], textures[0x16C],
                textures[0x28C], textures[0x28D],
                textures[0x29C], textures[0x29D]).GetTexture2D(spriteBatch, MarioPalettes);
        }

        public static IndexedTexture CombineIndexedTextures(int widthByTiles, int heightByTiles, Color[] p = null, params IndexedTexture[] tiles)
        {
            byte[] ret = new byte[widthByTiles * heightByTiles * 64];

            for (int i = 0; i < heightByTiles; i++)
            {
                for (int j = 0; j < widthByTiles; j++)
                {
                    IndexedTexture t = tiles[(i * widthByTiles + j) % tiles.Length];
                    for (int k = 0; k < 64; k++)
                    {
                        int l = (k / 8) * (widthByTiles * 8);
                        int m = k % 8;
                        ret[(i * widthByTiles * 8 + j) * 8 + l + m] = t.IndexData[k];
                    }
                }
            }

            return new IndexedTexture() { IndexData = ret, Palette = p, Width = widthByTiles * 8, Height = heightByTiles * 8 };
        }

        public static Color SNESRGBConversion(short c)
        {
            return new Color((float)(c & 0x1F) / 31.0f, (float)(c >> 5 & 0x1F) / 31.0F, (float)(c >> 10 & 0x1F) / 31.0F, 1.0F);
        }

        static readonly Color[] MarioPalettes = new Color[]
        {
            Color.Transparent,
            Color.White,
            Color.Black,
            SNESRGBConversion(0xD71),
            SNESRGBConversion(0x1E9B),
            SNESRGBConversion(0x3B7F),
            SNESRGBConversion(0x635F),
            SNESRGBConversion(0x581D),
            SNESRGBConversion(0xA),
            SNESRGBConversion(0x391F),
            SNESRGBConversion(0x44C4),
            SNESRGBConversion(0x4E08),
            SNESRGBConversion(0x6770),
            SNESRGBConversion(0x30B6),
            SNESRGBConversion(0x35DF),
            SNESRGBConversion(0x3FF)
        };
    }
}
