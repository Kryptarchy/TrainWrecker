using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainwrecker {
    class Arrow : DrawableObject{
        private const float DISTANCE_FROM_PLAYER = 150;

        public Arrow(Vector2 position) : base("Arrow") {
            Position = position;
        }

        public void Update(Player player, Powerup powerUp) {
            Vector2 direction = powerUp.Position - player.Position;
            direction.Normalize();
            Position = player.Position + direction * DISTANCE_FROM_PLAYER;
            Rotation = (float)Math.Atan2(powerUp.Position.Y - player.Position.Y, powerUp.Position.X - player.Position.X);
        }
    }
}
