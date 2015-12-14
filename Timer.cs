using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Trainwrecker {
    class Timer : DrawableObject {

        public int Progress { get; set; }
        public Vector2 Direction { get; set; }

        public Timer(Vector2 position) : base("Timer") {
            Position = position;
            Progress = 0;
        }

        
        public void DrawProgress(SpriteBatch spriteBatch) {
            for (int i = 0; i < Progress; i++) {
                spriteBatch.Draw(Texture, Position + i * Direction * 16, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, LayerDepth);
            }
        }

    }
}
