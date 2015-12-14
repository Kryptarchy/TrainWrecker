using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainwrecker {
    class Powerup : DrawableObject {
        private const float ROTATION_SPEED = 10;
        private bool growing;
        public Powerup(Vector2 position) : base("PowerUp") {
            Scale = 0;
            Position = position;
            Refresh();
            growing = true;
        }

        public void Update() {
            Rotation += MathHelper.ToRadians(ROTATION_SPEED);
            if(Scale < 2 && growing == true) {
                Scale += 0.025f;
                if((int)Scale == 2) {
                    growing = false;
                }
            }
            else {
                Scale -= 0.025f;
                if((int)(Scale * 10) == 5) {
                    growing = true;
                }
            }
            
        }
    }
}
