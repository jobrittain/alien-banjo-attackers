#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Xml;
using System.Reflection;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using ABAFS.Sprites;
using ABAFS.Screen;
#endregion

namespace ABAFS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Playfield playfield;
        SaveManager saveManager;
        ScreenSystem screenSys;
        Menu menu;

        // Textures
        Texture2D mainTitleTexture;
        Texture2D gameOverTitleTexture;
        Texture2D[] accordianTexture = new Texture2D[2];
        Texture2D lifeTexture;
        Texture2D[] banjoTexture = new Texture2D[3];
        Texture2D[] noteTexture = new Texture2D[3];
        Texture2D[] explosionTexture = new Texture2D[13];
        Texture2D pixelTexture;
        Texture2D backgroundTexture;
        Texture2D[] buttonTexture = new Texture2D[2];

        // Rectangles
        Rectangle backgroundRectangle;

        // Fonts
        SpriteFont debugFont;
        SpriteFont tinyFont;
        SpriteFont smallFont;
        SpriteFont mediumFont;
        SpriteFont largeFont;

        // Music
        SoundEffect startMusic;
        SoundEffect gameOverMusicNormal;
        SoundEffect gameOverMuiscHighScore;

        // Sounds
        SoundEffect menuSelectSound;
        SoundEffect menuMoveSound;
        SoundEffect accordianFireSound;
        SoundEffect banjoDestroySound;

        // Default spawn locations
        Vector2 accordianSpawnLoc;
        Vector2[] banjoSpawnLoc = new Vector2[5];

        // Text positions
        Vector2 debugTextPos;
        Vector2 fpsTextPos;

        Vector2 scoreTextPos;
        Vector2 livesTextPos;

        Vector2 startTitlePos;
        Vector2 menuTitlePos;
        Vector2 menuPos;
        Vector2 startTextPos;
        Vector2 highScoreTextPos;
        Vector2 gameOverTextPos;
        Vector2 gameOverInfoTextPos;
        Vector2 backTextPos;

        // Life icon positions
        Vector2[] lifePos;

        /// <summary>
        /// Game constructor.
        /// </summary>
        public MainGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialization logic
            graphics.PreferMultiSampling = false;

            debugMode = false;
            menuActive = false;
            gameActive = false;
            gamePaused = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Set spawn locations
            accordianSpawnLoc = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - 80);
            banjoSpawnLoc[0] = new Vector2(10, 10);
            banjoSpawnLoc[1] = new Vector2(220, 50);
            banjoSpawnLoc[2] = new Vector2(390, 35);
            banjoSpawnLoc[3] = new Vector2(620, 20);
            banjoSpawnLoc[4] = new Vector2(750, 10);

            // Create a new SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Set background rectangle
            backgroundRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Load textures
            mainTitleTexture = Content.Load<Texture2D>("textures\\screen\\title");
            gameOverTitleTexture = Content.Load<Texture2D>("textures\\screen\\gameovertitle");
            buttonTexture[0] = Content.Load<Texture2D>("textures\\screen\\button");
            buttonTexture[1] = Content.Load<Texture2D>("textures\\screen\\buttonh");
            accordianTexture[0] = Content.Load<Texture2D>("textures\\accordian\\accordian");
            accordianTexture[1] = Content.Load<Texture2D>("textures\\accordian\\accordiang");
            lifeTexture = Content.Load<Texture2D>("textures\\screen\\accordian2");
            banjoTexture[0] = Content.Load<Texture2D>("textures\\banjo\\banjo1");
            banjoTexture[1] = Content.Load<Texture2D>("textures\\banjo\\banjo2");
            banjoTexture[2] = Content.Load<Texture2D>("textures\\banjo\\banjo3");
            noteTexture[0] = Content.Load<Texture2D>("textures\\note\\note1");
            noteTexture[1] = Content.Load<Texture2D>("textures\\note\\note2");
            noteTexture[2] = Content.Load<Texture2D>("textures\\note\\note3");
            for (int i = 0; i < explosionTexture.Length; i++)
            {
                explosionTexture[i] = Content.Load<Texture2D>("textures\\explosion\\explosion" + (i+1));
            }
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            backgroundTexture = Content.Load<Texture2D>("textures\\screen\\background");
            pixelTexture.SetData(new[] { Color.White });

            // Load fonts
            debugFont = Content.Load<SpriteFont>("spritefonts\\font1");
            tinyFont = Content.Load<SpriteFont>("spritefonts\\font2");
            smallFont = Content.Load<SpriteFont>("spritefonts\\font5");
            mediumFont = Content.Load<SpriteFont>("spritefonts\\font3");
            largeFont = Content.Load<SpriteFont>("spritefonts\\font4");

            // Set positions
            float titleCenterPointX = UsefulFunctions.GetTextureCenterPoint(graphics.GraphicsDevice.Viewport.Bounds, Vector2.Zero, mainTitleTexture, UsefulFunctions.CenterPointType.X);
            startTitlePos = new Vector2(titleCenterPointX, 100);
            menuTitlePos = new Vector2(titleCenterPointX, 10);

            menuPos = new Vector2(UsefulFunctions.GetTextureCenterPoint(graphics.GraphicsDevice.Viewport.Bounds, Vector2.Zero, buttonTexture[0], UsefulFunctions.CenterPointType.X), 200);
            debugTextPos = new Vector2(1, 1);
            fpsTextPos = new Vector2(725, 1);
            scoreTextPos = new Vector2(1, 430);
            livesTextPos = new Vector2(110, 430);
            startTextPos = new Vector2(UsefulFunctions.GetSpriteFontCenterPoint(graphics.GraphicsDevice.Viewport.Bounds, Vector2.Zero, tinyFont, "Press enter to play", UsefulFunctions.CenterPointType.X), 300);
            highScoreTextPos = new Vector2(580, 1);
            gameOverTextPos = new Vector2(255, 100);
            gameOverInfoTextPos = new Vector2(305, 210);
            backTextPos = new Vector2(200, 280);

            lifePos = new Vector2[3];
            lifePos[0] = new Vector2(110, 453);
            lifePos[1] = new Vector2(140, 453);
            lifePos[2] = new Vector2(170, 453);
            
            // Load music
            startMusic = Content.Load<SoundEffect>("sound\\music1");
            gameOverMusicNormal = Content.Load<SoundEffect>("sound\\music2");
            gameOverMuiscHighScore = Content.Load<SoundEffect>("sound\\music3");

            // Load sound
            menuSelectSound = Content.Load<SoundEffect>("sound\\sound1");
            menuMoveSound = Content.Load<SoundEffect>("sound\\sound2");
            accordianFireSound = Content.Load<SoundEffect>("sound\\sound3");
            banjoDestroySound = Content.Load<SoundEffect>("sound\\sound4");

            // Setup playfield
            playfield = new Playfield(accordianTexture, noteTexture, banjoTexture, explosionTexture, pixelTexture, 
                accordianSpawnLoc, banjoSpawnLoc, 
                accordianFireSound, banjoDestroySound, 999);

            // Add player
            playfield.AddPlayer(accordianSpawnLoc, 3, 0);

            saveManager = new SaveManager("savegame.xml", playfield);
            //saveManager.Load();

            menu = new Menu(menuPos, 60f, buttonTexture, smallFont, menuMoveSound, menuSelectSound);
            menu.AddItem("New Game");
            menu.AddItem("Continue", !saveManager.Load());
            menu.AddItem("Exit");

            screenSys = new ScreenSystem(playfield, menu,
                startMusic, gameOverMusicNormal, gameOverMuiscHighScore, // Music
                menuSelectSound,                                         // Selection Sound
                mainTitleTexture, startTitlePos, menuTitlePos,           // Title
                tinyFont, startTextPos,                                 // Start text
                tinyFont, highScoreTextPos,                             // High score
                gameOverTitleTexture, gameOverTextPos,                  // Game over text
                tinyFont, gameOverInfoTextPos,                          // Game over info (score etc.)
                tinyFont, backTextPos);                                 // Return to start text

            Debug.WriteLine(GraphicsDeviceManager.DefaultBackBufferWidth + " x " + GraphicsDeviceManager.DefaultBackBufferHeight);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        // Important game flags
        bool debugMode;
        bool menuActive;
        bool gameActive;
        bool gamePaused;
        bool exitTriggered;

        bool highScore;

        bool autoSaved = false;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Global controls
            CheckMasterControls(gameTime);

            if (screenSys.Active == true)
            {
                screenSys.Update(gameTime, ref gameActive, ref exitTriggered, ref autoSaved, highScore);
            }

            // Update logic
            if (gameActive == true && gamePaused == false)
            {
                playfield.Update(gameTime, GraphicsDevice, debugMode);
            }

            if (playfield.GameOver == true)
            {
                screenSys.Active = true;
                if (autoSaved == false)
                {
                    if (playfield.Player.Score.Value > playfield.HighScore.Value)
                    {
                        highScore = true;
                        playfield.HighScore.Value = playfield.Player.Score.Value;
                    }
                    else
                    {
                        highScore = false;
                    }
                    saveManager.Save(false);
                    menu.MenuItems[1].Blank = true;
                    autoSaved = true;
                }
            }

            if (exitTriggered == true)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        bool _yReleased;
        bool _startReleased;
        bool _bigButtonReleased;
        bool _rightSButtonReleased;

        void CheckMasterControls(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (debugMode == true)
                {
                    if (_startReleased == true)
                    {
                        if (gamePaused == false)
                        {
                            gamePaused = true;
                        }
                        else
                        {
                            gamePaused = false;
                        }
                        _startReleased = false;
                    }
                }
            }
            else
            {
                _startReleased = true;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (_bigButtonReleased == true)
                {
                    if (gameActive == true)
                    {
                        gamePaused = true;
                        saveManager.Save(true);
                        gamePaused = false;
                    }
                    _bigButtonReleased = false;
                }
            }
            else
            {
                _bigButtonReleased = true;
            }

#if DEBUG
            if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                if (_yReleased == true)
                {
                    if (debugMode == false)
                    {
                        debugMode = true;
                    }
                    else
                    {
                        debugMode = false;
                    }
                    _yReleased = false;
                }
            }
            else
            {
                _yReleased = true;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                if (_rightSButtonReleased == true)
                {
                    playfield.Player.Hit = true;
                    _rightSButtonReleased = false;
                }
            }
            else
            {
                _rightSButtonReleased = true;
            }
#endif
        }

        double frameRate = 0;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            DrawScene(spriteBatch);

            if (screenSys.Active == true)
            {
                screenSys.Draw(spriteBatch);
            }

            if (gameActive == true)
            {
                DrawGameInfo();
                DrawPlayerObjects();
                DrawBanjoObjects();
            }

            if (debugMode == true)
            {
                DrawDebug(gameTime);
            }

            spriteBatch.End();
            frameRate = 1 / gameTime.ElapsedGameTime.TotalSeconds;
            base.Draw(gameTime);
        }

        void DrawScene(SpriteBatch spriteBatch)
        {
            // Draw background
            spriteBatch.Draw(backgroundTexture, backgroundRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
        void DrawGameInfo()
        {
            if (playfield.GameOver == false)
            {
                spriteBatch.DrawString(tinyFont, "Score\n" + playfield.Player.Score.ToString(), scoreTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
                spriteBatch.DrawString(tinyFont, "Lives", livesTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
                for (int life = 0; life < playfield.Player.Lives; life++)
                {
                    spriteBatch.Draw(lifeTexture, lifePos[life], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
                }
            }
        }
        void DrawPlayerObjects()
        {
            playfield.Player.Draw(spriteBatch);
            foreach (Note note in playfield.Notes)
            {
                if (note.Expired == false)
                {
                    note.Draw(spriteBatch);
                }
            }
        }
        void DrawBanjoObjects()
        {
            foreach (Banjo banjo in playfield.Banjos)
            {
                if (banjo.Expired == false)
                {
                    banjo.Draw(spriteBatch);
                }
            }
        }
        void DrawDebug(GameTime gameTime)
        {
            spriteBatch.DrawString(debugFont, "Debug Mode", debugTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(debugFont, "FPS " + ((int)frameRate).ToString(), fpsTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            if (playfield.CollisionDetector.NearBoxDrawTime > 0)
            {
                playfield.CollisionDetector.NearBox.Draw(spriteBatch, 0.992f);
                playfield.CollisionDetector.NearBoxDrawTime -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (playfield.CollisionDetector.IntersectBoxDrawTime > 0)
            {
                playfield.CollisionDetector.IntersectBox.Draw(spriteBatch, 0.993f);
                playfield.CollisionDetector.IntersectBoxDrawTime -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (playfield.CollisionDetector.TouchBoxDrawTime > 0)
            {
                playfield.CollisionDetector.TouchBox.Draw(spriteBatch, 0.994f);
                playfield.CollisionDetector.TouchBoxDrawTime -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            foreach (Banjo banjo in playfield.Banjos)
            {
                banjo.Box.Draw(spriteBatch, 0.99f);
            }
            foreach (Note note in playfield.Notes)
            {
                note.Box.Draw(spriteBatch, 0.99f);
            }
            foreach (Playfield.SpawnPoint banjoSpawnPoint in playfield.BanjoSpawnPoint)
            {
                banjoSpawnPoint.Box.Draw(spriteBatch, 0.05f);
            }
            playfield.Player.Box.Draw(spriteBatch, 0.99f);
        }
    }
   
}
