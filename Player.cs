using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainwrecker {
    struct PositionData {
        public Vector2 position;
        public float rotation;
        public PositionData(Vector2 position, float rotation) {
            this.position = position;
            this.rotation = rotation;
        }
    }
    

    class Player : DrawableObject {

        public Vector2 Direction { get; set; }
        public Vector2 LinkTarget { get { return Position - Direction * 128; } }
        private const float ROTATION_SPEED = 3f;
        private const float SPEED = 10;
        private const int KEY_DELAY = 25;
        public List<Link> LinkList { get; private set; }
        public List<Cart> CartList { get; private set; }
        public List<Bullet> BulletList { get; set; }
        public Turret turret { get; set; }
        private List<PositionData> positionList;
        private int cartNumber;
        public int CurrentCart { get; set; }
        private int keyTimer;
        private Timer timer;
        private bool dReleased = true;
        private bool aReleased = true;
        private Random r;
        private int fireCD;
        private int cartFireIndex;
        private int turretFireIndex;
        public bool dead = false;
        public bool hurt = false;

        public Player(Vector2 position) : base("Train") {
            Position = position;
            Direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            LinkList = new List<Link>();
            CartList = new List<Cart>();
            BulletList = new List<Bullet>();
            AddCart();
            positionList = new List<PositionData>();
            turret = new Turret(Position - new Vector2(64,0));
            CurrentCart = 0;
            timer = new Timer(Vector2.Zero);
            r = new Random();
            fireCD = 0;
            cartFireIndex = 0;
            turretFireIndex = 0;
        }


        public void Update(Vector2 target, KeyboardState kb, KeyboardState prevKb, MouseState mb, Camera camera) {
            /*
            float targetRotation = (float)Math.Atan2(target.Y - Position.Y, target.X - Position.X);
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
            if (kb.IsKeyDown(Keys.A)) { Rotation -= MathHelper.ToRadians(ROTATION_SPEED); }
            if (kb.IsKeyDown(Keys.D)) { Rotation += MathHelper.ToRadians(ROTATION_SPEED); }
            if (kb.IsKeyDown(Keys.Space) && fireCD == 0) {

                int tries = 0;
                cartFireIndex = r.Next(0, CartList.Count);
                turretFireIndex = r.Next(0, 3);
                while (CartList[cartFireIndex].turretList[turretFireIndex].reloading) {
                    cartFireIndex = r.Next(0, CartList.Count);
                    turretFireIndex = r.Next(0, 3);
                    tries++;
                    if(tries >= CartList.Count * 6) { break; }
                }
                if (tries < CartList.Count * 6) {
                    BulletList.Add(CartList[cartFireIndex].turretList[turretFireIndex].Fire(new Vector2(mb.X - 640, mb.Y - 360) / camera.Zoom + camera.Position));
                    Resource<SoundEffect>.Get("Laser").Play();
                }
                /*
                foreach (Cart cart in CartList) {
                    foreach (Bullet bullet in cart.Fire(new Vector2(mb.X - 640, mb.Y - 360) / camera.Zoom + camera.Position)) {
                        BulletList.Add(bullet);
                    }
                }
                */
                fireCD = 10;
            }
            if(fireCD > 0) { fireCD--; }
            
            /*
            if (kb.IsKeyDown(Keys.A) && prevKb.IsKeyUp(Keys.A)) { keyTimer = 0; }
            if (kb.IsKeyDown(Keys.A) && aReleased) {
                keyTimer++;
                timer.Progress = 1 + keyTimer / (KEY_DELAY / 5);
                timer.Position = CartList[CurrentCart].Position + -CartList[CurrentCart].Direction * 18 + new Vector2(-CartList[CurrentCart].Direction.Y, CartList[CurrentCart].Direction.X) * 60;
                timer.Direction = -CartList[CurrentCart].Direction;
                timer.Rotation = CartList[CurrentCart].Rotation;
            }
            if ((kb.IsKeyUp(Keys.A) && prevKb.IsKeyDown(Keys.A) && aReleased) || keyTimer == KEY_DELAY) {
                timer.Progress = 0;
                if (!prevKb.IsKeyUp(Keys.A)) { aReleased = false; }
                if (keyTimer >= KEY_DELAY) {

                    if (CurrentCart < CartList.Count - 1) { CurrentCart++; }
                }
                else {
                    foreach (Bullet bullet in CartList[CurrentCart].Fire(Directions.Left)) {
                        BulletList.Add(bullet);
                    }
                }
                keyTimer = 0;
            }
            if (kb.IsKeyDown(Keys.D) && prevKb.IsKeyUp(Keys.D)) { keyTimer = 0; }
            if (kb.IsKeyDown(Keys.D) && dReleased) {
                keyTimer++;
                timer.Progress = 1 + keyTimer / (KEY_DELAY / 5);
                timer.Position = CartList[CurrentCart].Position + CartList[CurrentCart].Direction * 38 + new Vector2(-CartList[CurrentCart].Direction.Y, CartList[CurrentCart].Direction.X) * 60;
                timer.Direction = CartList[CurrentCart].Direction;
                timer.Rotation = CartList[CurrentCart].Rotation;
            }
            if ((kb.IsKeyUp(Keys.D) && prevKb.IsKeyDown(Keys.D) && dReleased) || keyTimer == KEY_DELAY) {
                timer.Progress = 0;
                if (!prevKb.IsKeyUp(Keys.D)) { dReleased = false; }
                if (keyTimer >= KEY_DELAY) {
                    
                    if (CurrentCart > 0) { CurrentCart--; }
                }
                else {
                    foreach (Bullet bullet in CartList[CurrentCart].Fire(Directions.Right)) {
                        BulletList.Add(bullet);
                    }
                }
                keyTimer = 0;
            }
            if (!aReleased && prevKb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.A)) { aReleased = true; }
            if (!dReleased && prevKb.IsKeyUp(Keys.D) && kb.IsKeyUp(Keys.A)) { dReleased = true; }
            */

            //if (kb.IsKeyDown(Keys.Space) && prevKb.IsKeyUp(Keys.Space)) { AddCart(); }

            Direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            for (int i = 0; i < SPEED; i++) {
                Position += Direction;
                positionList.Add(new PositionData(Position, Rotation));
            }
            turret.Position = Position - Direction * 64;
            turret.Update(Rotation, kb, mb, camera);
            foreach (Link link in LinkList) {
                link.Update(positionList);
            }
            foreach (Cart cart in CartList) {
                cart.Update(positionList, kb, mb, camera);
            }

            if(positionList.Count > 256 * (CartList.Count + 1)) {
                positionList.RemoveRange(0, positionList.Count - 256 * (CartList.Count + 1));
            }
            foreach (Bullet bullet in BulletList) {
                bullet.Update();
            }


            for (int i = 0; i < CartList.Count; i++) {
                if (CartList[i].dead) {
                    CartList[i].dead = false;
                    CartList.RemoveAt(CartList.Count - 1);
                    LinkList.RemoveAt(LinkList.Count - 1);
                    cartNumber--;
                }
            }

            if (hurt) {
                hurt = false;
                CartList.RemoveAt(CartList.Count - 1);
                LinkList.RemoveAt(LinkList.Count - 1);
                cartNumber--;
            }

            for (int i = 0; i < BulletList.Count; i++) {
                if (BulletList[i].dead) {
                    BulletList.RemoveAt(i);
                    i--;
                }
            }
            if(CartList.Count == 0) { dead = true; }
            Refresh();
        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);
            foreach (Link link in LinkList) {
                link.Draw(spriteBatch);
            }
            foreach (Cart cart in CartList) {
                cart.Draw(spriteBatch);
            }
            foreach (Bullet bullet in BulletList) {
                bullet.Draw(spriteBatch);
            }
            turret.Draw(spriteBatch);
            //timer.DrawProgress(spriteBatch);
        }

        public void AddCart() {
            LinkList.Add(new Link(LinkTarget, cartNumber++));
            CartList.Add(new Cart(LinkTarget, cartNumber));
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
