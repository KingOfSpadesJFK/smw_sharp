using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;

using MarioWorldSharp.Sprite;
using KdTree;
using KdTree.Math;
using System.Linq;
using MarioWorldSharp.Levels;
using System.Runtime.InteropServices;

namespace MarioWorldSharp
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 

    public class SMW : Game
    {
        GraphicsDeviceManager graphics;
        RenderTarget2D gamescreen;
        SpriteBatch spriteBatch;
        SpriteFont debugFont;
        Texture2D[] map16Textures;
        bool resChange;
        float scale;

        public static Player Character;
        public static Level Level;
        public static int GameMode;
        public static Color colorDisp;

        readonly int ResWidth = 400;
        readonly float ResWidthF = 400F;
        readonly int ResHeight = 224;
        readonly float ResHeightF = 224F;
        private bool drawCollision = false;
        public static InputEvent InputEvent = new InputEvent();

        public SMW()
        {
            scale = 3f;
            resChange = false;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int) (ResWidth * scale);
            graphics.PreferredBackBufferHeight = (int) (ResHeight * scale);
            graphics.HardwareModeSwitch = false;
            Content.RootDirectory = "Content";
            GameMode = 0;
            colorDisp = new Color();

            InputEvent.DEBUG_ShowHitboxEvent += ShowHitbox;
            InputEvent.DEBUG_PrintSpriteTreeEvent += PrintSpriteTree;
            InputEvent.DEBUG_KillAllSpritesEvent += SpriteHandler.KillSprites;
            InputEvent.DEBUG_ResetLevelEvent += ResetLevel;
        }

        private void ResetLevel(object sender, EventArgs e)
        {
            SpriteHandler.KillSprites(sender, e);
            Character = new Player();
            Character.Poses[0] = GraphicsHandler.SmallPlayerGraphics[0];
            Character.Poses[1] = GraphicsHandler.SmallPlayerGraphics[1];
            Character.Poses[2] = Character.Poses[1];
            Character.Poses[3] = GraphicsHandler.SmallPlayerGraphics[2];
            Character.Poses[4] = GraphicsHandler.SmallPlayerGraphics[3];
            Character.Poses[5] = GraphicsHandler.SmallPlayerGraphics[4];
            Character.Poses[6] = Character.Poses[5];
            Character.Poses[0x0B] = GraphicsHandler.SmallPlayerGraphics[5];
            Character.Poses[0x0C] = GraphicsHandler.SmallPlayerGraphics[6];
            Character.Poses[0x0D] = GraphicsHandler.SmallPlayerGraphics[7];
            Character.Poses[0x0F] = GraphicsHandler.SmallPlayerGraphics[8];
            Character.Poses[0x24] = GraphicsHandler.SmallPlayerGraphics[9];
            Character.Poses[0x39] = GraphicsHandler.SmallPlayerGraphics[10];
            Character.Poses[0x3C] = GraphicsHandler.SmallPlayerGraphics[11];
            Level = new Level();
            Level.SpawnSprites();
            Console.WriteLine(SpriteHandler.GetSpriteTree());
        }

        private void PrintSpriteTree(object sender, EventArgs e)
        {
            Console.WriteLine($"Sprite Tree: \n {SpriteHandler.GetSpriteTree()}");

            Console.WriteLine("Sprite List:");
            foreach (ISprite s in SpriteHandler.SpriteList)
                Console.WriteLine($"    ({s})");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Character = new Player();
            Level = new Level();
            Level.SpawnSprites();
            Console.WriteLine(SpriteHandler.GetSpriteTree());
            gamescreen = new RenderTarget2D(graphics.GraphicsDevice, ResWidth, ResHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            GraphicsHandler.ImportIndexedPlayerGraphics(spriteBatch, Program.ExtractEmbeddedBin("assets.image.snes.GFX00_Player.bin"));

            Texture2D[] smallPoses;
            Texture2D map16_page1;
            Texture2D map16_page2;
            Character.Poses[0] = GraphicsHandler.SmallPlayerGraphics[0];
            Character.Poses[1] = GraphicsHandler.SmallPlayerGraphics[1];
            Character.Poses[2] = Character.Poses[1];
            Character.Poses[3] = GraphicsHandler.SmallPlayerGraphics[2];
            Character.Poses[4] = GraphicsHandler.SmallPlayerGraphics[3];
            Character.Poses[5] = GraphicsHandler.SmallPlayerGraphics[4];
            Character.Poses[6] = Character.Poses[5];
            Character.Poses[0x0B] = GraphicsHandler.SmallPlayerGraphics[5];
            Character.Poses[0x0C] = GraphicsHandler.SmallPlayerGraphics[6];
            Character.Poses[0x0D] = GraphicsHandler.SmallPlayerGraphics[7];
            Character.Poses[0x0F] = GraphicsHandler.SmallPlayerGraphics[8];
            Character.Poses[0x24] = GraphicsHandler.SmallPlayerGraphics[9];
            Character.Poses[0x39] = GraphicsHandler.SmallPlayerGraphics[10];
            Character.Poses[0x3C] = GraphicsHandler.SmallPlayerGraphics[11];
            map16_page1 = Content.Load<Texture2D>("assets\\image\\map16\\grass1");
            map16_page2 = Content.Load<Texture2D>("assets\\image\\map16\\grass2");
            debugFont = Content.Load<SpriteFont>("assets\\font\\debug");
            map16Textures = new Texture2D[16 * 32];

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    Color[] imageData = new Color[256 * 256];
                    Rectangle sourceRectangle;
                    if (y >= 16)
                    {
                        map16_page2.GetData<Color>(imageData);
                        sourceRectangle = new Rectangle(x * 16, (y-16) * 16, 16, 16);
                    }
                    else
                    {
                        map16_page1.GetData<Color>(imageData);
                        sourceRectangle = new Rectangle(x * 16, y * 16, 16, 16);
                    }
                    Color[] imagePiece = GetImageData(imageData, 256, sourceRectangle);
                    Texture2D subtexture = new Texture2D(GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                    subtexture.SetData<Color>(imagePiece);
                    int place = (y * 16) + x;
                    map16Textures[place] = subtexture;
                }
            }
            map16_page1.Dispose();
            map16_page2.Dispose();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected new void Exit()
        {
            base.Exit();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            InputEvent.Process();
            LevelGameMode();
            base.Update(gameTime);

            FrameTimer++;
        }

        private void ShowHitbox(object sender, EventArgs e)
        {
            drawCollision = !drawCollision;
        }

        private void LevelGameMode()
        {
            Character.Process();
            SpriteHandler.ProcessSprites();
            Level.Scroll(Character.XPosition, Character.YPosition);
        }

        public static bool SecondPassed { get => FrameTimer % 60L == 0; }
        public static long FrameTimer { get; set; }

        private static double fpsTotal = 0;
        private static double fps = 0;
        private static int fpsAvgCount = 0;
        private int FPS(GameTime gameTime)
        {
            fpsTotal += (1000.0 / gameTime.ElapsedGameTime.TotalMilliseconds);
            fpsAvgCount++;
            if (FrameTimer % 30L == 0)
            { fps = fpsTotal / fpsAvgCount; fpsAvgCount = 0;  fpsTotal = 0; }

            if (fps % 1.0 > 0.5)
                return (int)fps + 1;
            return (int)fps;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var key = Keyboard.GetState();
            #region Resolution changing
            if (!resChange && key.IsKeyDown(Keys.D1))
            {
                if (graphics.IsFullScreen)
                    graphics.ToggleFullScreen();
                scale = 1;
                resChange = true;
                graphics.PreferredBackBufferWidth = (int)(ResWidth * scale);
                graphics.PreferredBackBufferHeight = (int)(ResHeight * scale);
                graphics.ApplyChanges();
            }
            else if (!resChange && key.IsKeyDown(Keys.D2))
            {
                if (graphics.IsFullScreen)
                    graphics.ToggleFullScreen();
                scale = 2;
                resChange = true;
                graphics.PreferredBackBufferWidth = (int)(ResWidth * scale);
                graphics.PreferredBackBufferHeight = (int)(ResHeight * scale);
                graphics.ApplyChanges();
            }
            else if (!resChange && key.IsKeyDown(Keys.D3))
            {
                if (graphics.IsFullScreen)
                    graphics.ToggleFullScreen();
                scale = 3;
                resChange = true;
                graphics.PreferredBackBufferWidth = (int)(ResWidth * scale);
                graphics.PreferredBackBufferHeight = (int)(ResHeight * scale);
                graphics.ApplyChanges();
            }
            else if (!resChange && key.IsKeyDown(Keys.D4))
            {
                if (GraphicsDevice.DisplayMode.Width / ResWidthF < GraphicsDevice.DisplayMode.Height / ResHeightF)
                    scale = (int) (GraphicsDevice.DisplayMode.Width / ResWidthF);
                else
                    scale = (int) (GraphicsDevice.DisplayMode.Height / ResHeightF);

                resChange = true;
                graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                graphics.ApplyChanges();
                if (!graphics.IsFullScreen)
                    graphics.ToggleFullScreen();
            }
            else if (!resChange && key.IsKeyDown(Keys.D5))
            {
                if (GraphicsDevice.DisplayMode.Width / ResWidthF < GraphicsDevice.DisplayMode.Height / ResHeightF)
                    scale = (GraphicsDevice.DisplayMode.Width / ResWidthF);
                else
                    scale = (GraphicsDevice.DisplayMode.Height / ResHeightF);

                resChange = true;
                graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                graphics.ApplyChanges();
                if (!graphics.IsFullScreen)
                    graphics.ToggleFullScreen();
            }
            else if (resChange && key.IsKeyUp(Keys.D1) && key.IsKeyUp(Keys.D2) && key.IsKeyUp(Keys.D3) && key.IsKeyUp(Keys.D4) && key.IsKeyUp(Keys.D5))
                resChange = false;
            #endregion

            GraphicsDevice.SetRenderTarget(gamescreen);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Level Drawing
            //TODO: Optimize routine
            Level layer = Level;
            while (layer != null)
            {
                short[,] leveldraw = layer.GetCameraSelection();
                for (int i = 0; i < leveldraw.GetLength(0); i++)
                {
                    for (int j = 0; j < leveldraw.GetLength(1); j++)
                    {
                        spriteBatch.Draw(map16Textures[leveldraw[i, j]], new Vector2((int)-Level.X % 16 + (i * 16), (int)-Level.Y % 16 + (j * 16)), Color.White);
                    }
                }
                layer = layer.GetNextLayer();
            }

            //Draw Sprites
            foreach (ISprite s in SpriteHandler.SpriteList)
                s?.Draw(spriteBatch);

            //Draw player
            Character.Draw(spriteBatch);

            if (drawCollision)
            {
                Texture2D box = CreateRectangleTexture(Character.GetCollisionBox(), new Color(64, 0, 0, 100), new Color(255, 45, 45, 100));
                Texture2D cross = CreateCrossTexture(Color.Black);
                spriteBatch.Draw(box, new Vector2(Character.GetCollisionBox().X - (int)Level.X, Character.GetCollisionBox().Y - (int)Level.Y), Color.White);
                spriteBatch.Draw(cross, new Vector2((int)Character.XPosition - (int)Level.X - 1, (int)Character.YPosition - (int)Level.Y - 1), Color.White);
                spriteBatch.End();
                box.Dispose();
                cross.Dispose();
            }
            else
                spriteBatch.End();

            float width = graphics.PreferredBackBufferWidth;
            float height = graphics.PreferredBackBufferHeight;
            int trueScale = (int)scale;
            if (scale % 1F != 0)
                trueScale++;
            //if (scale % 1F >= 0.5)
            //    trueScale += 1;
            float x = (width - (ResWidthF * scale)) / (scale * 2);
            float y = (height - (ResHeightF * scale)) / (scale * 2);
            if (trueScale != (int) scale)
            {
                RenderTarget2D temp = new RenderTarget2D(graphics.GraphicsDevice, ResWidth * trueScale, ResHeight * trueScale, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
                GraphicsDevice.SetRenderTarget(temp);
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, Matrix.CreateScale(trueScale));
                spriteBatch.Draw(gamescreen, new Vector2(0, 0), Color.White);
                spriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Matrix.CreateScale(scale / (float)trueScale));
                spriteBatch.Draw(temp, new Vector2(x, y), Color.White);
                spriteBatch.End();
                temp.Dispose();
            }
            else
            {
                GraphicsDevice.SetRenderTarget(null);
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, Matrix.CreateScale(trueScale));
                spriteBatch.Draw(gamescreen, new Vector2(x, y), Color.White);
                spriteBatch.End();
            }

            spriteBatch.Begin();
            string debug = graphics.PreferredBackBufferWidth + "x" + graphics.PreferredBackBufferHeight + "\nScale: " + scale + ", True Scale: " + trueScale + "\n"
                + $"{FPS(gameTime)}FPS \n"
                + "Player: (" + Character.XPosition + ", " + Character.YPosition + ")\n"
                + "Player (Integral): (" + (int)Character.XPosition + ", " + (int)Character.YPosition + ")\n"
                + "Camera: (" + (int)Level.X + ", " + (int)Level.Y + ")\n"
                + "Player Block Position: (" + (int)(Character.XPosition / 16) + ", " + (int)(Character.YPosition / 16) + ")\n"
                + "Player Within Chunk: (" + (int) ((Character.XPosition / 16) % 16) + ", " + (int) ((Character.YPosition / 16) % 16) + ")\n"
                + "Player Chunk: (" + (int)((Character.XPosition / 16) / 16) + ", " + (int)((Character.YPosition / 16) / 16) + ")\n"
                + "Speed: (" + Character.XSpeed + "," + Character.YSpeed + ")\n"
                + "Speed (SMW Units): (" + (int)(Character.XSpeed * 16.0) + "," + (int)(Character.YSpeed * 16.0) + ")\n"
                + $"Called UpdateCollisionTree() {SpriteHandler.UpdateCalls} {(SpriteHandler.UpdateCalls == 1 ? "time" : "times")} this past second\n"
                + $"Sprites in level: {Level.SpriteCount}\n"
                + $"Sprites on screen: {SpriteHandler.SpriteCount}\n";
            debug += SpriteHandler.SpriteLastSpawned != null ? $"Last sprite spawned: ({SpriteHandler.SpriteLastSpawned})\n" : "";
            debug += SpriteHandler.SpriteLastDepawned != null ? $"Last sprite disposed: ({SpriteHandler.SpriteLastDepawned})\n" : "";
            spriteBatch.DrawString(debugFont, debug, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle)
        {
            Color[] color = new Color[rectangle.Width * rectangle.Height];
            for (int x = 0; x < rectangle.Width; x++)
                for (int y = 0; y < rectangle.Height; y++)
                {
                    int place1 = x + y * rectangle.Width;
                    int place2 = x + rectangle.X + ((y + rectangle.Y) * width);
                    color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
                }
            return color;
        }

        #region Drawing methods
        public Texture2D CreateRectangleTexture(Rectangle rect, Color color)
        {
            return CreateRectangleTexture(rect, color, color);
        }

        public Texture2D CreateRectangleTexture(Rectangle rect)
        {
            return CreateRectangleTexture(rect, Color.Black, Color.Black);
        }

        public Texture2D CreateRectangleTexture(Rectangle rect, Color solid, Color outline)
        {
            Color[] colColor = new Color[rect.Width * rect.Height];
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    if (i == 0 || j == 0 || i == rect.Width - 1 || j == rect.Height - 1)
                        colColor[(j * rect.Width) + i] = outline;
                    else
                        colColor[(j * rect.Width) + i] = solid;
                }
            }
            Texture2D box = new Texture2D(GraphicsDevice, rect.Width, rect.Height);
            box.SetData(colColor);
            return box;
        }

        public Texture2D CreateCrossTexture(Color c)
        {
            Color[] colColor = new Color[3 * 3];            //Resolution: 3x3
            for (int i = 0; i < colColor.Length; i++)
            {
                if (i % 2 == 1 || i == 4)
                    colColor[i] = c;
                else
                    colColor[i] = Color.Transparent;
            }
            Texture2D t = new Texture2D(GraphicsDevice, 3, 3);
            t.SetData(colColor);
            return t;
        }
        #endregion

        public SpriteBatch GetSpriteBatch()
        {
            return spriteBatch;
        }
    }
}