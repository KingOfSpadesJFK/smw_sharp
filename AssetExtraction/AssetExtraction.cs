using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace AssetExtraction
{
    class AssetExtraction
    {
        #region Lunar Compress
        [DllImport("Lunar Compress.dll")]
        public static extern bool LunarCreatePixelMap([MarshalAs(UnmanagedType.LPArray)] byte[] Source, [MarshalAs(UnmanagedType.LPArray)] byte[] Destination, uint NumTiles, uint GFXType);
        
        [DllImport("Lunar Compress.dll")]
        static extern int LunarOpenRAMFile([MarshalAs(UnmanagedType.LPArray)] byte[] data, int fileMode, int size);

        [DllImport("Lunar Compress.dll")]
        static extern bool LunarSaveRAMFile(string Path);

        [DllImport("Lunar Compress.dll")]
        static extern bool LunarCloseFile();

        [DllImport("Lunar Compress.dll")]
        public static extern int LunarDecompress([MarshalAs(UnmanagedType.LPArray)] byte[] Destination, uint AddressToStart, uint MaxDataSize, uint Format, uint Format2, out uint LastROMPosition);
        #endregion

        static uint ROMPosition;
        static uint LastROMPosition;
        static FileStream ROMFileStream;
        static Memory<byte> ROMBuffer;

        static void Main(string[] args)
        {
            string saveLoc = "";
            if (args.Length != 0)
            {
                switch (args[0])
                {
                    case "-d":
                        if (args.Length <= 1)
                        {
                            Console.WriteLine("-d requires a relative directory afterwards");
                            return;
                        }
                        saveLoc = $"{args[1]}/";
                        Directory.SetCurrentDirectory(Directory.GetCurrentDirectory()+$"/{args[1]}");
                        break;
                    default:
                        Console.WriteLine("Invalid argument");
                        return;
                }
            }

            ROMPosition = 0x40000;
            LastROMPosition = ROMPosition;
            ROMFileStream = new FileStream("baserom.sfc", FileMode.Open);

            using BinaryReader b = new BinaryReader(ROMFileStream);
            ROMBuffer = b.ReadBytes((int)ROMFileStream.Length);
            var crc32 = new Crc32();
            uint checksum = crc32.Get(ROMBuffer.ToArray());
            //Headered SMW CRC32: B9B52E0F
            //Unheadered SMW CRC32: B19ED489

            if (checksum != 0xB19ED489)
            { Console.WriteLine("Invalid ROM. Make sure the ROM is an unheadered unmoddified American Super Mario World ROM."); return; }

            ROMFileStream.Close();
            ROMFileStream.Dispose();

            if (!Directory.Exists("assets/image/snes/"))
                Directory.CreateDirectory("assets/image/snes/");

            for (int i = 0; i < 0x34; i++)
            {
                string path = $"assets/image/snes/GFX{i:X2}_{names[i]}.bin";
                var varrick = DecompressGFX(bitDepths[i]);

                LunarOpenRAMFile(pixelMap, 5, pixelMap.Length);
                LunarSaveRAMFile(path);
                LunarCloseFile();
                Console.WriteLine("\n" +
                    $"      Saved to {saveLoc}{path}\n");
            }
            Console.WriteLine(LineBreak);
            Console.WriteLine("All assets extracted!\n");
        }

        readonly static string LineBreak = "-----------------------------------------------------\n";
        static byte[] pixelMap;

        static byte[] DecompressGFX(int bitDepth)
        {
            byte[] compArr = ROMBuffer.Slice((int)ROMPosition, 0x10000).ToArray();
            LunarOpenRAMFile(compArr, 0, ROMBuffer.Length);
            byte[] decompSNES = new byte[0x10000];

            int size = LunarDecompress(decompSNES, 0, 0x10000, 1, 0, out var _lastpos);

            LastROMPosition = ROMPosition + _lastpos;
            string startS = ROMPosition.ToString("X");
            string endS = LastROMPosition.ToString("X");
            Console.WriteLine(LineBreak + 
                $"\nDecompressed graphics block 0x{startS} to 0x{endS} \n" +
                $"      Size compressed: {LastROMPosition - ROMPosition} bytes({(LastROMPosition - ROMPosition)/1024}kb) \n" +
                $"      Size decompressed: {size} bytes ({size / 1024}kb)");

            pixelMap = new byte[size / bitDepth * 8];
            LunarCreatePixelMap(decompSNES, pixelMap, (uint)(size / (8*bitDepth)), (uint)bitDepth);
            Console.WriteLine(
                $"      Reformatted size: {pixelMap.Length} bytes ({pixelMap.Length / 1024}kb)");
            //Format:
            //0x40 bytes make one 8x8 tile
            //0x400 bytes make one line of 8x8 tiles
            //0x4000 bytes make one full gfx page

            LunarCloseFile();

            ROMPosition = LastROMPosition;
            return pixelMap;
        }

        readonly static int[] bitDepths = new int[]
        {
            4,3,3,3, 3,3,3,3,   //00-0F
            3,3,3,3, 3,3,3,3,
            3,3,3,3, 3,3,3,3,   //10-1F
            3,3,3,3, 3,3,3,3,
            3,3,3,3, 3,3,3,3,   //20-2F
            3,3,2,2, 2,2,3,3,
            3,3,3,3             //30-33
        };

        public static string[] names = new string[]
        {
            "Player",
            "Animation",
            "GenericSprites1",
            "GenericSprites2",
            "ForestSprites",
            "CastleSprites2",
            "CaveSprites",
            "MushroomSprites",
            "WaterSprites",
            "GhostHouseTiles",
            "SwitchPalaceTiles",
            "PokeySumoMoleSprites",
            "LemmyWendySprites",
            "LudwigMortonRoySprites",
            "GhostHouseCaveBG",
            "Peach",
            "DiscoNinjiSprites_PeachEnd",
            "YoshiHouseSprites",
            "PlayerOverworldSprites",
            "GhostHouseSprites",
            "CastleSprites1",
            "GenericSprites3",
            "GenericTiles1_OverworldAnimation",
            "GrassTiles",
            "RopeTiles",
            "GenericTiles2",
            "CastleTiles",
            "ForestHillsBG",
            "CaveTiles",
            "DottedHillsCastle1BG",
            "OverworldTiles1",
            "OverworldTiles2",
            "OverworldTiles3",
            "ForestCloudTiles",
            "BanzaiBillSprites",
            "Bowser",
            "BossBackgrounds",
            "DinoRhino",
            "BowserBattleSprites",
            "IggyLarryReznor",
            "YoshiEndingSprites",
            "Mode7",
            "Layer3Tiles1",
            "Layer3Tiles2",
            "Layer3Tiles3",
            "Layer3Tiles4",
            "CastleCutsceneTiles1",
            "CastleCutsceneTiles2",
            "ThankYouTiles",
            "CreditsFont",
            "MarioLuigiEndScreen",
            "SpecialWorldSprites"

        };
    }

    /// <summary>
    /// Performs 32-bit reversed cyclic redundancy checks.
    /// </summary>
    public class Crc32
    {
        #region Constants
        /// <summary>
        /// Generator polynomial (modulo 2) for the reversed CRC32 algorithm. 
        /// </summary>
        private const UInt32 s_generator = 0xEDB88320;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the Crc32 class.
        /// </summary>
        public Crc32()
        {
            // Constructs the checksum lookup table. Used to optimize the checksum.
            m_checksumTable = Enumerable.Range(0, 256).Select(i =>
            {
                var tableEntry = (uint)i;
                for (var j = 0; j < 8; ++j)
                {
                    tableEntry = ((tableEntry & 1) != 0)
                        ? (s_generator ^ (tableEntry >> 1))
                        : (tableEntry >> 1);
                }
                return tableEntry;
            }).ToArray();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculates the checksum of the byte stream.
        /// </summary>
        /// <param name="byteStream">The byte stream to calculate the checksum for.</param>
        /// <returns>A 32-bit reversed checksum.</returns>
        public UInt32 Get<T>(IEnumerable<T> byteStream)
        {
            try
            {
                // Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
                return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
                          (m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
            }
            catch (FormatException e)
            {
                throw e;
            }
            catch (InvalidCastException e)
            {
                throw e;
            }
            catch (OverflowException e)
            {
                throw e;
            }
        }
        #endregion

        #region Fields
        /// <summary>
        /// Contains a cache of calculated checksum chunks.
        /// </summary>
        private readonly UInt32[] m_checksumTable;

        #endregion
    }
}
