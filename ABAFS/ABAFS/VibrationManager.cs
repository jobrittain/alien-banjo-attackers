using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABAFS
{
    public class VibrationManager
    {
        static double _vibrationTime;
        static bool _vibrationActive;

        public static void SetVibration(float leftMotor, float rightMotor, double time)
        {
            if (_vibrationActive == false && GamePad.GetState(PlayerIndex.One).IsConnected == true)
            {
                GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
                _vibrationTime = time;
                _vibrationActive = true;
            }
        }

        public static void Update(GameTime gameTime)
        {
            if (_vibrationActive == true)
            {
                _vibrationTime -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_vibrationTime <= 0)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    _vibrationTime = 0;
                    _vibrationActive = false;
                }
            }
        }
    }
}
