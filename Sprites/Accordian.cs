using ABAFS.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS.Sprites
{
    public class Accordian : Sprite
    {
        /// <summary>
        /// A type for handling scores.
        /// </summary>
        public struct ScoreHandler
        {
            public int Value;

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

        public Texture2D TextureGlow;
        public Texture2D NoteTexture;
        public Color[] NoteTextureColorData;
        public ScoreHandler Score;
        public int Lives;
        public bool Expired;
        public bool Hit;

        Vector2 _noteSpawnPoint;
        SoundEffect _noteFireSound;

        int _velocity = 5;
        int _explosionCount;
        float _scale = 1f;
        float _glowOpacity;
        double _noteSpawnTime;
        double _noteFireSoundTime;
        double _vibrationTime;
        bool _firingGamePad;
        bool _firingKeyboard;


        public Accordian(int identityNo, ActionBox box, Texture2D[] texture, Color[] textureColorData, Explosion[] explosions, Random explosionPointGen, SoundEffect explodeSound, SoundEffect noteFireSound, Vector2 location, int lives, int score)
        {
            ID = identityNo;
            Texture = texture[0];
            TextureGlow = texture[1];
            TextureColorData = textureColorData;
            Location = location;
            Lives = lives;
            Score = new ScoreHandler();
            Score.Value = score;
            Box = new ActionBox(box.PixelTexture, box.Color, (int)Location.X, (int)Location.Y, (int)(Texture.Width * _scale), (int)(Texture.Height * _scale));
            _noteSpawnPoint = new Vector2(0, location.Y);
            _noteFireSound = noteFireSound;
            base.SetupExplosions(explosions, explosionPointGen, explodeSound);
        }

        public void Update(GameTime gameTime, GraphicsDevice graphics, Playfield playfield, bool debugMode)
        {
            if (Explode == false)
            {
                if (Hit == true)
                {
                    if (Lives > 1)
                    {
                        VibrationManager.SetVibration(0.5f, 0.5f, 0.2);
                        Lives--;
                        Hit = false;
                    }
                    else
                    {
                        VibrationManager.SetVibration(0.8f, 0.8f, 0.4);
                        base.TriggerExpireExplosion();
                        playfield.GameOver = true;
                    }
                }

                // GamePad
                if (GamePad.GetState(PlayerIndex.One).IsConnected == true)
                {
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

                    if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0.8f)
                    {
                        NoteSpawnTrigger(gameTime, playfield);
                        _firingGamePad = true;
                    }
                    else
                    {
                        _firingGamePad = false;
                    }
                }

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
                    _firingKeyboard = true;
                }
                else
                {
                    _firingKeyboard = false;
                }
            }
            else
            {
                if (base.UpdateExplosions(gameTime) == true)
                {
                    Expired = true;
                }
            }

            // Final Changes
            if ((_firingGamePad == true || _firingKeyboard == true) && _glowOpacity < 1)
            {
                _glowOpacity += 0.1f;
            }
            else if (_firingGamePad == false && _firingKeyboard == false && _glowOpacity > 0)
            {
                _glowOpacity -= 0.1f;
            }

            base.UpdateBox(debugMode);
            _noteSpawnTime += gameTime.ElapsedGameTime.TotalSeconds;
        }
        void NoteSpawnTrigger(GameTime gameTime, Playfield playfield)
        {
            _noteSpawnPoint.X = Location.X + 35;
            if (_noteSpawnTime > 0.10)
            {
                _noteSpawnTime = 0;
                _noteFireSoundTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (_noteFireSoundTime > 0.01)
                {
                    _noteFireSound.Play();
                    _noteFireSoundTime = 0;
                }
                playfield.AddNote(_noteSpawnPoint, ID, false);
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
            if (Explode == false)
            {
                spriteBatch.Draw(Texture, Location, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.990f);
                spriteBatch.Draw(TextureGlow, Location, null, Color.White * _glowOpacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.990f);
            }
            else
            {
                base.DrawExplosions(spriteBatch);
            }
        }
    }
}
