using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainwrecker {
    class Link : DrawableObject {
        int cartNumber;

        public Link(Vector2 position, int linkNumber) : base("Link") {
            Position = position;
            Scale = 0;
            this.cartNumber = linkNumber;
        }
        public virtual void Update(List<PositionData> positionList) {
            if (Scale < 1) {
                Scale += 0.05f;
            }
                if (positionList.Count >= 128 + 256 * cartNumber) {
                Position = positionList[positionList.Count - 128 - 256 * cartNumber].position;
                Rotation = positionList[positionList.Count - 128 - 256 * cartNumber].rotation;
            }
        }
    }
}
