using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainwrecker {
    class Cart : DrawableObject {
        protected int cartNumber;
        public List<Turret> turretList;
        public Vector2 Direction { get; set; }
        public bool dead;
        public Cart(Vector2 position ,int cartNumber) : base("Cart") {
            Position = position;
            Scale = 0;
            this.cartNumber = cartNumber;
            turretList = new List<Turret>();
            turretList.Add(new Turret(Position));
            turretList.Add(new Turret(Position));
            turretList.Add(new Turret(Position));

        }

        public virtual void Update(List<PositionData> positionList, KeyboardState kb, MouseState mb, Camera camera) {
            if(Scale < 1) {
                Scale += 0.05f;
                if((int)Scale == 1) {
                    turretList[0].Scale = 1;
                    turretList[1].Scale = 1;
                    turretList[2].Scale = 1;
                }
            }
            if (positionList.Count >= 256 * cartNumber) {
                Position = positionList[positionList.Count - 256 * cartNumber].position;
                Rotation = positionList[positionList.Count - 256 * cartNumber].rotation;
            }
            Direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            if (turretList.Count > 0) {
                turretList[0].Position = Position - Direction * 64;
                turretList[1].Position = Position;
                turretList[2].Position = Position + Direction * 64;
                foreach (Turret turret in turretList) {
                    turret.Update(Rotation, kb, mb, camera);
                }
            }
            Refresh();

        }

        public List<Bullet> Fire(Directions dir) {
            List<Bullet> bulletList = new List<Bullet>();
            foreach (Turret turret in turretList) {
                Bullet bullet = turret.Fire(dir, Direction);
                if (bullet != null) {
                    bulletList.Add(bullet);
                }
            }
            return bulletList;
        }
        public List<Bullet> Fire(Vector2 target) {
            List<Bullet> bulletList = new List<Bullet>();
            foreach (Turret turret in turretList) {
                if (!turret.reloading) {
                    bulletList.Add(turret.Fire(target));
                }
            }
            return bulletList;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);
            foreach (Turret turret in turretList) {
                turret.Draw(spriteBatch);
            }
        }

        
    }
}
