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
    public class Menu
    {
        public class MenuItem
        {
            public Vector2 Position;
            public Vector2 TextPosition;
            public bool Blank;
            public string Text;


            public MenuItem(bool blank, string text, Vector2 position, Vector2 textPosition)
            {
                Blank = blank;
                Position = position;
                TextPosition = textPosition;
                Text = text;
            }

        }

        enum Direction
        {
            Up, Down
        }

        public Vector2 Location;
        public int SelectedButtonIndex;
        public bool ButtonSelected;

        public List<MenuItem> MenuItems;

        Texture2D[] _buttonTexture;
        SpriteFont _buttonFont;

        SoundEffect _buttonMoveSound;
        SoundEffect _buttonSelectSound;

        float _spacing;
        bool _keyDown;
        bool _keyUp;
        bool _thumbStickDown;
        bool _thumbStickUp;
        bool _buttonPressed;
        bool _buttonSelectSoundPlayed;
        float _buttonSelectorAlpha = 1f;
        double _buttonPressEffectTime;
        double _buttonPressEffectCompleteTime;

        public Menu(Vector2 location, float spacing, Texture2D[] buttonTexture, SpriteFont buttonFont, SoundEffect moveSound, SoundEffect selectSound)
        {
            MenuItems = new List<MenuItem>();
            Location = location;
            _spacing = spacing;
            _buttonTexture = buttonTexture;
            _buttonFont = buttonFont;
            _buttonMoveSound = moveSound;
            _buttonSelectSound = selectSound;
        }

        public void AddItem(string buttonText, bool blank = false)
        {
            Vector2 buttonPosition;
            int itemCount = MenuItems.Count;

            if (itemCount != 0)
            {
                buttonPosition = new Vector2(Location.X, MenuItems[itemCount - 1].Position.Y + _spacing);
            }
            else
            {
                buttonPosition = new Vector2(Location.X, Location.Y);
            }

            MenuItems.Add(new MenuItem(blank, buttonText, buttonPosition,
                new Vector2(
                    UsefulFunctions.GetSpriteFontCenterPoint(_buttonTexture[0].Bounds, buttonPosition, _buttonFont, buttonText, UsefulFunctions.CenterPointType.X),
                    UsefulFunctions.GetSpriteFontCenterPoint(_buttonTexture[0].Bounds, buttonPosition, _buttonFont, buttonText, UsefulFunctions.CenterPointType.Y)
                    )));
        }

        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                // Down
                if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -0.8f && _thumbStickDown == false)
                {
                    Move(Direction.Down);
                    _thumbStickDown = true;
                }
                else if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > -0.2f)
                {
                    _thumbStickDown = false;
                }

                // Up
                if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0.8f && _thumbStickUp == false)
                {
                    Move(Direction.Up);
                    _thumbStickUp = true;
                }
                else if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0.2f)
                {
                    _thumbStickUp = false;
                }

                // Select
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                    _buttonPressed = true;
                }
            }

            // Down
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (_keyDown == false)
                {
                    Move(Direction.Down);
                    _keyDown = true;
                }
            }
            else
            {
                _keyDown = false;
            }

            // Up
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (_keyUp == false)
                {
                    Move(Direction.Up);
                    _keyUp = true;
                }
            }
            else
            {
                _keyUp = false;
            }

            // Select
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _buttonPressed = true;
            }

            if (_buttonPressed == true)
            {
                if (_buttonSelectSoundPlayed == false)
                {
                    _buttonSelectSound.Play();
                    _buttonSelectSoundPlayed = true;
                }
                FlashSelector(gameTime);
                _buttonPressEffectCompleteTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (_buttonPressEffectCompleteTime > 0.6)
                {
                    ButtonSelected = true;
                }
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
                    else
                    {
                        SelectedButtonIndex = 0;
                    }
                }
                else
                {
                    if (SelectedButtonIndex - 1 > -1)
                    {
                        SelectedButtonIndex--;
                    }
                    else
                    {
                        SelectedButtonIndex = MenuItems.Count - 1;
                    }
                }

                if (MenuItems[SelectedButtonIndex].Blank == true)
                {
                    ammount++;
                }

                ammount--;
            }
            _buttonMoveSound.Play();
        }
        void FlashSelector(GameTime gameTime)
        {
            _buttonPressEffectTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (_buttonPressEffectTime > 0.02)
            {
                if (_buttonSelectorAlpha == 0f)
                {
                    _buttonSelectorAlpha = 1f;
                }
                else
                {
                    _buttonSelectorAlpha = 0f;
                }
                _buttonPressEffectTime = 0;
            }
        }

        public void Reset()
        {
            _buttonPressEffectTime = 0;
            _buttonPressEffectCompleteTime = 0;
            _buttonPressed = false;
            _buttonSelectSoundPlayed = false;
            ButtonSelected = false;
            SelectedButtonIndex = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MenuItem menuItem in MenuItems)
            {
                if (menuItem.Blank == false)
                {
                    spriteBatch.Draw(_buttonTexture[0], menuItem.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    spriteBatch.DrawString(_buttonFont, menuItem.Text, menuItem.TextPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            }
            spriteBatch.Draw(_buttonTexture[1], MenuItems[SelectedButtonIndex].Position, null, Color.White * _buttonSelectorAlpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
    }
}
