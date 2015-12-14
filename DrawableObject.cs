using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainwrecker {
    class DrawableObject {
        public virtual Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        protected Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        protected Color Color { get; set; }
        protected float LayerDepth { get; set; }

        public Matrix Transform { get; protected set; }
        public Color[] TextureData { get; protected set; }
        public Rectangle BoundingBox { get; protected set; }

        public DrawableObject(string texture) {
            Texture = Resource<Texture2D>.Get(texture);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Rotation = 0;
            Scale = 1;
            Color = Color.White;
            LayerDepth = 0.5f;
            Transform =
                    Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateTranslation(new Vector3(Position, 0.0f));
            BoundingBox =
                new Rectangle(0, 0, Texture.Width, Texture.Height);
            TextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
            
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
            if (Texture != null) {
                spriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, LayerDepth);
            }
            //spriteBatch.Draw(Resource<Texture2D>.Get("Bullet"), new Vector2(BoundingBox.X, BoundingBox.Y), Color.Black);
            //spriteBatch.Draw(Resource<Texture2D>.Get("Bullet"), new Vector2(BoundingBox.X + BoundingBox.Width, BoundingBox.Y), Color.Black);
            //spriteBatch.Draw(Resource<Texture2D>.Get("Bullet"), new Vector2(BoundingBox.X, BoundingBox.Y + BoundingBox.Height), Color.Black);
            //spriteBatch.Draw(Resource<Texture2D>.Get("Bullet"), new Vector2(BoundingBox.X + BoundingBox.Width, BoundingBox.Y + BoundingBox.Height), Color.Black);
        }

        public Vector2[] Corners {
            get {
                return new Vector2[] {
                    Position - Origin,
                    Position + Origin,
                    new Vector2(Position.X - Origin.X, Position.Y + Origin.Y),
                    new Vector2(Position.X + Origin.X, Position.Y - Origin.Y)
                };
            }
        }

        protected static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform) {
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop), Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop), Vector2.Max(leftBottom, rightBottom));

            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public void Refresh() {
            Transform =
                    Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateTranslation(new Vector3(Position + Origin, 0.0f));

            BoundingBox = CalculateBoundingRectangle(
                new Rectangle(0, 0, Texture.Width, Texture.Height),
                Transform);
            BoundingBox = new Rectangle(BoundingBox.X - (int)Origin.X,
                                        BoundingBox.Y - (int)Origin.Y,
                                        BoundingBox.Width,
                                        BoundingBox.Height);
        }
    }
}
