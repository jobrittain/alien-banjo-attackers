using ABAFS.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS.Sprites
{
    public class Note : Sprite
    {
        public Texture2D[] Textures;
        public Color[][] TexturesColorData;
        public bool Expired;
        public bool TargetPlayer;
        public int ParentID;

        int _velocity = 5;
        float _scale = 1f;
        int _textureNo = 0;
        double _textureTime;

        float _xVelocity;
        float _yVelocity;

        public Note(int identityNo, int parentIdentityNo, ActionBox box, Texture2D[] textures, Color[][] textureColorData, Vector2 location, Playfield playfield, float layerPoint, bool targetPlayer = false, float xVelocity = 0, float yVelocity = 0)
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

            TargetPlayer = targetPlayer;
            if (targetPlayer == true && xVelocity == 0 && yVelocity == 0)
            {
                GetBidirectionalVelocity(playfield.Player.Location);
            }
            else
            {
                _xVelocity = xVelocity;
                _yVelocity = yVelocity;
            }
        }

        void GetBidirectionalVelocity(Vector2 targetLocation)
        {
            double xDiff = targetLocation.X - Location.X;
            double yDiff = targetLocation.Y - Location.Y;
            double zDiff = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
            double downScale = zDiff / _velocity;
            _xVelocity = (float)(xDiff / downScale);
            _yVelocity = (float)(yDiff / downScale);
        }

        public void Update(GameTime gameTime, bool debugMode)
        {
            if (TargetPlayer == false)
            {
                Location.Y -= _velocity;
            }
            else
            {
                Location.X += _xVelocity;
                Location.Y += _yVelocity;
            }

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
}
