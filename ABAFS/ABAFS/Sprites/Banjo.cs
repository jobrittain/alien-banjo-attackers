using ABAFS.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS.Sprites
{
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
        public bool Hit;
        public bool Killed
        {
            get 
            {
                return _killed;
            }
        }
        
        Random _random;
        Vector2 _noteSpawnPoint;

        bool _killed;
        int _direction = 0;
        float _scale = 1f;
        int _lastTime;
        int _ySideMovement = 50;
        int _yMovementRemaining;
        bool _goingDown = false;
        int _hoverSize = 20;
        int _hoverSpeed = 1;
        int _hoverRemaining;
        bool _hoverDown = true;
        bool _wdMovementSet;
        float _wdX;
        float _wdY;
        float _opacity;
        double _opacityTime;

        double _noteTimeGap = 1;
        double _noteSpawnTime;

        public Banjo(int identityNo, ActionBox box, BanjoType type, Texture2D[] textures, Color[][] textureColorData, Explosion[] explosions, Random explosionPointGen, SoundEffect explodeSound, Vector2 location, float layerPoint, int ageInMilliseconds = 0)
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
            _random = explosionPointGen;
            base.SetupExplosions(explosions, explosionPointGen, explodeSound);
        }

        public void Update(GraphicsDevice graphics, GameTime gameTime, Playfield playfield, bool debugMode)
        {
            AgeInMilliseconds += gameTime.ElapsedGameTime.Milliseconds;

            if (_opacity != 1)
            {
                _opacityTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (_opacityTime > 0.025)
                {
                    _opacity += 0.1f;
                    _opacityTime = 0;
                }
            }

            if (Explode == false)
            {
                if (Hit == true)
                {
                    if (HitPoints > 1)
                    {
                        HitPoints--;
                        Hit = false;
                    }
                    else
                    {
                        _killed = true;
                        base.TriggerExpireExplosion();
                    }
                }
                if (playfield.GameOver == false)
                {
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
                            PerformAdvancedBanjoBehaviour(gameTime, playfield);
                        }
                        ReachedBottomCheck(graphics);
                    }
                    else // BanjoType.Deadly
                    {
                        PerformAdvancedBanjoBehaviour(gameTime, playfield);
                        ReachedBottomCheck(graphics);
                    }
                }
                else
                {
                    if (_wdMovementSet == false)
                    {
                        _wdX = _random.Next(-1, 2);
                        _wdY = Velocity;
                        _wdMovementSet = true;
                    }
                    PerformWorldDominationBehaviour();
                }
            }
            else
            {
                if (base.UpdateExplosions(gameTime) == true)
                {
                    Expired = true;
                }
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
        private void PerformAdvancedBanjoBehaviour(GameTime gameTime, Playfield playfield)
        {
            // Homing Movement
            if (playfield.Player.Location.X > Location.X)
            {
                if (Location.X + Velocity <= playfield.Player.Location.X)
                {
                    Location.X = Location.X + Velocity;
                }
                else
                {
                    Location.X = playfield.Player.Location.X;
                }
            }
            else
            {
                if (Location.X - Velocity >= playfield.Player.Location.X)
                {
                    Location.X = Location.X - Velocity;
                }
                else
                {
                    Location.X = playfield.Player.Location.X;
                }
            }

            if (playfield.Player.Location.Y > Location.Y)
            {
                if (Location.Y + Velocity <= playfield.Player.Location.Y)
                {
                    Location.Y = Location.Y + Velocity;
                }
                else
                {
                    Location.Y = playfield.Player.Location.Y;
                }
            }

            // Note Fire
            if (Type == BanjoType.Deadly)
            {
                NoteSpawnTrigger(gameTime, playfield);
            }
        }
        private void PerformWorldDominationBehaviour()
        {
            Location.X += _wdX;
            Location.Y += _wdY;
        }
        private void NoteSpawnTrigger(GameTime gameTime, Playfield playfield)
        {
            _noteSpawnTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (_noteSpawnTime > _noteTimeGap)
            {
                _noteSpawnTime = 0;
                _noteSpawnPoint.X = UsefulFunctions.GetTextureCenterPoint(Texture.Bounds, Location, Texture, UsefulFunctions.CenterPointType.X);
                _noteSpawnPoint.Y = Location.Y + 20;
                playfield.AddNote(_noteSpawnPoint, ID, true);
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
            if (Location.Y >= graphics.Viewport.Height)
            {
                ReachedBottom = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Explode == false)
            {
                spriteBatch.Draw(Texture, Location, null, Color.White * _opacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, LayerPoint);
            }
            else
            {
                base.DrawExplosions(spriteBatch);
            }
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
}
