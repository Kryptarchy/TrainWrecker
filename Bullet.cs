using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainwrecker {
    class Bullet : DrawableObject {
        public bool dead = false;
        private const float SPEED = 20;
        private Vector2 Direction { get; set; }
        public Bullet(Vector2 position, Vector2 direction, float rotation) : base("Bullet") {
            Position = position;
            Rotation = rotation;
            direction.Normalize();
            Direction = direction;
            
        }

        public void Update() {
            Position += Direction * SPEED;
            Refresh();
        }
    }
}
