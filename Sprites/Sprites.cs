using ABAFS.Physics;
using ABAFS.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS
{
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

        // Explosions Code

        public bool Explode
        {
            get
            {
                return _explode;
            }
        }

        Explosion[] _explosions;
        Random _explosionPointGenerator;
        SoundEffect _explodeSound;

        bool _explode;
        bool _explosionsSetup;
        bool[] _explosionsComplete;
        int _explosionCount;

        public bool UpdateExplosions(GameTime gameTime)
        {
            for (int i = 0; i < _explosionCount; i++)
            {
                _explosions[i].Update(gameTime);
                _explosionsComplete[i] = _explosions[i].Complete;
            }
            foreach (bool ec in _explosionsComplete)
            {
                if (ec == false)
                {
                    return false;
                }
            }
            return true;
        }
        public void SetupExplosions(Explosion[] explosions, Random explosionPointGenerator, SoundEffect explodeSound)
        {
            _explosionCount = explosions.Length;
            _explosions = explosions;
            _explosionsComplete = new bool[_explosionCount];
            _explosionPointGenerator = explosionPointGenerator;
            _explodeSound = explodeSound;
            _explosionsSetup = true;
        }
        public void TriggerExpireExplosion()
        {
            if (_explosionsSetup == true)
            {
                _explode = true;
                _explodeSound.Play();
                ActivateExplosions();
            }
            else
            {
                throw new Exception("Explosions not activated.");
            }
        }
        private void ActivateExplosions()
        {
            foreach (Explosion e in _explosions)
            {
                e.Location = new Vector2((Location.X - (Texture.Width)) + _explosionPointGenerator.Next(0, Texture.Width),
                                         (Location.Y - (Texture.Height)) + _explosionPointGenerator.Next(0, Texture.Height));
                e.Activate();
            }
        }

        public void DrawExplosions(SpriteBatch spriteBatch)
        {
            foreach (Explosion e in _explosions)
            {
                e.Draw(spriteBatch);
            }
        }

    }
}
