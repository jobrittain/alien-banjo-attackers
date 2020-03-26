using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS.Physics
{
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
}
