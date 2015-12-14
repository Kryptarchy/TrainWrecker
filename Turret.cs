using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainwrecker {
    class Turret : DrawableObject {

        private const float ROTATION_SPEED = 2;
        public bool reloading;
        public List<Bullet> BulletList { get; set; }
        private int cd;
        private Vector2 mousePosition;

        public Turret(Vector2 position) : base("Bullet") {
            Position = position;
            reloading = false;
            BulletList = new List<Bullet>();
            Scale = 0;
        }

        public void Update(float rotation, KeyboardState kb, MouseState mb, Camera camera) {
            float targetRotation = (float)Math.Atan2((mb.Y - 360) / camera.Zoom + camera.Position.Y - Position.Y, (mb.X - 640) / camera.Zoom + camera.Position.X - Position.X);
            mousePosition = new Vector2(mb.X - 640, mb.Y - 360) / camera.Zoom + camera.Position;
            targetRotation = RotationWrap(targetRotation);
            /*
            if (!(Rotation < targetRotation + MathHelper.ToRadians(ROTATION_SPEED) && Rotation > targetRotation - MathHelper.ToRadians(ROTATION_SPEED))) {

                float minVal1 = Rotation - MathHelper.ToRadians(180);
                float minVal2 = RotationWrap(minVal1);
                float maxVal1 = Rotation + MathHelper.ToRadians(180);
                float maxVal2 = RotationWrap(maxVal1);

                if ((targetRotation > minVal1 && targetRotation < Rotation) || minVal1 != minVal2 && (targetRotation > minVal2 && targetRotation < MathHelper.ToRadians(180))) {
                    Rotation -= MathHelper.ToRadians(ROTATION_SPEED);
                }
                else {
                    Rotation += MathHelper.ToRadians(ROTATION_SPEED);
                }
                Rotation = RotationWrap(Rotation);
            }
            */

            Rotation = rotation;

            if (reloading) {
                cd--;
                if(cd == 0) {
                    reloading = false;
                    Rotation = 0;
                }
            }
            


            /*
            if (Rotation > targetRotation - MathHelper.ToRadians(ROTATION_SPEED) &&
                Rotation < targetRotation + MathHelper.ToRadians(ROTATION_SPEED) &&
                !reloading) {
                BulletList.Add(new Bullet(Position, mousePosition - Position, Rotation));
                reloading = true;
                cd = 100;
            }
            */
            foreach (Bullet bullet in BulletList) {
                bullet.Update();
            }
        }

        public Bullet Fire(Directions dir, Vector2 direction) {
            if (!reloading) {
                if (dir == Directions.Left) {
                    reloading = true;
                    cd = 100;
                    return new Bullet(Position, new Vector2(direction.Y, -direction.X), Rotation);
                }
                if (dir == Directions.Right) {
                    reloading = true;
                    cd = 100;
                    return new Bullet(Position, new Vector2(-direction.Y, direction.X), Rotation);
                }
            }
            return null;
        }

        public Bullet Fire(Vector2 target) {
            reloading = true;
            cd = 100;
            Vector2 direction = target - Position;
            direction.Normalize();
            float rotation = (float)Math.Atan2(target.Y - Position.Y, target.X - Position.X);
            return new Bullet(Position, direction, rotation);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            if (!reloading) {
                base.Draw(spriteBatch);
            }
        }
        private float RotationWrap(float rotation) {
            if (rotation > MathHelper.ToRadians(180)) {
                rotation -= MathHelper.ToRadians(360);
            }
            if (rotation < MathHelper.ToRadians(-180)) {
                rotation += MathHelper.ToRadians(360);
            }

            return rotation;
        }
    }
}
