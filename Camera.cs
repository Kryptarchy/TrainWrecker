using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainwrecker {
    public class Camera {
        private Viewport view;
        private Vector2 position;
        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }
        public float Zoom { get { return (float)Math.Pow(zoom, 10); } set { zoom = value; } }
        private Vector2 savedPosition;
        private Vector2 focusPoint;
        private float zoom;
        public float Rotation { get; set; }
        private float savedRotation;
        private float positionShakeAmount;
        private float rotationShakeAmount;
        private float maxShakeTime;
        private Matrix transform;
        public Matrix Transform {
            get { return transform; }
        }
        private TimeSpan shaketimer;
        private Random random;

        public Camera(Viewport view, Vector2 position) {
            this.view = view;
            this.position = position;
            zoom = 1;
            Rotation = 0;
            random = new Random();
            focusPoint = new Vector2(view.Width / 2, view.Height / 2);
        }

        public void Update(GameTime gametime) {
            if (shaketimer.TotalSeconds > 0) {
                focusPoint = savedPosition;
                Rotation = savedRotation;

                shaketimer = shaketimer.Subtract(gametime.ElapsedGameTime);

                if (shaketimer.TotalSeconds > 0) {
                    focusPoint += new Vector2((float)((random.NextDouble() * 2) - 1) * positionShakeAmount,
                        (float)((random.NextDouble() * 2) - 1) * rotationShakeAmount);
                    Rotation += (float)((random.NextDouble() * 2) - 1) * rotationShakeAmount;
                }
            }
            Vector2 objectPosition = position;
            float objectRotation = Rotation;
            float deltaRotation = 0;
            transform = Matrix.CreateTranslation(new Vector3(-objectPosition, 0)) *
                Matrix.CreateScale(new Vector3((float)Math.Pow(zoom, 10), (float)Math.Pow(zoom, 10), 0)) *
                Matrix.CreateRotationZ(-objectRotation + deltaRotation) *
                Matrix.CreateTranslation(new Vector3(focusPoint.X, focusPoint.Y, 0));
        }

        public void Control(KeyboardState keyboard, KeyboardState prevKeyboard, MouseState mouse, MouseState prevMouse) {
            if (keyboard.IsKeyDown(Keys.Up)) {
                position += new Vector2(0, -10) / Zoom;
            }
            if (keyboard.IsKeyDown(Keys.Down)) {
                position += new Vector2(0, 10) / Zoom;
            }
            if (keyboard.IsKeyDown(Keys.Left)) {
                position += new Vector2(-10, 0) / Zoom;
            }
            if (keyboard.IsKeyDown(Keys.Right)) {
                position += new Vector2(10, 0) / Zoom;
            }
            if (mouse.ScrollWheelValue < prevMouse.ScrollWheelValue) {
                Vector2 mouseOld = new Vector2(mouse.X - 640, mouse.Y - 360) / Zoom + position;
                zoom -= 0.01f;
                Vector2 mouseNew = new Vector2(mouse.X - 640, mouse.Y - 360) / Zoom + position;
                position += mouseOld - mouseNew;
            }
            if (mouse.ScrollWheelValue > prevMouse.ScrollWheelValue) {
                Vector2 mouseOld = new Vector2(mouse.X - 640, mouse.Y - 360) / Zoom + position;
                zoom += 0.01f;
                Vector2 mouseNew = new Vector2(mouse.X - 640, mouse.Y - 360) / Zoom + position;
                position += mouseOld - mouseNew;
            }
        }

        public void Shake(float shakeTime, float positionAmount, float rotationAmount) {
            if (shaketimer.TotalSeconds <= 0) {
                maxShakeTime = shakeTime;
                shaketimer = TimeSpan.FromSeconds(maxShakeTime);
                positionShakeAmount = positionAmount;
                rotationShakeAmount = rotationAmount;

                savedPosition = focusPoint;
                savedRotation = Rotation;
            }
        }
    }    
}
