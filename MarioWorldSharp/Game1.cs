using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using MonoGame;

namespace MarioWorldSharp
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 

    public struct ColorDisplacement
    {
        public short Red;
        public short Green;
        public short Blue;
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        RenderTarget2D gamescreen;
        SpriteBatch spriteBatch;
        SpriteFont debugFont;
        Texture2D[] map16Textures;
        Player Mario;
        Level level;
        bool resChange;
        float scale;
        int gameMode;
        const int ResWidth = 400;
        const float ResWidthF = 400F;
        const int ResHeight = 224;
        const float ResHeightF = 224F;
        ColorDisplacement colorDisp;
        const bool drawCollision = true;

        public Game1()
        {
            scale = 1f;
            resChange = false;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int) (ResWidth * scale);
            graphics.PreferredBackBufferHeight = (int) (ResHeight * scale);
            Content.RootDirectory = "Content";
            gameMode = 0;
            colorDisp = new ColorDisplacement();
            colorDisp.Red = 0;
            colorDisp.Green = 0;
            colorDisp.Blue = 0;
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
            level = new Level();
            Mario = new Player();
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
            Texture2D[] smallMarioPoses;
            Texture2D map16_page1;
            Texture2D map16_page2;
            smallMarioPoses = new Texture2D[70];
            smallMarioPoses[0] = Content.Load<Texture2D>("assets\\image\\player\\small\\idle");
            smallMarioPoses[1] = Content.Load<Texture2D>("assets\\image\\player\\small\\run");
            smallMarioPoses[2] = smallMarioPoses[1];
            smallMarioPoses[3] = Content.Load<Texture2D>("assets\\image\\player\\small\\lookup");
            smallMarioPoses[4] = Content.Load<Texture2D>("assets\\image\\player\\small\\fastrun1");
            smallMarioPoses[5] = Content.Load<Texture2D>("assets\\image\\player\\small\\fastrun2");
            smallMarioPoses[6] = smallMarioPoses[5];
            smallMarioPoses[0x0B] = Content.Load<Texture2D>("assets\\image\\player\\small\\jump");
            smallMarioPoses[0x0C] = Content.Load<Texture2D>("assets\\image\\player\\small\\fastjump");
            smallMarioPoses[0x0D] = Content.Load<Texture2D>("assets\\image\\player\\small\\skid");
            smallMarioPoses[0x0F] = Content.Load<Texture2D>("assets\\image\\player\\small\\forward");
            smallMarioPoses[0x24] = Content.Load<Texture2D>("assets\\image\\player\\small\\fall");
            smallMarioPoses[0x39] = Content.Load<Texture2D>("assets\\image\\player\\small\\backward");
            smallMarioPoses[0x3C] = Content.Load<Texture2D>("assets\\image\\player\\small\\duck");
            Mario.Poses = smallMarioPoses;
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
            LevelGameMode();
            base.Update(gameTime);
        }

        protected void LevelGameMode()
        {
            Mario.Process();
            level.Scroll(Mario.xPos, Mario.yPos);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var key = Keyboard.GetState();
            //Resolution changing
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

            GraphicsDevice.SetRenderTarget(gamescreen);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            Level layer = level;
            while (layer != null)
            {
                short[,] leveldraw = layer.GetCameraSelection();
                for (int i = 0; i < leveldraw.GetLength(0); i++)
                {
                    for (int j = 0; j < leveldraw.GetLength(1); j++)
                    {
                        spriteBatch.Draw(map16Textures[leveldraw[i, j]], new Vector2((int)-level.X % 16 + (i * 16), (int)-level.Y % 16 + (j * 16)), Color.White);
                    }
                }
                layer = layer.GetNextLayer();
            }
            Texture2D box;
            if (drawCollision)
                box = createRectangleTexture(Mario.GetCollisionBox(), new Color(64, 0, 0, 100), new Color(255, 45, 45, 100));
            spriteBatch.Draw(Mario.GetTexture(),
                new Rectangle((int)Mario.GetX() - (int)level.X, (int) Mario.GetY() + Mario.yDrawDisp - (int)level.Y, Mario.GetTexture().Width, Mario.GetTexture().Height),
                new Rectangle(0, 0, Mario.GetTexture().Width, Mario.GetTexture().Height), 
                Color.White, 0.0F, Vector2.Zero, Mario.GetSpriteEffect(), 1F);
            if (drawCollision)
                spriteBatch.Draw(box, new Vector2((int)Mario.GetCollisionBox().X - (int)level.X, (int)Mario.GetCollisionBox().Y - (int)level.Y), Color.White);
            spriteBatch.End();
            if (drawCollision)
                box.Dispose();

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
                + "Player: (" + Mario.xPos + ", " + Mario.yPos + ")\n"
                + "Camera: (" + (int)level.X + ", " + (int)level.Y + ")\n"
                + "Player Block Position: (" + (int)(Mario.xPos / 16) + ", " + (int)(Mario.yPos / 16) + ")\n"
                + "Player Within Chunk: (" + (int) ((Mario.xPos / 16) % 16) + ", " + (int) ((Mario.yPos / 16) % 16) + ")\n"
                + "Player Chunk: (" + (int)((Mario.xPos / 16) / 16) + ", " + (int)((Mario.yPos / 16) / 16) + ")\n"
                + "Speed: (" + Mario.xSpeed + "," + Mario.ySpeed + ")\n"
                ;
            spriteBatch.DrawString(debugFont, debug, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle)
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

        public Texture2D createRectangleTexture(Rectangle rect, Color color)
        {
            return createRectangleTexture(rect, color, color);
        }

        public Texture2D createRectangleTexture(Rectangle rect)
        {
            return createRectangleTexture(rect, Color.Black, Color.Black);
        }

        public Texture2D createRectangleTexture(Rectangle rect, Color solid, Color outline)
        {
            Color[] colColor = new Color[rect.Width * rect.Height];
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    if (i == 0 || j == 0 || i == rect.Width - 1 || j == rect.Height - 1)
                        colColor[(j * rect.Width) + i] = new Color(255, 45, 45, 100);
                    else
                        colColor[(j * rect.Width) + i] = new Color(64, 0, 0, 100);
                }
            }
            Texture2D box = new Texture2D(GraphicsDevice, rect.Width, rect.Height);
            box.SetData(colColor);
            return box;
        }

        public SpriteBatch GetSpriteBatch()
        {
            return spriteBatch;
        }
    }
}