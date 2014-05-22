using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS.Screen
{
    public class ScreenSystem
    {

        public enum ScreenMode
        {
            Start,
            Main,
            Gameover
        }

        public ScreenMode Mode;
        public bool Active = true;

        enum GameoverMusicType
        {
            Normal,
            HighScore
        }

        GameoverMusicType _gameOverMusicType;

        Playfield _playfield;
        Menu _menu;

        Texture2D _mainTitleTexture;
        Texture2D _gameOverTitleTexture;

        SpriteFont _startTextFont;
        SpriteFont _highScoreFont;
        SpriteFont _gameOverInfoFont;
        SpriteFont _backTextFont;

        SoundEffectInstance _mainTitleMusic;
        SoundEffectInstance _gameOverMusic;
        SoundEffectInstance _gameOverHighScoreMusic;
        SoundEffectInstance _menuSelectSound;

        Vector2 _currentTitlePosition;
        Vector2 _startTitlePosition;
        Vector2 _startTextPosition;
        Vector2 _highScorePosition;
        Vector2 _menuTitlePosition;
        Vector2 _gameOverTextPosition;
        Vector2 _gameOverInfoPosition;
        Vector2 _backTextPosition;

        bool _startReleased = true;
        bool _updatedScores = false;
        bool _returnButtonDown = false;
        bool _startButtonDown = false;
        bool _musicStarted = false;
        bool _titleMoved = false;
        string _startMessage;
        string _backMessage;
        string _highScoreText;
        string _gameOverInfoScoreText;
        string _gameOverInfoHighScoreText;
        double _gameOverTime;
        int _backWaitTime = 2;

        float _alphaVal = 1f;
        float _alphaSinInput = 0f;


        public ScreenSystem(Playfield playfield, Menu menu,
            SoundEffect mainTitleMusic, SoundEffect gameOverMusic, SoundEffect gameOverHighScoreMusic,
            SoundEffect menuSelectSound,
            Texture2D mainTitleTexture, Vector2 startTitlePosition, Vector2 menuTitlePosition,
            SpriteFont startTextFont, Vector2 startTextPosition,
            SpriteFont highScoreFont, Vector2 highScorePosition,
            Texture2D gameOverTitleTexture, Vector2 gameOverTextPosition,
            SpriteFont gameOverInfoFont, Vector2 gameOverInfoPosition,
            SpriteFont backTextFont, Vector2 backTextPosition
            )
        {
            _playfield = playfield;
            _menu = menu;

            // Textures
            _mainTitleTexture = mainTitleTexture;
            _gameOverTitleTexture = gameOverTitleTexture;

            // Font
            _startTextFont = startTextFont;
            _highScoreFont = highScoreFont;
            _gameOverInfoFont = gameOverInfoFont;
            _backTextFont = backTextFont;

            // Music
            _mainTitleMusic = mainTitleMusic.CreateInstance();
            _mainTitleMusic.IsLooped = true;
            _gameOverMusic = gameOverMusic.CreateInstance();
            _gameOverMusic.IsLooped = true;
            _gameOverHighScoreMusic = gameOverHighScoreMusic.CreateInstance();
            _gameOverHighScoreMusic.IsLooped = true;

            // Sound
            _menuSelectSound = menuSelectSound.CreateInstance();

            // Positions
            _currentTitlePosition = startTitlePosition;
            _startTitlePosition = startTitlePosition;
            _startTextPosition = startTextPosition;
            _highScorePosition = highScorePosition;
            _menuTitlePosition = menuTitlePosition;
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

        double _moveTitleWaitTime = 0.1;

        public void Update(GameTime gameTime, ref bool gameActive, ref bool exitTriggered, ref bool autoSaved, bool newHighScore)
        {
            // Scores
            if (_updatedScores == false)
            {
                if (newHighScore == true)
                {
                    _highScoreText = "Highscore " + _playfield.Player.Score.ToString();
                    _gameOverInfoHighScoreText = "\nNew High Score!";
                    _updatedScores = true;
                }
                else
                {
                    _gameOverInfoHighScoreText = "\nHigh Score " + _playfield.HighScore.ToString();
                }
            }

            // Music
            if (_musicStarted == false)
            {
                if (Mode == ScreenMode.Start)
                {
                    _mainTitleMusic.Play();
                }
                else if (Mode == ScreenMode.Gameover)
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

            // Behaviour
            if (Mode == ScreenMode.Start)
            {
                _alphaVal = UsefulFunctions.GetSineAlphaVal(_alphaSinInput);
                _alphaSinInput += 0.01f;

                if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (_returnButtonDown == false)
                    {
                        _menuSelectSound.Play();

                        Mode = ScreenMode.Main;
                    }
                }
                else
                {
                    _returnButtonDown = false;
                }

            }
            else if (Mode == ScreenMode.Main)
            {

                if (_titleMoved == false)
                {
                    _moveTitleWaitTime += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_moveTitleWaitTime > 0)
                    {
                        _currentTitlePosition.Y--;
                        if (_currentTitlePosition.Y <= _menuTitlePosition.Y)
                        {
                            _titleMoved = true;
                        }
                    }
                }
                else
                {
                    _menu.Update(gameTime);
                    if (_menu.ButtonSelected == true)
                    {
                        switch (_menu.SelectedButtonIndex)
                        {
                            case 0:
                                GameStart(true, ref autoSaved, ref gameActive);
                                break;

                            case 1:
                                GameStart(false, ref autoSaved, ref gameActive);
                                break;

                            case 2:
                                exitTriggered = true;
                                break;
                        }
                    }
                }
            }
            else // Gameover
            {
                if (_gameOverTime > _backWaitTime)
                {
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
                            _currentTitlePosition = _startTitlePosition;
                            _titleMoved = false;

                            gameActive = false;
                            _menu.Reset();

                            Mode = ScreenMode.Start;

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

        void GameStart(bool newGame, ref bool autoSaved, ref bool gameActive)
        {
            if (newGame == true)
            {
                _playfield.Reset();
            }
            _mainTitleMusic.Stop();
            _musicStarted = false;
            _updatedScores = false;

            autoSaved = false;
            Active = false;
            gameActive = true;
            Mode = ScreenMode.Gameover;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Mode == ScreenMode.Start)
            {
                spriteBatch.Draw(_mainTitleTexture, _currentTitlePosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(_startTextFont, _startMessage, _startTextPosition, Color.White * _alphaVal, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(_highScoreFont, _highScoreText, _highScorePosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            else if (Mode == ScreenMode.Main)
            {
                spriteBatch.Draw(_mainTitleTexture, _currentTitlePosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                if (_titleMoved == true)
                {
                    _menu.Draw(spriteBatch);
                }
            }
            else // Gameover
            {
                spriteBatch.Draw(_gameOverTitleTexture, _gameOverTextPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(_gameOverInfoFont, _gameOverInfoScoreText + _gameOverInfoHighScoreText, _gameOverInfoPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

                if (_gameOverTime > _backWaitTime)
                {
                    spriteBatch.DrawString(_backTextFont, _backMessage, _backTextPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            }
        }
    }
}
