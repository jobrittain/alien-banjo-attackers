#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using System.Diagnostics;
using System.Xml;
using System.Reflection;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace ABAFS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Playfield playfield;
        SaveManager saveManager;
        ScreenSystem screenSys;
        Menu menu;

        // Textures
        Texture2D mainTitleTexture;
        Texture2D accordianTexture;
        Texture2D lifeTexture;
        Texture2D[] banjoTexture = new Texture2D[3];
        Texture2D[] noteTexture = new Texture2D[3];
        Texture2D pixelTexture;
        Texture2D backgroundTexture;
        //Texture2D[] starTexture = new Texture2D[2];
        Texture2D[] buttonTexture = new Texture2D[2];

        // Rectangles
        Rectangle backgroundRectangle;

        // Fonts
        SpriteFont debugFont;
        SpriteFont smallFont;
        SpriteFont mediumFont;
        SpriteFont largeFont;

        // Music
        SoundEffect startMusic;
        SoundEffect gameOverMusicNormal;
        SoundEffect gameOverMuiscHighScore;

        // Sounds
        SoundEffect menuSelectSound;

        // Default spawn locations
        Vector2 accordianSpawnLoc;
        Vector2[] banjoSpawnLoc = new Vector2[5];

        // Text positions
        Vector2 debugTextPos;
        Vector2 fpsTextPos;

        Vector2 scoreTextPos;
        Vector2 livesTextPos;

        Vector2 mainTitlePos;
        Vector2 menuPos;
        Vector2 startTextPos;
        Vector2 highScoreTextPos;
        Vector2 gameOverTextPos;
        Vector2 gameOverInfoTextPos;
        Vector2 backTextPos;

        // Life icon positions
        Vector2[] lifePos;

        // Star positions
        Vector2[] starPos;

        public Game1()
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
            gameLost = false;



            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Set spawn locations
            accordianSpawnLoc = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - 75);
            banjoSpawnLoc[0] = new Vector2(10, 10);
            banjoSpawnLoc[1] = new Vector2(220, 50);
            banjoSpawnLoc[2] = new Vector2(390, 35);
            banjoSpawnLoc[3] = new Vector2(620, 20);
            banjoSpawnLoc[4] = new Vector2(750, 10);

            // Star positions
            //starPos = new Vector2[20];
            //starPos[0] = new Vector2(200, 10);
            //starPos[1] = new Vector2(120, 80);
            //starPos[2] = new Vector2(680, 130);
            //starPos[3] = new Vector2(150, 150);
            //starPos[4] = new Vector2(170, 170);
            //starPos[5] = new Vector2(175, 175);
            //starPos[6] = new Vector2(190, 200);
            //starPos[7] = new Vector2(210, 180);
            //starPos[8] = new Vector2(10, 200);
            //starPos[9] = new Vector2(100, 210);
            //starPos[10] = new Vector2(95, 235);
            //starPos[11] = new Vector2(105, 250);
            //starPos[12] = new Vector2(70, 182);

            // Create a new SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Set background rectangle
            backgroundRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Load textures
            mainTitleTexture = Content.Load<Texture2D>("title");
            buttonTexture[0] = Content.Load<Texture2D>("button");
            buttonTexture[1] = Content.Load<Texture2D>("buttonh");
            accordianTexture = Content.Load<Texture2D>("accordian");
            lifeTexture = Content.Load<Texture2D>("accordian2");
            banjoTexture[0] = Content.Load<Texture2D>("banjo1");
            banjoTexture[1] = Content.Load<Texture2D>("banjo2");
            banjoTexture[2] = Content.Load<Texture2D>("banjo3");
            noteTexture[0] = Content.Load<Texture2D>("note1");
            noteTexture[1] = Content.Load<Texture2D>("note2");
            noteTexture[2] = Content.Load<Texture2D>("note3");
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            backgroundTexture = Content.Load<Texture2D>("background");
            //starTexture[0] = Content.Load<Texture2D>("star1");
            //starTexture[1] = Content.Load<Texture2D>("star2");
            pixelTexture.SetData(new[] { Color.White });

            // Load fonts
            debugFont = Content.Load<SpriteFont>("font1");
            smallFont = Content.Load<SpriteFont>("font2");
            mediumFont = Content.Load<SpriteFont>("font3");
            largeFont = Content.Load<SpriteFont>("font4");

            // Set positions
            mainTitlePos = new Vector2(GetTextureCenterPoint(mainTitleTexture, CenterPointType.X), 100);
            menuPos = new Vector2(GetTextureCenterPoint(buttonTexture[0], CenterPointType.X), 300);
            debugTextPos = new Vector2(1, 1);
            fpsTextPos = new Vector2(725, 1);
            scoreTextPos = new Vector2(1, 430);
            livesTextPos = new Vector2(110, 430);
            startTextPos = new Vector2(GetSpriteFontCenterPoint(smallFont,"Press enter to play",CenterPointType.X), 300);
            highScoreTextPos = new Vector2(580, 1);
            gameOverTextPos = new Vector2(270, 150);
            gameOverInfoTextPos = new Vector2(305, 210);
            backTextPos = new Vector2(200, 280);

            lifePos = new Vector2[3];
            lifePos[0] = new Vector2(110, 453);
            lifePos[1] = new Vector2(140, 453);
            lifePos[2] = new Vector2(170, 453);
            
            // Load music
            startMusic = Content.Load<SoundEffect>("music1");
            gameOverMusicNormal = Content.Load<SoundEffect>("music2");
            gameOverMuiscHighScore = Content.Load<SoundEffect>("music3");

            // Load sound
            menuSelectSound = Content.Load<SoundEffect>("sound1");

            // Setup playfield
            playfield = new Playfield(accordianTexture, noteTexture, banjoTexture, pixelTexture, accordianSpawnLoc, banjoSpawnLoc);

            // Add player
            playfield.AddPlayer(accordianSpawnLoc, 3, 0);

            saveManager = new SaveManager("savegame.xml", playfield);
            saveManager.Load();

            menu = new Menu(menuPos, 20f, buttonTexture);

            screenSys = new ScreenSystem(playfield, menu,
                startMusic, gameOverMusicNormal, gameOverMuiscHighScore, // Music
                menuSelectSound,                 // Selection Sound
                mainTitleTexture, mainTitlePos,  // Main title
                smallFont, startTextPos,         // Start text
                smallFont, highScoreTextPos,     // High score
                largeFont, gameOverTextPos,      // Game over text
                smallFont, gameOverInfoTextPos,  // Game over info (score etc.)
                smallFont, backTextPos);         // Return to start text

            Debug.WriteLine(GraphicsDeviceManager.DefaultBackBufferWidth + " x " + GraphicsDeviceManager.DefaultBackBufferHeight);
        }

        enum CenterPointType
        {
            X,
            Y
        }
        float GetTextureCenterPoint(Texture2D texture, CenterPointType pointType)
        {
            float textureCenter;
            if (pointType == CenterPointType.X)
            {
                textureCenter = (graphics.GraphicsDevice.Viewport.Width / 2) - (texture.Width / 2);
            }
            else
            {
                textureCenter = (graphics.GraphicsDevice.Viewport.Height / 2) - (texture.Height / 2);
            }
            return textureCenter;
        }
        float GetSpriteFontCenterPoint(SpriteFont spriteFont, string text, CenterPointType pointType)
        {
            float spriteFontCenter;
            if (pointType == CenterPointType.X)
            {
                spriteFontCenter = (graphics.GraphicsDevice.Viewport.Width / 2) - (spriteFont.MeasureString(text).X / 2);
            }
            else
            {
                spriteFontCenter = (graphics.GraphicsDevice.Viewport.Height / 2) - (spriteFont.MeasureString(text).Y / 2);
            }
            return spriteFontCenter;
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
        bool gameLost;
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
                    saveManager.Save(false);
                    autoSaved = true;
                }
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.BigButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.S))
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


            // DEBUG
#if DEBUG
            if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F1))
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
                    playfield.Player.Expired = true;
                    _rightSButtonReleased = false;
                }
            }
            else
            {
                _rightSButtonReleased = true;
            }
#endif
        }

        // old managers
        //void ManagePlayer(GameTime gameTime)
        //{
        //    player.Update(gameTime, graphics.GraphicsDevice);
        //    foreach (Note note in player.notes)
        //    {
        //        if (note.isVisible(graphics.GraphicsDevice) == false || note.Expired == true)
        //        {
        //            garbageNotes.Add(player.notes.IndexOf(note));
        //        }
        //        else
        //        {
        //            note.Update();
        //        }
        //    }
        //}

        //void ManageBanjos(GameTime gameTime)
        //{
        //    if (spawnTime >= 1)
        //    {
        //        spawnTime = 0;
        //        if (banjos.Count < 2)
        //        {
        //            banjos.Add(new Banjo(Banjo.BanjoType.Normal, banjoTexture, banjoColorData, banjoSpawnLoc, gameTime));
        //        }
        //    }
        //    foreach (Banjo banjo in banjos)
        //    {
        //        if (banjo.isVisible(graphics.GraphicsDevice) == false || banjo.Expired == true)
        //        {
        //            garbageBanjos.Add(banjos.IndexOf(banjo));
        //        }
        //        else
        //        {
        //            banjo.Update(graphics.GraphicsDevice, gameTime, player);
        //        }
        //    }
        //}


        //void ManagePlayer(GameTime gameTime)
        //{

        //    playfield.Player.Update(gameTime, GraphicsDevice, ref playfield);
        //    foreach (Note note in playfield.Notes)
        //    {
        //        if (note.IsVisible(GraphicsDevice) == false || note.Expired == true)
        //        {
        //            playfield.GarbageNotes.Add(playfield.Notes.IndexOf(note));
        //        }
        //        else
        //        {
        //            note.Update();
        //        }
        //    }
        //}
        //void ManageBanjos(GameTime gameTime)
        //{
        //    if (spawnTime >= 1)
        //    {
        //        spawnTime = 0;
        //        if (playfield.Banjos.Count < 50)
        //        {
        //            //playfield.Banjos.Add(new Banjo(Banjo.BanjoType.Normal, banjoTexture, banjoColorData, banjoSpawnLoc, gameTime));
        //            playfield.AddBanjo(banjoSpawnLoc[randSpanwnPoint.Next(0,5)], gameTime, Banjo.BanjoType.Normal);
        //        }
        //    }
        //    foreach (Banjo banjo in playfield.Banjos)
        //    {
        //        if (banjo.IsVisible(GraphicsDevice) == false || banjo.Expired == true)
        //        {
        //            playfield.GarbageBanjos.Add(playfield.Banjos.IndexOf(banjo));
        //        }
        //        else
        //        {
        //            banjo.Update(GraphicsDevice, gameTime, playfield.Player);

        //        }
        //    }
        //}

        //void ClearGarbage()
        //{
        //    foreach (int noteIndex in playfield.GarbageNotes)
        //    {
        //        //playfield.Notes[noteIndex] = null;
        //        playfield.Notes.Remove(playfield.Notes[noteIndex]);
        //    }
        //    playfield.GarbageNotes.Clear();
        //    foreach (int banjoIndex in playfield.GarbageBanjos)
        //    {
        //        try
        //        {
        //            //playfield.Banjos[banjoIndex] = null;
        //            playfield.Banjos.Remove(playfield.Banjos[banjoIndex]);
        //        }
        //        catch
        //        {
        //            Debug.WriteLine("Could not remove banjo at index " + banjoIndex + ".");
        //        }
        //    }
        //    playfield.GarbageBanjos.Clear();
        //}

        //void DetectCollisions()
        //{
        //    foreach (Banjo banjo in playfield.Banjos)
        //    {
        //        foreach (Note note in playfield.Notes)
        //        {
        //            if (IsNear(banjo.Location,note.Location))
        //            {
        //                nearBox.X = banjo.Box.X - nbMaxX;
        //                nearBox.Y = banjo.Box.Y - nbMaxY;
        //                nearBox.Width = nbMaxX - nbMinX;
        //                nearBox.Height = nbMaxY - nbMinY;
        //                nearBox.Update();
        //                drawIsNearBoxFrames = 60;

        //                if (banjo.Box.Intersects(note.Box))
        //                {
        //                    intersectBox.Rectangle = banjo.Box;
        //                    intersectBox.Update();
        //                    drawIntersectBoxFrames = 60;

        //                    if (IsTouching(banjo.Box, note.Box, banjo.TextureColorData, note.TextureColorData))
        //                    {
        //                        banjo.Expired = true;
        //                        note.Expired = true;
        //                    }
        //                }
        //            }
        //        }
        //        if (IsNear(banjo.Location, playfield.Player.Location))
        //        {
        //            if (banjo.Box.Intersects(playfield.Player.Box))
        //            {
        //                if (IsTouching(banjo.Box, playfield.Player.Box, banjo.TextureColorData, playfield.Player.TextureColorData))
        //                {
        //                    banjo.Expired = true;
        //                    playfield.Player.Expired = true;
        //                }
        //            }
        //        }
        //    }
        //}

        //// Near box (bounds) for being near a banjo
        //int nbMaxX = 20;
        //int nbMinX = -60;
        //int nbMaxY = 20;
        //int nbMinY = -90;

        //bool IsNear(Vector2 LocationA, Vector2 LocationB)
        //{
        //    float xDiff = LocationA.X - LocationB.X;
        //    float yDiff = LocationA.Y - LocationB.Y;
        //    if (xDiff < nbMaxX && xDiff > nbMinX)
        //    {
        //        if (yDiff < nbMaxY && yDiff > nbMinY)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        // Number of frames to draw the debug boxes for

        //bool IsTouching(Rectangle rectangleA, Rectangle rectangleB, Color[] colorsA, Color[] colorsB)
        //{
        //    int top = Math.Max(rectangleA.Top, rectangleB.Top);
        //    int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
        //    int left = Math.Max(rectangleA.Left, rectangleB.Left);
        //    int right = Math.Min(rectangleA.Right, rectangleB.Right);

        //    touchBox.X = left;
        //    touchBox.Y = top;
        //    touchBox.Width = right - left;
        //    touchBox.Height = bottom - top;
        //    touchBox.Update();
        //    drawTouchBoxFrames = 60;

        //    for (int yPix = top; yPix < bottom; yPix++)
        //    {
        //        for (int xPix = left; xPix < right; xPix++)
        //        {
        //            Color colorBanjo = colorsA[(xPix - rectangleA.Left) + ((yPix - rectangleA.Top) * rectangleA.Width)];
        //            Color colorNote = colorsB[(xPix - rectangleB.Left) + ((yPix - rectangleB.Top) * rectangleB.Width)];

        //            if (colorBanjo.A != 0 && colorNote.A != 0)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

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

            // Draw stars
            //spriteBatch.Draw(starTexture[0], starPos[0], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[1], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[2], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[3], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[4], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[5], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[6], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[7], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[8], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[9], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[10], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[11], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(starTexture[0], starPos[12], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);

        }
        void DrawGameInfo()
        {
            if (playfield.GameOver == false)
            {
                spriteBatch.DrawString(smallFont, "Score\n" + playfield.Player.Score.ToString(), scoreTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
                spriteBatch.DrawString(smallFont, "Lives", livesTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
                for (int life = 0; life < playfield.Player.Lives; life++)
                {
                    spriteBatch.Draw(lifeTexture, lifePos[life], null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
                }
            }
            else
            {

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
                playfield.CollisionDetector.NearBox.Draw(spriteBatch, 0.99f);
                playfield.CollisionDetector.NearBoxDrawTime -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (playfield.CollisionDetector.IntersectBoxDrawTime > 0)
            {
                playfield.CollisionDetector.IntersectBox.Draw(spriteBatch, 0.991f);
                playfield.CollisionDetector.IntersectBoxDrawTime -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (playfield.CollisionDetector.TouchBoxDrawTime > 0)
            {
                playfield.CollisionDetector.TouchBox.Draw(spriteBatch, 0.992f);
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

    //old menu class
    //public class Menu
    //{
    //    public class MenuItem
    //    {
    //        public int Index;
    //        public string Text;

    //        public MenuItem(string text, int index, Vector2 location, Action task)
    //        {

    //        }
    //    }

    //    public int selectedItemIndex;
    //    public List<MenuItem> MenuItems;

    //    public Menu(Texture2D buttonTexture, Texture2D buttonSelectTexture)
    //    {

    //        MenuItems = new List<MenuItem>();
    //    }

    //    public void AddMenuItem()
    //    {

    //    }

    //}
    public class ScreenSystem
    {

        public enum MenuMode
        {
            Start,
            Main,
            Gameover
        }

        public MenuMode Mode;
        public bool Active = true;

        enum GameoverMusicType
        {
            Normal,
            HighScore
        }

        GameoverMusicType _gameOverMusicType;

        Playfield _playfield;

        Texture2D _mainTitleTexture;

        SpriteFont _startTextFont;
        SpriteFont _highScoreFont;
        SpriteFont _gameOverTextFont;
        SpriteFont _gameOverInfoFont;
        SpriteFont _backTextFont;

        SoundEffectInstance _mainTitleMusic;
        SoundEffectInstance _gameOverMusic;
        SoundEffectInstance _gameOverHighScoreMusic;
        SoundEffectInstance _menuSelectSound; 

        Vector2 _mainTitlePosition;
        Vector2 _startTextPosition;
        Vector2 _highScorePosition;
        Vector2 _gameOverTextPosition;
        Vector2 _gameOverInfoPosition;
        Vector2 _backTextPosition;

        bool _startReleased = true;
        bool _returnToMenuEnabled = false;
        bool _updatedScores = false;
        bool _returnButtonDown = false;
        bool _startButtonDown = false;
        bool _musicStarted = false;
        string _startMessage;
        string _backMessage;
        string _highScoreText;
        string _gameOverInfoScoreText;
        string _gameOverInfoHighScoreText;
        double _gameOverTime;
        double _shineRespawnTime;
        int _backWaitTime;
        int _playerScore;
        int _highScore;
        
        float _alphaVal = 1f;
        float _alphaSinInput = 0f;


        public ScreenSystem(Playfield playfield, Menu menu,
            SoundEffect mainTitleMusic, SoundEffect gameOverMusic, SoundEffect gameOverHighScoreMusic,
            SoundEffect menuSelectSound,
            Texture2D mainTitleTexture, Vector2 mainTitlePosition,
            SpriteFont startTextFont, Vector2 startTextPosition,
            SpriteFont highScoreFont, Vector2 highScorePosition,
            SpriteFont gameOverTextFont, Vector2 gameOverTextPosition,
            SpriteFont gameOverInfoFont, Vector2 gameOverInfoPosition,
            SpriteFont backTextFont, Vector2 backTextPosition
            )
        {
            _playfield = playfield;

            _mainTitleTexture = mainTitleTexture;

            _startTextFont = startTextFont;
            _highScoreFont = highScoreFont;
            _gameOverTextFont = gameOverTextFont;
            _gameOverInfoFont = gameOverInfoFont;
            _backTextFont = backTextFont;

            _mainTitleMusic = mainTitleMusic.CreateInstance();
            _mainTitleMusic.IsLooped = true;
            _gameOverMusic = gameOverMusic.CreateInstance();
            _gameOverMusic.IsLooped = true;
            _gameOverHighScoreMusic = gameOverHighScoreMusic.CreateInstance();
            _gameOverHighScoreMusic.IsLooped = true;
            _menuSelectSound = menuSelectSound.CreateInstance();

            _mainTitlePosition = mainTitlePosition;
            _startTextPosition = startTextPosition;
            _highScorePosition = highScorePosition;
            _gameOverTextPosition = gameOverTextPosition;
            _gameOverInfoPosition = gameOverInfoPosition;
            _backTextPosition = backTextPosition;

            _highScoreText = "Highscore " + playfield.HighScore.ToString();
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                _startMessage = "Press start to play";
                _backMessage = "Press start to return and play again \n"
                              + "        Press back to exit";
            }
            else
            {
                _startMessage = "Press enter to play";
                _backMessage = "Press enter to return and play again \n"
                              + "      Press backspace to exit";
            }
        }

        public void Update(GameTime gameTime, ref bool gameActive, ref bool exitTriggered, ref bool autoSaved, bool newHighScore)
        {
            // Scores
            if (_updatedScores == false)
            {
                if (newHighScore == true)
                {
                    _highScoreText = "Highscore " + _playfield.Player.Score.ToString();
                    _gameOverInfoHighScoreText = "\nNew High Score!";
                }
                else
                {
                    _gameOverInfoHighScoreText = "\nHigh Score " + _playfield.HighScore.ToString();
                }
                _updatedScores = true;
            }

            // Music
            if (_musicStarted == false)
            {
                if (Mode == MenuMode.Main)
                {
                    _mainTitleMusic.Play();
                }
                else
                {
                    if (newHighScore == true)
                    {
                        _gameOverMusicType = GameoverMusicType.HighScore;
                        _gameOverHighScoreMusic.Play();
                    }
                    else
                    {
                        _gameOverMusicType = GameoverMusicType.Normal;
                        _gameOverMusic.Play();
                    }
                }
                _musicStarted = true;
            }

            if (Mode == MenuMode.Main)
            {
                _alphaVal = GetAlphaVal(_alphaSinInput);
                _alphaSinInput += 0.01f;

                if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (_returnButtonDown == false)
                    {
                        _menuSelectSound.Play();
                        _updatedScores = false;
                        _playfield.Reset();
                        _mainTitleMusic.Stop();
                        _musicStarted = false;

                        autoSaved = false;
                        gameActive = true;
                        Active = false;
                        Mode = MenuMode.Gameover;

                        _returnButtonDown = true;
                    }
                }
                else
                {
                    _returnButtonDown = false;
                }

            }
            else
            {
                if (_gameOverTime > _backWaitTime)
                {
                    _returnToMenuEnabled = true;
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        if (_returnButtonDown == false)
                        {
                            if (_gameOverMusicType == GameoverMusicType.HighScore)
                            {
                                _gameOverHighScoreMusic.Stop();
                            }
                            else
                            {
                                _gameOverMusic.Stop();
                            }
                            _musicStarted = false;
                            Mode = MenuMode.Main;

                            _returnButtonDown = true;
                        }
                    }
                    else if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Back))
                    {
                        exitTriggered = true;
                    }
                    else
                    {
                        _returnButtonDown = false;
                    }
                }
                _gameOverInfoScoreText = "Score      " + _playfield.Player.Score.ToString();

                _gameOverTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private float GetAlphaVal(float val, double phase = 0)
        {
            return 1 - ((float)Math.Pow(Math.Sin(((val + phase) * 1.7)), 2) * 0.8f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Mode == MenuMode.Start)
            {
                spriteBatch.Draw(_mainTitleTexture, _mainTitlePosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(_startTextFont, _startMessage, _startTextPosition, Color.White * _alphaVal, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(_highScoreFont, _highScoreText, _highScorePosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                //spriteBatch.DrawString(_highScoreFont, _alphaVal.ToString(), _highScorePosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            else if (Mode == MenuMode.Main)
            {
                spriteBatch.Draw(_mainTitleTexture, _mainTitlePosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            else
            {
                spriteBatch.DrawString(_gameOverTextFont, "Game Over", _gameOverTextPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(_gameOverInfoFont, _gameOverInfoScoreText + _gameOverInfoHighScoreText, _gameOverInfoPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                
                if (_gameOverTime > _backWaitTime)
                {
                    spriteBatch.DrawString(_backTextFont, _backMessage, _backTextPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            }
        }
    }

    public class Menu
    {
        public class MenuItem
        {
            public Vector2 Position;
            public bool Blank;

            string _text;
            

            public MenuItem(bool blank, string text, Vector2 position)
            {
                Blank = blank;
                Position = position;
                _text = text;
            }

        }

        enum Direction
        {
            Up, Down
        }

        public Vector2 Location;
        public int SelectedButtonIndex;

        public List<MenuItem> MenuItems;

        Texture2D[] _buttonTexture;

        float _spacing;
        bool _selectedItemChanged;

        public Menu(Vector2 location, float spacing, Texture2D[] buttonTexture)
        {
            MenuItems = new List<MenuItem>();
            Location = location;
            _spacing = spacing;
            _buttonTexture = buttonTexture;
        }

        public void AddItem(int index, string buttonText, bool blank = false)
        {
            Vector2 buttonPosition;
            if (index != 0)
            {
                buttonPosition = new Vector2(Location.X, MenuItems[index - 1].Position.Y + _spacing);
            }
            else
            {
                buttonPosition = new Vector2(Location.X, Location.Y);
            }

            MenuItems.Add(new MenuItem(blank, buttonText, buttonPosition));
        }

        public void Update()
        {
            // Up
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -0.8f && _selectedItemChanged == false)
            {
                Move(Direction.Down);
                _selectedItemChanged = true;
            }
            else if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > -0.2f)
            {
                _selectedItemChanged = false;
            }

            // Down
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0.8f && _selectedItemChanged == false)
            {
                Move(Direction.Up);
                _selectedItemChanged = true;
            }
            else if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0.2f)
            {
                _selectedItemChanged = false;
            }
        }

        void Move(Direction direction, int ammount = 1)
        {
            while (ammount > 0)
            {
                if (direction == Direction.Down)
                {
                    if (SelectedButtonIndex + 1 < MenuItems.Count)
                    {
                        SelectedButtonIndex++;
                    }
                }
                else
                {
                    if (SelectedButtonIndex - 1 > -1)
                    {
                        SelectedButtonIndex--;
                    }
                }
                ammount--;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MenuItem menuItem in MenuItems)
            {
                if (menuItem.Blank == false)
                {
                    spriteBatch.Draw(_buttonTexture[0], menuItem.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                }
            }
            spriteBatch.Draw(_buttonTexture[1], MenuItems[SelectedButtonIndex].Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
    }

    public class SaveManager
    {
        public string FileName;
        public bool Saving;

        Playfield _playfield;
        XmlReader _reader;
        XmlWriter _writer;
        XmlWriterSettings _writerSettings;
        string _exeDirectory;

        public SaveManager(string fileName, Playfield playfield)
        {
            _playfield = playfield;
            _exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            _writerSettings = new XmlWriterSettings();
            _writerSettings.Indent = true;
            _writerSettings.NewLineOnAttributes = true;
            FileName = fileName;
        }

        public void Save(bool userSave)
        {
            Saving = true;
            _writer = XmlWriter.Create(_exeDirectory + FileName, _writerSettings);
            _writer.WriteStartDocument();
            _writer.WriteStartElement("savegame");

            _writer.WriteElementString("highScore", _playfield.HighScore.ToString());
            _writer.WriteElementString("userSave", userSave.ToString().ToLower());
            _writer.WriteElementString("IDCount", _playfield.IDCount.ToString());

            // Player
            _writer.WriteStartElement("player");

            _writer.WriteElementString("ID", _playfield.Player.ID.ToString());
            _writer.WriteElementString("score", _playfield.Player.Score.Value.ToString());
            _writer.WriteElementString("lives", _playfield.Player.Lives.ToString());

            _writer.WriteStartElement("location");
            _writer.WriteElementString("x", _playfield.Player.Location.X.ToString());
            _writer.WriteElementString("y", _playfield.Player.Location.Y.ToString());
            _writer.WriteEndElement();

            _writer.WriteEndElement();

            // Notes
            _writer.WriteStartElement("notes");
            _writer.WriteElementString("count", _playfield.Notes.Count.ToString());
            foreach (Note note in _playfield.Notes)
            {
                _writer.WriteStartElement("note");

                _writer.WriteElementString("ID", note.ID.ToString());
                _writer.WriteElementString("parentID", note.ParentID.ToString());

                _writer.WriteStartElement("location");
                _writer.WriteElementString("x", note.Location.X.ToString());
                _writer.WriteElementString("y", note.Location.Y.ToString());
                _writer.WriteEndElement();

                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();

            // Banjos
            _writer.WriteStartElement("banjos");
            _writer.WriteElementString("count", _playfield.Banjos.Count.ToString());
            foreach (Banjo banjo in _playfield.Banjos)
            {
                _writer.WriteStartElement("banjo");

                _writer.WriteElementString("ID", banjo.ID.ToString());
                _writer.WriteElementString("age", banjo.AgeInMilliseconds.ToString());
                _writer.WriteElementString("hitPoints", banjo.HitPoints.ToString());
                _writer.WriteElementString("type", banjo.Type.ToString());

                _writer.WriteStartElement("location");
                _writer.WriteElementString("x", banjo.Location.X.ToString());
                _writer.WriteElementString("y", banjo.Location.Y.ToString());
                _writer.WriteEndElement();

                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();

            _writer.WriteEndElement();
            _writer.WriteEndDocument();
            _writer.Close();
            Saving = false;
        }
        public bool Load()
        {
            if (System.IO.File.Exists(_exeDirectory + FileName) == true)
            {
                _reader = XmlReader.Create(_exeDirectory + FileName);
                try
                {
                    int count;
                    int ID;
                    int parentID;
                    int ageInMillliSecs;
                    int hitPoints;
                    Banjo.BanjoType type;
                    float x;
                    float y;

                    while (_reader.Read() == true)
                    {
                        if (_reader.IsStartElement())
                        {
                            switch (_reader.Name)
                            {
                                case "savegame":
                                    _reader.ReadToFollowing("highScore");
                                    _playfield.HighScore.Value = _reader.ReadElementContentAsInt();
                                    _reader.ReadToFollowing("userSave");
                                    if (_reader.ReadElementContentAsBoolean() == false)
                                    {
                                        _reader.Close();
                                        return true;
                                    }
                                    _reader.ReadToFollowing("IDCount");
                                    _playfield.IDCount = _reader.ReadElementContentAsInt();
                                    break;

                                case "player":
                                    _reader.ReadToFollowing("ID");
                                    _playfield.Player.ID = _reader.ReadElementContentAsInt();
                                    _reader.ReadToFollowing("score");
                                    _playfield.Player.Score.Value = _reader.ReadElementContentAsInt();
                                    _reader.ReadToFollowing("lives");
                                    _playfield.Player.Lives = _reader.ReadElementContentAsInt();
                                    _reader.ReadToFollowing("location");
                                    _reader.ReadToFollowing("x");
                                    _playfield.Player.Location.X = _reader.ReadElementContentAsFloat();
                                    _reader.ReadToFollowing("y");
                                    _playfield.Player.Location.Y = _reader.ReadElementContentAsFloat();
                                    break;

                                case "notes":
                                    _reader.ReadToFollowing("count");
                                    count = _reader.ReadElementContentAsInt();
                                    while (count > 0)
                                    {
                                        _reader.ReadToFollowing("ID");
                                        ID = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("parentID");
                                        parentID = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("x");
                                        x = _reader.ReadElementContentAsFloat();
                                        _reader.ReadToFollowing("y");
                                        y = _reader.ReadElementContentAsFloat();

                                        _playfield.LoadNote(ID, parentID, x, y);

                                        count--;
                                    }
                                    break;

                                case "banjos":
                                    _reader.ReadToFollowing("count");
                                    count = _reader.ReadElementContentAsInt();
                                    while (count > 0)
                                    {
                                        _reader.ReadToFollowing("ID");
                                        ID = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("age");
                                        ageInMillliSecs = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("hitPoints");
                                        hitPoints = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("type");
                                        type = (Banjo.BanjoType)Enum.Parse(typeof(Banjo.BanjoType), _reader.ReadElementContentAsString());
                                        _reader.ReadToFollowing("x");
                                        x = _reader.ReadElementContentAsFloat();
                                        _reader.ReadToFollowing("y");
                                        y = _reader.ReadElementContentAsFloat();

                                        _playfield.LoadBanjo(ID, type, x, y, ageInMillliSecs);

                                        count--;
                                    }
                                    break;
                            }
                        }
                    }
                    _reader.Close();
                }
                catch (Exception e)
                {
                    try
                    {
                        File.Delete(_exeDirectory + FileName);
                    }
                    catch
                    {
                        Debug.WriteLine("Failed to delete settings file " + FileName + "." + e.Message);
                    }

                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

    }

    public class Playfield
    {
        public struct SpawnPoint
        {
            public Vector2 Location;
            public ActionBox Box;

            public SpawnPoint(Vector2 location, Texture2D pixelTexture)
            {
                Location = location;
                Box = new ActionBox(pixelTexture, Color.Yellow, (int)location.X, (int)location.Y, 20, 20);
                Box.UpdateDraw();
            }
        }

        public Accordian Player;
        public List<Banjo> Banjos;
        public List<Note> Notes;
        public List<int> GarbageBanjos;
        public List<int> GarbageNotes;

        public Texture2D AccordianTexture;
        public Texture2D[] BanjoTexture;
        public Texture2D[] NoteTexture;

        public SpawnPoint[] BanjoSpawnPoint;

        public Color[] AccordianColorData;
        public Color[][] BanjoColorData;
        public Color[][] NoteColorData;

        public CollisionDetector CollisionDetector;

        public ActionBox ObjectBox;

        public bool GameOver;

        //public int HighScore;
        public Accordian.ScoreHandler HighScore;
        public int IDCount;

        int _maxSprites;
        int _spriteCount;
        float _noteLayerPoint;
        float _banjoLayerPoint;
        double _banjoSpawnTime;
        Random _randGen;
        Vector2 _defaultPlayerLocation;

        public Playfield(Texture2D accordianTexture, Texture2D[] noteTexture, Texture2D[] banjoTexture, Texture2D pixelTexture,
            Vector2 defaultPlayerLocation, Vector2[] banjoSpawnLocation, int maxSprites = 100)
        {
            AccordianTexture = accordianTexture;
            BanjoTexture = banjoTexture;
            NoteTexture = noteTexture;

            Banjos = new List<Banjo>();
            Notes = new List<Note>();
            GarbageBanjos = new List<int>();
            GarbageNotes = new List<int>();

            // Set up color array sizes
            AccordianColorData = new Color[accordianTexture.Width * accordianTexture.Height];
            BanjoColorData = new Color[3][];
            BanjoColorData[0] = new Color[banjoTexture[0].Width * banjoTexture[0].Height];
            BanjoColorData[1] = new Color[banjoTexture[1].Width * banjoTexture[1].Height];
            BanjoColorData[2] = new Color[banjoTexture[2].Width * banjoTexture[2].Height];
            NoteColorData = new Color[3][];
            NoteColorData[0] = new Color[noteTexture[0].Width * noteTexture[0].Height];
            NoteColorData[1] = new Color[noteTexture[1].Width * noteTexture[1].Height];
            NoteColorData[2] = new Color[noteTexture[2].Width * noteTexture[2].Height];

            // Give color arrays data accordingly
            AccordianTexture.GetData(AccordianColorData);
            BanjoTexture[0].GetData(BanjoColorData[0]);
            BanjoTexture[1].GetData(BanjoColorData[1]);
            BanjoTexture[2].GetData(BanjoColorData[2]);
            NoteTexture[0].GetData(NoteColorData[0]);
            NoteTexture[1].GetData(NoteColorData[1]);
            NoteTexture[2].GetData(NoteColorData[2]);

            BanjoSpawnPoint = new SpawnPoint[5];
            BanjoSpawnPoint[0] = new SpawnPoint(banjoSpawnLocation[0], pixelTexture);
            BanjoSpawnPoint[1] = new SpawnPoint(banjoSpawnLocation[1], pixelTexture);
            BanjoSpawnPoint[2] = new SpawnPoint(banjoSpawnLocation[2], pixelTexture);
            BanjoSpawnPoint[3] = new SpawnPoint(banjoSpawnLocation[3], pixelTexture);
            BanjoSpawnPoint[4] = new SpawnPoint(banjoSpawnLocation[4], pixelTexture);

            CollisionDetector = new CollisionDetector(this, pixelTexture);

            ObjectBox = new ActionBox(pixelTexture, Color.Blue, 0, 0);

            _defaultPlayerLocation = defaultPlayerLocation;

            // Set maximum number of sprites
            _maxSprites = maxSprites;

            _noteLayerPoint = 0.719f;
            _banjoLayerPoint = 0.799f;
            _randGen = new Random();
        }

        public void AddPlayer(Vector2 location, int lives, int score)
        {
            Player = new Accordian(NewID(), ObjectBox, AccordianTexture, AccordianColorData, location, lives, score);
        }
        public void AddNote(Vector2 location, int parentIdentityNo)
        {
            Notes.Add(new Note(NewID(), parentIdentityNo, ObjectBox, NoteTexture, NoteColorData, location, NewNoteLayer()));
        }
        public void AddBanjo(Vector2 location, Banjo.BanjoType type)
        {
            Banjos.Add(new Banjo(NewID(), ObjectBox, type, BanjoTexture, BanjoColorData, location, NewBanjoLayer()));
        }

        public void LoadNote(int ID, int parentID, float x, float y)
        {
            Notes.Add(new Note(ID, parentID, ObjectBox, NoteTexture, NoteColorData, new Vector2(x, y), NewNoteLayer()));
        }
        public void LoadBanjo(int ID, Banjo.BanjoType type, float x, float y, int ageInMilliseconds)
        {
            Banjos.Add(new Banjo(ID, ObjectBox, type, BanjoTexture, BanjoColorData, new Vector2(x, y), NewBanjoLayer(), ageInMilliseconds));
        }

        int NewID()
        {
            IDCount++;
            return IDCount;
        }

        float NewNoteLayer()
        {
            _noteLayerPoint += 0.001f;
            if (_noteLayerPoint >= 0.800f)
            {
                _noteLayerPoint = 0.720f;
            }
            return _noteLayerPoint;
        }
        float NewBanjoLayer()
        {
            _banjoLayerPoint += 0.001f;
            if (_banjoLayerPoint >= 0.989f)
            {
                _banjoLayerPoint = 0.800f;
            }
            return _banjoLayerPoint;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, bool debugMode)
        {
            CollisionDetector.Detect(debugMode);
            if (GameOver == false)
            {
                ManagePlayer(gameTime, graphicsDevice, debugMode);
                ManageBanjos(gameTime, graphicsDevice, debugMode);
            }

            //ClearGarbage();

            _banjoSpawnTime += gameTime.ElapsedGameTime.TotalSeconds;
        }
        void ManagePlayer(GameTime gameTime, GraphicsDevice graphicsDevice, bool debugMode)
        {

            Player.Update(gameTime, graphicsDevice, this, debugMode);
            for (int noteNo = 0; noteNo < Notes.Count; noteNo++)
            {
                if (Notes[noteNo].IsVisible(graphicsDevice) == false || Notes[noteNo].Expired == true)
                {
                    //GarbageNotes.Add(Notes.IndexOf(note));
                    Notes.RemoveAt(noteNo);
                }
                else
                {
                    Notes[noteNo].Update(gameTime, debugMode);
                }
            }
        }
        void ManageBanjos(GameTime gameTime, GraphicsDevice graphicsDevice, bool debugMode)
        {
            if (_banjoSpawnTime >= 0.2)
            {
                _banjoSpawnTime = 0;
                if (Banjos.Count < 50)
                {
                    AddBanjo(BanjoSpawnPoint[_randGen.Next(0, 5)].Location, GetBanjoType());
                }
            }
            for (int banjoNo = 0; banjoNo < Banjos.Count; banjoNo++)
            {
                if (Banjos[banjoNo].IsVisible(graphicsDevice) == false || Banjos[banjoNo].Expired == true)
                {
                    //GarbageBanjos.Add(Banjos.IndexOf(banjo))
                    if (Banjos[banjoNo].Expired == true)
                    {
                        Player.Score.Value += Banjos[banjoNo].ScoreValue;
                    }
                    Banjos.Remove(Banjos[banjoNo]);
                }
                else
                {
                    Banjos[banjoNo].Update(graphicsDevice, gameTime, Player, debugMode);
                }
            }
        }
        void ClearGarbage()
        {
            foreach (int noteIndex in GarbageNotes)
            {
                //playfield.Notes[noteIndex] = null;
                Notes.Remove(Notes[noteIndex]);
            }
            GarbageNotes.Clear();
            foreach (int banjoIndex in GarbageBanjos)
            {
                try
                {
                    //playfield.Banjos[banjoIndex] = null;
                    Banjos.Remove(Banjos[banjoIndex]);
                }
                catch
                {
                    Debug.WriteLine("Could not remove banjo at index " + banjoIndex + ".");
                }
            }
            GarbageBanjos.Clear();
        }

        Banjo.BanjoType GetBanjoType()
        {
            int rand = _randGen.Next(1, 101);
            if (rand < 81)
            {
                return Banjo.BanjoType.Normal;
            }
            else if (rand < 96)
            {
                return Banjo.BanjoType.Hunter;
            }
            else
            {
                return Banjo.BanjoType.Deadly;
            }
        }

        public void Clear()
        {
            Banjos.Clear();
            Notes.Clear();
            GarbageBanjos.Clear();
            GarbageNotes.Clear();
        }

        public void ClearGarbageSprites()
        {
            foreach (int noteIndex in GarbageNotes)
            {
                Notes[noteIndex] = null;
                Notes.Remove(Notes[noteIndex]);
            }
            GarbageNotes.Clear();
            foreach (int banjoIndex in GarbageBanjos)
            {
                Banjos[banjoIndex] = null;
                Banjos.Remove(Banjos[banjoIndex]);
            }
            GarbageBanjos.Clear();
        }

        public void Reset()
        {
            Clear();
            IDCount = 0;
            Player = new Accordian(NewID(), ObjectBox, AccordianTexture, AccordianColorData, _defaultPlayerLocation, 3, 0);
            GameOver = false;
        }
    }

    public class Sprite
    {
        public int ID;
        public Texture2D Texture;
        public Color[] TextureColorData;
        public ActionBox Box;
        public Vector2 Location;
        public Vector2 CenterPoint;
        public float LayerPoint;

        public void UpdateBox(bool debugMode)
        {
            Box.X = (int)Location.X;
            Box.Y = (int)Location.Y;
            if (debugMode == true)
            {
                Box.UpdateDraw();
            }
        }
    }

    public class Accordian : Sprite
    {
        public struct ScoreHandler
        {
            public int Value;
            //public string String
            //{
            //    get
            //    {
            //        string vString = Value.ToString();
            //        int vLength = vString.Length;
            //        for (int zerosToAdd = 8 - vLength; zerosToAdd > 0; zerosToAdd--)
            //        {
            //            vString = "0" + vString;
            //        }
            //        return vString;
            //    }
            //}
            public string ToString(bool zeros = true)
            {
                string vString = Value.ToString();
                int vLength = vString.Length;
                for (int zerosToAdd = 8 - vLength; zerosToAdd > 0; zerosToAdd--)
                {
                    if (zeros == true)
                    {
                        vString = "0" + vString;
                    }
                    else
                    {
                        vString = " " + vString;
                    }
                }
                return vString;
            }
        }
        public Texture2D NoteTexture;
        public Color[] NoteTextureColorData;
        public int Lives;
        public ScoreHandler Score;


        public bool Expired;

        int _velocity = 5;
        float _scale = 1f;
        Vector2 _noteSpawnPoint;
        double _noteSpawnTime;


        //public Accordian(Texture2D texture, Color[] textureColorData, Texture2D noteTexture, Color[] noteTextureColorData, Vector2 location, int lives, int score)
        public Accordian(int identityNo, ActionBox box, Texture2D texture, Color[] textureColorData, Vector2 location, int lives, int score)
        {
            ID = identityNo;
            Texture = texture;
            TextureColorData = textureColorData;
            //NoteTexture = noteTexture;
            //NoteTextureColorData = noteTextureColorData;
            Location = location;
            Lives = lives;
            Score = new ScoreHandler();
            Score.Value = score;
            Box = new ActionBox(box.PixelTexture, box.Color, (int)Location.X, (int)Location.Y, (int)(Texture.Width * _scale), (int)(Texture.Height * _scale));
            _noteSpawnPoint = new Vector2(0, location.Y);
        }

        public void Update(GameTime gameTime, GraphicsDevice graphics, Playfield playfield, bool debugMode)
        {
            if (Expired == true)
            {
                if (Lives > 1)
                {
                    Lives--;
                    Expired = false;
                }
                else
                {
                    // game over
                    playfield.GameOver = true;
                }
            }

            // GamePad
            #region Analogue Stick
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < -0.8f)
            {
                if (atLeftEnd(graphics) == false)
                {
                    Location.X = Location.X - _velocity;
                }
            }
            else if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0.8f)
            {
                if (atRightEnd(graphics) == false)
                {
                    Location.X = Location.X + _velocity;
                }
            }
            #endregion
            #region D-Pad
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
            {
                if (atLeftEnd(graphics) == false)
                {
                    Location.X = Location.X - _velocity;
                }
            }
            else if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
            {
                if (atRightEnd(graphics) == false)
                {
                    Location.X = Location.X + _velocity;
                }
            }
            #endregion
            #region Trigger
            if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0.8f)
            {
                NoteSpawnTrigger(gameTime, playfield);
            }
            #endregion
            // Keyboard
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (atLeftEnd(graphics) == false)
                {
                    Location.X = Location.X - _velocity;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (atRightEnd(graphics) == false)
                {
                    Location.X = Location.X + _velocity;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                NoteSpawnTrigger(gameTime, playfield);
            }

            // Final Changes

            base.UpdateBox(debugMode);
            _noteSpawnTime += gameTime.ElapsedGameTime.TotalSeconds;
        }
        void NoteSpawnTrigger(GameTime gameTime, Playfield playfield)
        {
            _noteSpawnPoint.X = Location.X + 35;
            if (_noteSpawnTime > 0.10)
            {
                _noteSpawnTime = 0;
                playfield.AddNote(_noteSpawnPoint, ID);
            }
        }
        bool atLeftEnd(GraphicsDevice graphics)
        {
            if (Location.X <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        bool atRightEnd(GraphicsDevice graphics)
        {
            if (Location.X >= (graphics.Viewport.Width - (Texture.Width * _scale)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.95f);
        }
    }
    public class Note : Sprite
    {
        public Texture2D[] Textures;
        public Color[][] TexturesColorData;
        public bool Expired;
        public int ParentID;

        int _velocity = 5;
        //float _scale = 0.1f;
        float _scale = 1f;
        int _textureNo = 0;
        double _textureTime;

        public Note(int identityNo, int parentIdentityNo, ActionBox box, Texture2D[] textures, Color[][] textureColorData, Vector2 location, float layerPoint)
        {
            ID = identityNo;
            ParentID = parentIdentityNo;
            Textures = textures;
            Texture = textures[0];
            TexturesColorData = textureColorData;
            TextureColorData = textureColorData[0];
            Location = location;
            Box = new ActionBox(box.PixelTexture, box.Color, (int)Location.X, (int)Location.Y, (int)(Texture.Width * _scale), (int)(Texture.Height * _scale));
            LayerPoint = layerPoint;
        }

        public void Update(GameTime gameTime, bool debugMode)
        {
            Location.Y = Location.Y - _velocity;
            if (_textureTime > 0.02)
            {
                _textureTime = 0;
                Texture = Textures[GetTextureNo()];
                TextureColorData = TexturesColorData[GetTextureNo()];
            }
            base.UpdateBox(debugMode);
            _textureTime += gameTime.ElapsedGameTime.TotalSeconds;
        }
        int GetTextureNo()
        {
            _textureNo++;
            if (_textureNo > 2)
            {
                _textureNo = 0;
            }
            return _textureNo;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, LayerPoint);
        }

        public bool IsVisible(GraphicsDevice graphics)
        {
            if (Location.Y < 0 - (Texture.Height * _scale))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public class Banjo : Sprite
    {
        public enum BanjoType
        {
            Normal,
            Hunter,
            Deadly,
        }

        public BanjoType Type;
        public int Velocity;
        public int HitPoints;
        public int ScoreValue;
        public int AgeInMilliseconds;
        public bool ReachedBottom;
        public bool Expired;


        int _direction = 0;
        //float _scale = 0.15f;
        float _scale = 1f;
        int _lastTime;
        int _ySideMovement = 50;
        int _yMovementRemaining;
        bool _goingDown = false;
        int _hoverSize = 20;
        int _hoverSpeed = 1;
        int _hoverRemaining;
        bool _hoverDown = true;

        public Banjo(int identityNo, ActionBox box, BanjoType type, Texture2D[] textures, Color[][] textureColorData, Vector2 location, float layerPoint, int ageInMilliseconds = 0)
        {
            ID = identityNo;
            Type = type;
            if (Type == BanjoType.Normal)
            {
                Texture = textures[0];
                TextureColorData = textureColorData[0];
                Velocity = 1;
                HitPoints = 1;
                ScoreValue = 10;
            }
            else if (Type == BanjoType.Hunter)
            {
                Texture = textures[1];
                TextureColorData = textureColorData[1];
                Velocity = 1;
                HitPoints = 1;
                ScoreValue = 20;
            }
            else
            {
                Texture = textures[2];
                TextureColorData = textureColorData[2];
                Velocity = 3;
                HitPoints = 2;
                ScoreValue = 50;
            }

            Location = location;
            Box = new ActionBox(box.PixelTexture, box.Color, (int)Location.X, (int)Location.Y, (int)(Texture.Width * _scale), (int)(Texture.Height * _scale));
            LayerPoint = layerPoint;
            AgeInMilliseconds = ageInMilliseconds;
        }

        public void Update(GraphicsDevice graphics, GameTime gameTime, Accordian player, bool debugMode)
        {
            AgeInMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
            if (Type == BanjoType.Normal)
            {
                PerformRegularBanjoBehaviour(graphics);
                ReachedBottomCheck(graphics);
            }
            else if (Type == BanjoType.Hunter)
            {
                if ((AgeInMilliseconds) <= 5000)
                {
                    PerformRegularBanjoBehaviour(graphics);
                }
                else
                {
                    PerformAdvancedBanjoBehaviour(player);
                }
                ReachedBottomCheck(graphics);
            }
            else
            {
                PerformAdvancedBanjoBehaviour(player);
            }
            base.UpdateBox(debugMode);
        }
        private void PerformRegularBanjoBehaviour(GraphicsDevice graphics)
        {
            if ((Location.X <= 0 || Location.X >= (graphics.Viewport.Width - (Texture.Width * _scale))) && _goingDown != true)
            {
                _goingDown = true;
                _yMovementRemaining = _ySideMovement;
            }
            if (_goingDown == false)
            {
                MoveInDirection();
                Hover();
            }
            else
            {
                Location.Y = Location.Y + Velocity;
                if (_yMovementRemaining > 0)
                {
                    _yMovementRemaining--;
                }
                else
                {
                    SwapDirection();
                    MoveInDirection();
                    _goingDown = false;
                }
            }
        }
        private void PerformAdvancedBanjoBehaviour(Accordian player)
        {
            if (player.Location.X > Location.X)
            {
                if (Location.X + Velocity <= player.Location.X)
                {
                    Location.X = Location.X + Velocity;
                }
                else
                {
                    Location.X = player.Location.X;
                }
            }
            else
            {
                if (Location.X - Velocity >= player.Location.X)
                {
                    Location.X = Location.X - Velocity;
                }
                else
                {
                    Location.X = player.Location.X;
                }
            }

            if (player.Location.Y > Location.Y)
            {
                if (Location.Y + Velocity <= player.Location.Y)
                {
                    Location.Y = Location.Y + Velocity;
                }
                else
                {
                    Location.Y = player.Location.Y;
                }
            }
        }
        private void MoveInDirection()
        {
            if (_direction == 0)
            {
                Location.X = Location.X + Velocity;
            }
            else
            {
                Location.X = Location.X - Velocity;
            }

        }
        private void Hover()
        {
            if (_hoverDown == true)
            {
                if (_hoverRemaining > 0)
                {
                    Location.Y += _hoverSpeed;
                    _hoverRemaining--;
                }
                else
                {
                    _hoverRemaining = _hoverSize;
                    _hoverDown = false;
                }
            }
            else
            {
                if (_hoverRemaining > 0)
                {
                    Location.Y -= _hoverSpeed;
                    _hoverRemaining--;
                }
                else
                {
                    _hoverRemaining = _hoverSize;
                    _hoverDown = true;
                }
            }
        }
        private void SwapDirection()
        {
            if (_direction == 0)
            {
                _direction = 1;
            }
            else
            {
                _direction = 0;
            }
        }
        private void ReachedBottomCheck(GraphicsDevice graphics)
        {
            if (Location.Y >= graphics.Viewport.Y)
            {
                ReachedBottom = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, LayerPoint);
        }

        public bool IsVisible(GraphicsDevice graphics)
        {
            if (Location.Y > graphics.Viewport.Height)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class ActionBox
    {
        public Texture2D PixelTexture;
        public Rectangle Rectangle
        {
            get
            {
                return _mainRect;
            }
            set
            {
                _mainRect = value;
            }
        }
        public Color Color;
        public int Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                _thickness = value;
            }
        }

        // Rectangle-like properties
        public int X
        {
            get
            {
                return _mainRect.X;
            }
            set
            {
                _mainRect.X = value;
            }
        }
        public int Y
        {
            get
            {
                return _mainRect.Y;
            }
            set
            {
                _mainRect.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return _mainRect.Width;
            }
            set
            {
                _mainRect.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return _mainRect.Height;
            }
            set
            {
                _mainRect.Height = value;
            }
        }

        Rectangle _mainRect;
        int _thickness;

        Rectangle _top;
        Rectangle _right;
        Rectangle _bottom;
        Rectangle _left;

        /// <summary>
        /// This method should be called after making any changes to the box.
        /// </summary>
        public void UpdateDraw()
        {
            _top = new Rectangle(_mainRect.X, _mainRect.Y, _mainRect.Width, _thickness);
            _right = new Rectangle((_mainRect.X + _mainRect.Width - _thickness), _mainRect.Y, _thickness, _mainRect.Height);
            _bottom = new Rectangle(_mainRect.X, _mainRect.Y + _mainRect.Height - _thickness, _mainRect.Width, _thickness);
            _left = new Rectangle(_mainRect.X, _mainRect.Y, _thickness, _mainRect.Height);
        }

        public ActionBox(Texture2D pixelTexture, Color color, int x = 0, int y = 0, int width = 1, int height = 1, int thickness = 1)
        {
            PixelTexture = pixelTexture;
            Color = color;
            Rectangle = new Rectangle(x, y, width, height);
            Thickness = thickness;
        }

        public void Setup(int x, int y, int width, int height)
        {
            _mainRect.X = x;
            _mainRect.Y = y;
            _mainRect.Width = width;
            _mainRect.Height = height;
        }

        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            // Draw top line
            spriteBatch.Draw(PixelTexture, _top, null, Color, 0f, Vector2.Zero, SpriteEffects.None, depth);

            // Draw right line
            spriteBatch.Draw(PixelTexture, _right, null, Color, 0f, Vector2.Zero, SpriteEffects.None, depth);

            // Draw bottom line
            spriteBatch.Draw(PixelTexture, _bottom, null, Color, 0f, Vector2.Zero, SpriteEffects.None, depth);

            // Draw left line
            spriteBatch.Draw(PixelTexture, _left, null, Color, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }
    }
    public class CollisionDetector
    {
        public ActionBox IntersectBox;
        public ActionBox NearBox;
        public ActionBox TouchBox;

        public double NearBoxDrawTime = 0;
        public double IntersectBoxDrawTime = 0;
        public double TouchBoxDrawTime = 0;

        enum CollisionType
        {
            NoteToBanjo,
            BanjoToPlayer
        }

        Playfield _playfield;

        // Near box (bounds) for being near a banjo
        int _nbBanjoMaxX = 20;
        int _nbBanjoMinX = -60;
        int _nbBanjoMaxY = 20;
        int _nbBanjoMinY = -90;

        int _nbPlayerMaxX = 50;
        int _nbPlayerMinX = -150;
        int _nbPlayerMaxY = 100;
        int _nbPlayerMinY = -80;

        int _secsToDraw = 3;

        public CollisionDetector(Playfield playfield, Texture2D pixel)
        {
            _playfield = playfield;

            // Create debug boxes
            IntersectBox = new ActionBox(pixel, Color.Red);
            NearBox = new ActionBox(pixel, Color.Green);
            TouchBox = new ActionBox(pixel, Color.Purple);
        }

        public void Detect(bool debugMode)
        {
            foreach (Banjo banjo in _playfield.Banjos)
            {
                if (IsNear(_playfield.Player.Location, banjo.Location, CollisionType.BanjoToPlayer))
                {
                    NearBox.X = _playfield.Player.Box.X - _nbPlayerMaxX;
                    NearBox.Y = _playfield.Player.Box.Y - _nbPlayerMaxY;
                    NearBox.Width = _nbPlayerMaxX - _nbPlayerMinX;
                    NearBox.Height = _nbPlayerMaxY - _nbPlayerMinY;

                    if (debugMode == true)
                    {
                        NearBox.UpdateDraw();
                        NearBoxDrawTime = _secsToDraw;
                    }

                    //if (banjo.Box.Rectangle.Intersects(_playfield.Player.Box.Rectangle))
                    //{
                    //    IntersectBox.Rectangle = _playfield.Player.Box.Rectangle;

                    //    if (debugMode == true)
                    //    {
                    //        IntersectBox.UpdateDraw();
                    //        IntersectBoxDrawTime = _secsToDraw;
                    //    }

                    if (IsTouching(banjo.Box.Rectangle, _playfield.Player.Box.Rectangle, banjo.TextureColorData, _playfield.Player.TextureColorData, debugMode))
                    {
                        banjo.Expired = true;
                        _playfield.Player.Expired = true;
                    }

                    //}
                }

                foreach (Note note in _playfield.Notes)
                {
                    if (IsNear(banjo.Location, note.Location, CollisionType.NoteToBanjo))
                    {
                        NearBox.X = banjo.Box.X - _nbBanjoMaxX;
                        NearBox.Y = banjo.Box.Y - _nbBanjoMaxY;
                        NearBox.Width = _nbBanjoMaxX - _nbBanjoMinX;
                        NearBox.Height = _nbBanjoMaxY - _nbBanjoMinY;

                        if (debugMode == true)
                        {
                            NearBox.UpdateDraw();
                            NearBoxDrawTime = _secsToDraw;
                        }

                        //if (banjo.Box.Rectangle.Intersects(note.Box.Rectangle))
                        //{
                        //    IntersectBox.Rectangle = banjo.Box.Rectangle;

                        //    if (debugMode == true)
                        //    {
                        //        IntersectBox.UpdateDraw();
                        //        IntersectBoxDrawTime = _secsToDraw;
                        //    }

                        if (IsTouching(banjo.Box.Rectangle, note.Box.Rectangle, banjo.TextureColorData, note.TextureColorData, debugMode))
                        {
                            banjo.Expired = true;
                            note.Expired = true;
                        }
                        //}
                    }
                }
            }
        }

        bool IsNear(Vector2 Targetlocation, Vector2 Projectilelocation, CollisionType type)
        {
            float xDiff = Targetlocation.X - Projectilelocation.X;
            float yDiff = Targetlocation.Y - Projectilelocation.Y;

            int _nbMaxX;
            int _nbMinX;
            int _nbMaxY;
            int _nbMinY;

            if (type == CollisionType.NoteToBanjo)
            {
                _nbMaxX = _nbBanjoMaxX;
                _nbMinX = _nbBanjoMinX;
                _nbMaxY = _nbBanjoMaxY;
                _nbMinY = _nbBanjoMinY;
            }
            else
            {
                _nbMaxX = _nbPlayerMaxX;
                _nbMinX = _nbPlayerMinX;
                _nbMaxY = _nbPlayerMaxY;
                _nbMinY = _nbPlayerMinY;
            }

            if (xDiff < _nbMaxX && xDiff > _nbMinX)
            {
                if (yDiff < _nbMaxY && yDiff > _nbMinY)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        bool IsTouching(Rectangle rectangleA, Rectangle rectangleB, Color[] colorsA, Color[] colorsB, bool debugMode)
        {
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            TouchBox.X = left;
            TouchBox.Y = top;
            TouchBox.Width = right - left;
            TouchBox.Height = bottom - top;
            if (debugMode == true)
            {
                TouchBox.UpdateDraw();
                TouchBoxDrawTime = _secsToDraw;
            }

            for (int yPix = top; yPix < bottom; yPix++)
            {
                for (int xPix = left; xPix < right; xPix++)
                {
                    Color colorBanjo = colorsA[(xPix - rectangleA.Left) + ((yPix - rectangleA.Top) * rectangleA.Width)];
                    Color colorNote = colorsB[(xPix - rectangleB.Left) + ((yPix - rectangleB.Top) * rectangleB.Width)];

                    if (colorBanjo.A != 0 && colorNote.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
