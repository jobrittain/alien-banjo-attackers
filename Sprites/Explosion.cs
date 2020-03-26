using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS.Sprites
{
    public class Explosion : Sprite
    {
        public Texture2D[] Textures;
        public bool Active = false;
        public bool Complete = false;

        int _textureCount;
        int _textureIndex;
        double _textureTime;
        double _transitionTime = 0.01;
        double _timeOffset;

        public Explosion(Vector2 location, Texture2D[] explosionTextures, double timeOffset = 0)
        {
            Location = location;
            Textures = explosionTextures;
            Texture = explosionTextures[0];
            _textureCount = Textures.Length;
            _timeOffset = timeOffset;
        }

        public void Activate()
        {
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (_timeOffset > 0)
            {
                _timeOffset -= gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Active == true && _timeOffset <= 0)
            {
                _textureTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (_textureTime > _transitionTime)
                {
                    if (_textureIndex + 1 < _textureCount)
                    {
                        _textureIndex++;
                        Texture = Textures[_textureIndex];
                    }
                    else
                    {
                        Complete = true;
                    }
                    _textureTime = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Complete == false)
            {
                spriteBatch.Draw(Texture, Location, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.991f);
            }
        }
    }
}
