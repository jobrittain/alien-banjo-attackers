using ABAFS.Physics;
using ABAFS.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ABAFS
{
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

        public Texture2D[] AccordianTexture;
        public Texture2D[] BanjoTexture;
        public Texture2D[] NoteTexture;
        public Texture2D[] ExplosionTexture;

        public SoundEffect AccordianFireSound;
        public SoundEffect ExplosionSound;

        public SpawnPoint[] BanjoSpawnPoint;

        public Color[] AccordianColorData;
        public Color[][] BanjoColorData;
        public Color[][] NoteColorData;

        public CollisionDetector CollisionDetector;

        public ActionBox ObjectBox;

        public bool GameOver;

        public Accordian.ScoreHandler HighScore;
        public int IDCount;

        int _maxSprites;
        int _spriteCount;
        float _noteLayerPoint;
        float _banjoLayerPoint;
        double _banjoSpawnTime;
        Random _randGen;
        Vector2 _defaultPlayerLocation;
        Vector2[] _explosionLocations;

        public Playfield(Texture2D[] accordianTexture, Texture2D[] noteTexture, Texture2D[] banjoTexture, Texture2D[] explosionTexture, Texture2D pixelTexture,
            Vector2 defaultPlayerLocation, Vector2[] banjoSpawnLocation, 
            SoundEffect accordianFireSound, SoundEffect explosionSound, int maxSprites = 100)
        {
            AccordianTexture = accordianTexture;
            BanjoTexture = banjoTexture;
            NoteTexture = noteTexture;
            ExplosionTexture = explosionTexture;

            AccordianFireSound = accordianFireSound;
            ExplosionSound = explosionSound;

            Banjos = new List<Banjo>();
            Notes = new List<Note>();
            GarbageBanjos = new List<int>();
            GarbageNotes = new List<int>();

            // Set up color array sizes
            AccordianColorData = new Color[accordianTexture[0].Width * accordianTexture[0].Height];
            BanjoColorData = new Color[3][];
            BanjoColorData[0] = new Color[banjoTexture[0].Width * banjoTexture[0].Height];
            BanjoColorData[1] = new Color[banjoTexture[1].Width * banjoTexture[1].Height];
            BanjoColorData[2] = new Color[banjoTexture[2].Width * banjoTexture[2].Height];
            NoteColorData = new Color[3][];
            NoteColorData[0] = new Color[noteTexture[0].Width * noteTexture[0].Height];
            NoteColorData[1] = new Color[noteTexture[1].Width * noteTexture[1].Height];
            NoteColorData[2] = new Color[noteTexture[2].Width * noteTexture[2].Height];

            // Give color arrays data accordingly
            AccordianTexture[0].GetData(AccordianColorData);
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
            _explosionLocations = new Vector2[4];

            // Set maximum number of sprites
            _maxSprites = maxSprites;

            _noteLayerPoint = 0.719f;
            _banjoLayerPoint = 0.799f;
            _randGen = new Random();
        }

        public void AddPlayer(Vector2 location, int lives, int score)
        {
            Player = new Accordian(NewID(), ObjectBox, AccordianTexture, AccordianColorData, makeExplosions(4), _randGen, ExplosionSound, AccordianFireSound, location, lives, score);
        }
        public void AddNote(Vector2 location, int parentIdentityNo, bool targetPlayer)
        {
            Notes.Add(new Note(NewID(), parentIdentityNo, ObjectBox, NoteTexture, NoteColorData, location, this, NewNoteLayer(), targetPlayer));
        }
        public void AddBanjo(Vector2 location, Banjo.BanjoType type)
        {
            if (_spriteCount < _maxSprites)
            {
                Banjos.Add(new Banjo(NewID(), ObjectBox, type, BanjoTexture, BanjoColorData, makeExplosions(4), _randGen, ExplosionSound, location, NewBanjoLayer()));
            }
        }
        Explosion[] makeExplosions(int count)
        {
            Explosion[] _explosions = new Explosion[count];

            while (count > 0)
            {
                _explosions[count - 1] = new Explosion(Vector2.Zero, ExplosionTexture);
                count--;
            }

            return _explosions;
        }

        public void LoadNote(int ID, int parentID, float x, float y, bool playerTarget = false, float _xVelocity = 0, float _yVelocity = 0)
        {
            Notes.Add(new Note(ID, parentID, ObjectBox, NoteTexture, NoteColorData, new Vector2(x, y), this, NewNoteLayer()));
        }
        public void LoadBanjo(int ID, Banjo.BanjoType type, float x, float y, int ageInMilliseconds)
        {
            Vector2 location = new Vector2(x, y);
            Banjos.Add(new Banjo(ID, ObjectBox, type, BanjoTexture, BanjoColorData, makeExplosions(4), _randGen, ExplosionSound, location, NewBanjoLayer(), ageInMilliseconds));
        }

        /// <summary>
        /// Get a new ID.
        /// </summary>
        /// <returns>A new ID number</returns>
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

            ManagePlayer(gameTime, graphicsDevice, debugMode);
            ManageBanjos(gameTime, graphicsDevice, debugMode);
            
            VibrationManager.Update(gameTime);

            _spriteCount = Banjos.Count;
            _banjoSpawnTime += gameTime.ElapsedGameTime.TotalSeconds;
        }
        void ManagePlayer(GameTime gameTime, GraphicsDevice graphicsDevice, bool debugMode)
        {

            Player.Update(gameTime, graphicsDevice, this, debugMode);
            for (int noteNo = 0; noteNo < Notes.Count; noteNo++)
            {
                if (Notes[noteNo].IsVisible(graphicsDevice) == false || Notes[noteNo].Expired == true)
                {
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
                if (Banjos.Count < 50 && GameOver == false)
                {
                    AddBanjo(BanjoSpawnPoint[_randGen.Next(0, 5)].Location, GetBanjoType());
                }
            }
            for (int banjoNo = 0; banjoNo < Banjos.Count; banjoNo++)
            {
                if (Banjos[banjoNo].IsVisible(graphicsDevice) == false || Banjos[banjoNo].Expired == true)
                {
                    if (Banjos[banjoNo].Expired == true && Banjos[banjoNo].Killed == true && GameOver == false)
                    {
                        Player.Score.Value += Banjos[banjoNo].ScoreValue;
                    }
                    Banjos.Remove(Banjos[banjoNo]);
                }
                else
                {
                    Banjos[banjoNo].Update(graphicsDevice, gameTime, this, debugMode);
                }

                if (GameOver == false)
                {
                    if (Banjos[banjoNo].ReachedBottom == true)
                    {
                        Player.TriggerExpireExplosion();
                        GameOver = true;
                    }
                }
            }
        }
        void ClearGarbage()
        {
            foreach (int noteIndex in GarbageNotes)
            {
                Notes.Remove(Notes[noteIndex]);
            }
            GarbageNotes.Clear();
            foreach (int banjoIndex in GarbageBanjos)
            {
                try
                {
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
            Player = new Accordian(NewID(), ObjectBox, AccordianTexture, AccordianColorData, makeExplosions(4), _randGen, ExplosionSound, AccordianFireSound, _defaultPlayerLocation, 3, 0);
            GameOver = false;
        }
    }
}
