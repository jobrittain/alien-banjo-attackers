using ABAFS.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS.Physics
{
    public class CollisionDetector
    {
        public ActionBox IntersectBox;
        public ActionBox NearBox;
        public ActionBox TouchBox;

        public double NearBoxDrawTime = 0;
        public double IntersectBoxDrawTime = 0;
        public double TouchBoxDrawTime = 0;

        enum CollisionType
        {
            NoteToBanjo,
            NoteToPlayer,
            BanjoToPlayer
        }

        Playfield _playfield;

        // Near box (bounds) for being near a banjo
        int _nbBanjoMaxX = 20;
        int _nbBanjoMinX = -60;
        int _nbBanjoMaxY = 20;
        int _nbBanjoMinY = -90;

        int _nbPlayerMaxX = 50;
        int _nbPlayerMinX = -150;
        int _nbPlayerMaxY = 100;
        int _nbPlayerMinY = -80;

        int _secsToDraw = 3;

        public CollisionDetector(Playfield playfield, Texture2D pixel)
        {
            _playfield = playfield;

            // Create debug boxes
            IntersectBox = new ActionBox(pixel, Color.Red);
            NearBox = new ActionBox(pixel, Color.Green);
            TouchBox = new ActionBox(pixel, Color.Purple);
        }

        public void Detect(bool debugMode)
        {
            foreach (Banjo banjo in _playfield.Banjos)
            {
                if (banjo.Explode == false)
                {
                    if (_playfield.GameOver == false)
                    {
                        if (IsNear(_playfield.Player.Location, banjo.Location, CollisionType.BanjoToPlayer))
                        {
                            NearBox.X = _playfield.Player.Box.X - _nbPlayerMaxX;
                            NearBox.Y = _playfield.Player.Box.Y - _nbPlayerMaxY;
                            NearBox.Width = _nbPlayerMaxX - _nbPlayerMinX;
                            NearBox.Height = _nbPlayerMaxY - _nbPlayerMinY;

                            if (debugMode == true)
                            {
                                NearBox.UpdateDraw();
                                NearBoxDrawTime = _secsToDraw;
                            }

                            if (IsTouching(banjo.Box.Rectangle, _playfield.Player.Box.Rectangle, banjo.TextureColorData, _playfield.Player.TextureColorData, debugMode))
                            {
                                banjo.TriggerExpireExplosion();
                                _playfield.Player.Hit = true;
                            }

                        }
                    }

                    foreach (Note note in _playfield.Notes)
                    {
                        if (note.TargetPlayer == false)
                        {
                            if (IsNear(banjo.Location, note.Location, CollisionType.NoteToBanjo))
                            {
                                NearBox.X = banjo.Box.X - _nbBanjoMaxX;
                                NearBox.Y = banjo.Box.Y - _nbBanjoMaxY;
                                NearBox.Width = _nbBanjoMaxX - _nbBanjoMinX;
                                NearBox.Height = _nbBanjoMaxY - _nbBanjoMinY;

                                if (debugMode == true)
                                {
                                    NearBox.UpdateDraw();
                                    NearBoxDrawTime = _secsToDraw;
                                }

                                if (IsTouching(banjo.Box.Rectangle, note.Box.Rectangle, banjo.TextureColorData, note.TextureColorData, debugMode))
                                {
                                    banjo.Hit = true;
                                    note.Expired = true;
                                }

                            }
                        }
                        else
                        {
                            if (_playfield.GameOver == false)
                            {
                                if (IsNear(_playfield.Player.Location, note.Location, CollisionType.NoteToPlayer))
                                {
                                    NearBox.X = _playfield.Player.Box.X - _nbPlayerMaxX;
                                    NearBox.Y = _playfield.Player.Box.Y - _nbPlayerMaxY;
                                    NearBox.Width = _nbPlayerMaxX - _nbPlayerMinX;
                                    NearBox.Height = _nbPlayerMaxY - _nbPlayerMinY;

                                    if (debugMode == true)
                                    {
                                        NearBox.UpdateDraw();
                                        NearBoxDrawTime = _secsToDraw;
                                    }

                                    if (IsTouching(note.Box.Rectangle, _playfield.Player.Box.Rectangle, note.TextureColorData, _playfield.Player.TextureColorData, debugMode))
                                    {
                                        note.Expired = true;
                                        _playfield.Player.Hit = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        bool IsNear(Vector2 Targetlocation, Vector2 Projectilelocation, CollisionType type)
        {
            float xDiff = Targetlocation.X - Projectilelocation.X;
            float yDiff = Targetlocation.Y - Projectilelocation.Y;

            int _nbMaxX;
            int _nbMinX;
            int _nbMaxY;
            int _nbMinY;

            if (type == CollisionType.NoteToBanjo)
            {
                _nbMaxX = _nbBanjoMaxX;
                _nbMinX = _nbBanjoMinX;
                _nbMaxY = _nbBanjoMaxY;
                _nbMinY = _nbBanjoMinY;
            }
            else
            {
                _nbMaxX = _nbPlayerMaxX;
                _nbMinX = _nbPlayerMinX;
                _nbMaxY = _nbPlayerMaxY;
                _nbMinY = _nbPlayerMinY;
            }

            if (xDiff < _nbMaxX && xDiff > _nbMinX)
            {
                if (yDiff < _nbMaxY && yDiff > _nbMinY)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        bool IsTouching(Rectangle rectangleA, Rectangle rectangleB, Color[] colorsA, Color[] colorsB, bool debugMode)
        {
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            TouchBox.X = left;
            TouchBox.Y = top;
            TouchBox.Width = right - left;
            TouchBox.Height = bottom - top;
            if (debugMode == true)
            {
                TouchBox.UpdateDraw();
                TouchBoxDrawTime = _secsToDraw;
            }

            for (int yPix = top; yPix < bottom; yPix++)
            {
                for (int xPix = left; xPix < right; xPix++)
                {
                    Color colorBanjo = colorsA[(xPix - rectangleA.Left) + ((yPix - rectangleA.Top) * rectangleA.Width)];
                    Color colorNote = colorsB[(xPix - rectangleB.Left) + ((yPix - rectangleB.Top) * rectangleB.Width)];

                    if (colorBanjo.A != 0 && colorNote.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
