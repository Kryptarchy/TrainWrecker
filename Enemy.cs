using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainwrecker {
    class Enemy : DrawableObject{

        public Vector2 Target { get; set; }
        public bool dead = false;
        private const float SPEED = 7;
        public Enemy(Vector2 position) : base("Enemy") {
            Position = position;
        }

        public void Update() {
            Vector2 direction = Target - Position;
            direction.Normalize();
            Position += direction * SPEED;
            Refresh();
        }
    }
}
