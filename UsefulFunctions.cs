using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS
{
    public class UsefulFunctions
    {
        public enum CenterPointType
        {
            X,
            Y
        }

        public static float GetTextureCenterPoint(Rectangle parentBounds, Vector2 location, Texture2D texture, CenterPointType pointType)
        {
            float textureCenter;
            if (pointType == CenterPointType.X)
            {
                textureCenter = (parentBounds.Width / 2) - (texture.Width / 2) + location.X;
            }
            else
            {
                textureCenter = (parentBounds.Height / 2) - (texture.Height / 2) + location.Y;
            }
            return textureCenter;
        }

        public static float GetSpriteFontCenterPoint(Rectangle parentBounds, Vector2 location, SpriteFont spriteFont, string text, CenterPointType pointType)
        {
            float spriteFontCenter;
            if (pointType == CenterPointType.X)
            {
                spriteFontCenter = (parentBounds.Width / 2) - (spriteFont.MeasureString(text).X / 2) + location.X;
            }
            else
            {
                spriteFontCenter = (parentBounds.Height / 2) - (spriteFont.MeasureString(text).Y / 2) + location.Y;
            }
            return spriteFontCenter;
        }

        public static float GetSineAlphaVal(float val, double phase = 0)
        {
            return 1 - ((float)Math.Pow(Math.Sin(((val + phase) * 1.7)), 2) * 0.8f);
        }

    }
}
