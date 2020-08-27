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
        public byte[] ColorData { get; set; }
        public Color[] Palette { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public SpriteBatch SpriteBatch;

        private Texture2D Texture;

        public Texture2D GetTexture2D()
        {
            if (Texture != null)
                return Texture;
            return UpdateTexture2D();
        }

        private Texture2D UpdateTexture2D()
        {
            Color[] textureData = new Color[Width * Height];
            for (int i = 0; i < textureData.Length; i++)
            {
                textureData[i] = Palette[ColorData[i]];
            }
            Texture2D t = new Texture2D(SpriteBatch.GraphicsDevice, Width, Height);
            t.SetData(textureData);
            Texture.Dispose();
            Texture = t;
            return Texture;
        }
    }

    public class GraphicsHandler
    {
        private static Texture2D[] _smallPlayer = new Texture2D[70];
        public static Texture2D[] SmallPlayerGraphics { get => _smallPlayer; }

        public static Color[] ConvertIndexedTexture(IndexedTexture t)
        {
            Color[] ret = new Color[t.ColorData.Length];

            for (int i = 0; i < t.Height * t.Width; i += t.Width)
            {
                for (int j = 0; j < t.Width; j++)
                {
                    int b = t.ColorData[i + j];
                    ret[i + j] = t.Palette[b];
                }
            }
            return ret;
        }

        /// <summary>
        /// Used to import graphics with 8-bit indexed colors (Same format as 8bpp Mode 7 SNES graphics)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="indexedGFX"></param>
        public static void ImportIndexedPlayerGraphics(SpriteBatch spriteBatch, byte[] indexedGFX)
        {
            Color[][] textures = new Color[0xB800 / 64][];
            //8x8 tile, y, x
            for (int i = 0; i < 0xB800; i += 64)
            {
                Color[] colorSet = new Color[64];
                for (int j = 0; j < 64; j += 8)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        int b = indexedGFX[i + j + k];
                        colorSet[j + k] = b != 0 ? SNESRGBConversion(MarioPalettes[b]) : Color.Transparent;
                    }
                }
                textures[i / 64] = colorSet;
            }

            //Store the player tiles
            //  
            //  Idle
            Texture2D gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles2(textures, 2, 3, 0xB0, 0xB1, 0xE2, 0xE3, 0xF2, 0xF3));
            _smallPlayer[0] = gfx;
            //  Run
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 23);
            gfx.SetData(GrabTiles(textures, 2, 3, 0xB0), 16, 16 * 23);
            _smallPlayer[1] = gfx;
            //  Looking up
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles(textures, 2, 3, 0x212));
            _smallPlayer[2] = gfx;
            //  Fast running 1
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles2(textures, 2, 3, 0xB0, 0xB1, 0x128, 0x129, 0x138, 0x139));
            _smallPlayer[3] = gfx;
            //  Fast running 2
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 23);
            gfx.SetData(GrabTiles2(textures, 2, 3, 0xB0, 0xB1, 0x12C, 0x12D, 0x13C, 0x13D), 16, 16 * 23);
            _smallPlayer[4] = gfx;
            //  Jump
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles(textures, 2, 3, 0x256));
            _smallPlayer[5] = gfx;
            //  Fast Jump
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles2(textures, 2, 3, 0xB0, 0xB1, 0x168, 0x169, 0x178, 0x179));
            _smallPlayer[6] = gfx;
            //  Skidding
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles(textures, 2, 3, 0x25A));
            _smallPlayer[7] = gfx;
            //  Facing Forward
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles(textures, 2, 3, 0x1BA));
            _smallPlayer[8] = gfx;
            //  Falling
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles(textures, 2, 3, 0x258));
            _smallPlayer[9] = gfx;
            //  Facing Backward
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles(textures, 2, 3, 0x17C));
            _smallPlayer[10] = gfx;
            //  Ducking
            gfx = new Texture2D(spriteBatch.GraphicsDevice, 16, 24);
            gfx.SetData(GrabTiles2(textures, 2, 3, 0x16C, 0x16C, 0x28C, 0x28D, 0x29C, 0x29D));
            _smallPlayer[11] = gfx;
        }

        public static Color[] CombineTiles(int tileWidth, int tileHeight, int widthByTiles, int heightByTiles, params Color[][] tiles)
        {
            Color[] ret = new Color[widthByTiles * heightByTiles * tileWidth * tileHeight];

            for (int i = 0; i < heightByTiles; i++)
            {
                for (int j = 0; j < widthByTiles; j++)
                {
                    int k = 0;
                    foreach (Color c in tiles[i * widthByTiles + j])
                    {
                        ret[i * tileWidth + j + k] = c;
                        k++;
                    }
                }
            }

            return ret;
        }

        public static Color[] GrabTiles(Color[][] textures, int widthByTiles, int heightByTiles, int startPointer)
        {
            Color[] ret = new Color[widthByTiles * heightByTiles * 64];

            for (int i = 0; i < heightByTiles; i++)
            {
                for (int j = 0; j < widthByTiles; j++)
                {
                    int texturesPointer = i * 16 + j % 16 + startPointer;
                    for (int k = 0; k < 64; k++)
                    {
                        int l = (k / 8) * (widthByTiles * 8);
                        int m = k % 8;
                        ret[(i * widthByTiles * 8 + j) * 8 + l + m] = textures[texturesPointer][k];
                    }
                }
            }

            return ret;
        }

        public static Color[] GrabTiles2(Color[][] textures, int widthByTiles, int heightByTiles, params int[] startPointer)
        {
            Color[] ret = new Color[widthByTiles * heightByTiles * 64];

            for (int i = 0; i < heightByTiles; i++)
            {
                for (int j = 0; j < widthByTiles; j++)
                {
                    int texturesPointer = startPointer[(i * widthByTiles + j) % startPointer.Length];
                    for (int k = 0; k < 64; k++)
                    {
                        int l = (k / 8) * (widthByTiles * 8);
                        int m = k % 8;
                        ret[(i * widthByTiles * 8 + j) * 8 + l + m] = textures[texturesPointer][k];
                    }
                }
            }

            return ret;
        }

        public static Color SNESRGBConversion(short c)
        {
            return new Color((c & 0x1F) * 8, (c >> 5 & 0x1F) * 8, (c >> 10 & 0x1F) * 8, 255);
        }

        static readonly short[] MarioPalettes = new short[]
        {
            0x0,
            0x7FFF,
            0x0,
            0xD71,
            0x1E9B,
            0x3B7F,
            0x635F,
            0x581D,
            0xA,
            0x391F,
            0x44C4,
            0x4E08,
            0x6770,
            0x30B6,
            0x35DF,
            0x3FF
        };
    }
}
