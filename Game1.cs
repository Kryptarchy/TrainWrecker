using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text;

namespace Trainwrecker {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        Player player;
        Color backgroundColor = Color.Black;//new Color(150, 60, 60);
        KeyboardState kb;
        KeyboardState prevKb;
        MouseState mb;
        MouseState prevMb;
        Powerup powerUp;
        Random r;
        List<Enemy> enemyList;
        GameState gameState;
        SpriteFont font;
        int score;
        int wave;
        Arrow arrow;
        Texture2D cursor;
        BlendState customBlend = new BlendState() {
            ColorBlendFunction = BlendFunction.Add,
            AlphaBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.InverseDestinationColor,
            AlphaSourceBlend = Blend.InverseDestinationColor,
            AlphaDestinationBlend = Blend.InverseSourceColor,
            ColorDestinationBlend = Blend.InverseSourceColor
        };

        public Game1() {
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Resource<Texture2D>.Set("Train", Content.Load<Texture2D>("Train"));
            Resource<Texture2D>.Set("Cart", Content.Load<Texture2D>("Cart"));
            Resource<Texture2D>.Set("Link", Content.Load<Texture2D>("Link"));
            Resource<Texture2D>.Set("Bullet", Content.Load<Texture2D>("Bullet"));
            Resource<Texture2D>.Set("Timer", Content.Load<Texture2D>("Timer"));
            Resource<Texture2D>.Set("Enemy", Content.Load<Texture2D>("Enemy"));
            Resource<Texture2D>.Set("PowerUp", Content.Load<Texture2D>("PowerUp"));
            Resource<Texture2D>.Set("Cursor", Content.Load<Texture2D>("Cursor"));
            Resource<Texture2D>.Set("Arrow", Content.Load<Texture2D>("Arrow"));
            Resource<SoundEffect>.Set("Explosion", Content.Load<SoundEffect>("Explosion"));
            Resource<SoundEffect>.Set("Laser", Content.Load<SoundEffect>("Laser"));
            Resource<SoundEffect>.Set("PowerUpSfx", Content.Load<SoundEffect>("PowerUpSfx"));
            Resource<SpriteFont>.Set("Font", Content.Load<SpriteFont>("Font"));
            font = Resource<SpriteFont>.Get("Font");
            cursor = Resource<Texture2D>.Get("Cursor");
            r = new Random();
            
            Restart();

            // TODO: use this.Content to load your game content here
        }

        private void Restart() {
            player = new Player(new Vector2(640, 360));
            camera = new Camera(GraphicsDevice.Viewport, player.CartList[player.CurrentCart].Position);
            camera.Zoom = 0.9f;
            enemyList = new List<Enemy>();
            gameState = GameState.Tutorial;
            GeneratePowerUp();
            SpawnEnemies(5);
            score = 0;
            wave = 1;
            arrow = new Arrow(player.Position);
        }

        private void GeneratePowerUp() {
            float rotation = MathHelper.ToRadians(r.Next(0, 360));
            Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            powerUp = new Powerup(player.Position + direction * 2000);
        }

        private void SpawnEnemies(int amount) {
            for (int i = 0; i < amount; i++) {
                float rotation = MathHelper.ToRadians(r.Next(0, 360));
                Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                enemyList.Add(new Enemy(player.Position + direction * 4000));
                
            }
            wave++;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            kb = Keyboard.GetState();
            mb = Mouse.GetState();
            switch (gameState) {
                case GameState.Tutorial:
                    if (kb.IsKeyDown(Keys.Space) && prevKb.IsKeyUp(Keys.Space)) { gameState = GameState.Playing; }
                    break;
                case GameState.Playing:
                    player.Update(powerUp.Position, kb, prevKb, mb, camera);
                    powerUp.Update();
                    arrow.Update(player, powerUp);
                    if (Collision(player, powerUp, false)) {
                        GeneratePowerUp();
                        player.AddCart();
                        Resource<SoundEffect>.Get("PowerUpSfx").Play();
                    }

                    foreach (Enemy enemy in enemyList) {
                        enemy.Target = player.Position;
                        if(Collision(enemy, player)) {
                            player.hurt = true;
                            enemy.dead = true;
                            Resource<SoundEffect>.Get("Explosion").Play();
                        }
                        foreach (Cart cart in player.CartList) {
                            if (Vector2.Distance(enemy.Position, cart.Position) < Vector2.Distance(enemy.Position, enemy.Target)) {
                                enemy.Target = cart.Position;
                            }
                            if (Collision(enemy, cart)) {
                                enemy.dead = true;
                                cart.dead = true;
                                Resource<SoundEffect>.Get("Explosion").Play();
                            }
                        }
                        foreach (Bullet bullet in player.BulletList) {
                            if (Collision(bullet, enemy, false)) {
                                bullet.dead = true;
                                enemy.dead = true;
                                camera.Shake(0.5f, 20, 0.05f);
                                score += 50 * player.CartList.Count;
                                Resource<SoundEffect>.Get("Explosion").Play();
                            }
                        }
                        enemy.Update();
                    }

                    for (int i = 0; i < enemyList.Count; i++) {
                        if (enemyList[i].dead) {
                            enemyList.RemoveAt(i);
                            i--;
                        }
                    }
                    camera.Position = player.Position;

                    if (enemyList.Count < player.CartList.Count) {
                        SpawnEnemies(player.CartList.Count * 2 + wave * 2);
                    }
                    //camera.Position = player.CartList[player.CurrentCart].Position;
                    //camera.Rotation = player.CartList[player.CurrentCart].Rotation + MathHelper.ToRadians(90);
                    camera.Update(gameTime);
                    if (player.dead) { gameState = GameState.GameOver; }
                    break;
                case GameState.GameOver:

                    if(kb.IsKeyDown(Keys.Space) && prevKb.IsKeyUp(Keys.Space)) {
                        Restart();
                    }
                    break;
            }
            
            prevKb = kb;
            prevMb = mb;
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(backgroundColor);
            switch (gameState) {
                case GameState.Tutorial:
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "HOW TO PLAY:", new Vector2(548, 220), Color.White);
                    spriteBatch.DrawString(font, "A - Turn Left:", new Vector2(538, 238), Color.White);
                    spriteBatch.DrawString(font, "D - Turn Right:", new Vector2(530, 256), Color.White);
                    spriteBatch.DrawString(font, "Space - Shoot:", new Vector2(538, 274), Color.White);
                    spriteBatch.DrawString(font, "Mouse - Aim:", new Vector2(553, 292), Color.White);
                    spriteBatch.DrawString(font, "- PRESS SPACE TO START -", new Vector2(450, 370), Color.White);
                    spriteBatch.End();
                    break;
                case GameState.Playing:
                    spriteBatch.Begin(SpriteSortMode.Deferred, customBlend, null, null, null, null, camera.Transform);
                    powerUp.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    arrow.Draw(spriteBatch);
                    foreach (Enemy enemy in enemyList) {
                        enemy.Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "SCORE:" + score.ToString().PadLeft(6, '0'), new Vector2(20, 20), Color.White);
                    spriteBatch.DrawString(font, "WAVE:" + wave.ToString().PadLeft(2, '0'), new Vector2(1100, 20), Color.White);
                    spriteBatch.Draw(cursor, new Vector2(mb.X, mb.Y), null, Color.White, 0, new Vector2(cursor.Width / 2, cursor.Height / 2), 1, SpriteEffects.None, 0);
                    spriteBatch.End();
                    break;
                case GameState.GameOver:
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "GAME OVER!", new Vector2(560, 305), Color.White);
                    spriteBatch.DrawString(font, "FINAL SCORE:", new Vector2(546, 325), Color.White);
                    spriteBatch.DrawString(font, score.ToString().PadLeft(6, '0'), new Vector2(586, 345), Color.White);
                    spriteBatch.DrawString(font, "- PRESS SPACE TO START -", new Vector2(450, 370), Color.White);
                    spriteBatch.End();
                    break;
                default:
                    break;
            }
            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        static bool Collision(DrawableObject a, DrawableObject b, bool pixelPerfect) {
            if (a.BoundingBox.Intersects(b.BoundingBox)) {
                if (pixelPerfect) {
                    return PixelPerfectCollision(a, b);
                }
                return true;
            }
            return false;
        }

        static bool Collision(DrawableObject a, DrawableObject b) {
            if (a.BoundingBox.Intersects(b.BoundingBox)) {
                return PixelPerfectCollision(a, b);
            }
            return false;
        }

        static bool PixelPerfectCollision(DrawableObject a, DrawableObject b) {

            Matrix transformA = a.Transform;
            Matrix transformB = b.Transform;
            int widthA = a.Texture.Width;
            int heightA = a.Texture.Height;
            int widthB = b.Texture.Width;
            int heightB = b.Texture.Height;
            Color[] dataA = a.TextureData;
            Color[] dataB = b.TextureData;

            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < heightA; yA++) {
                Vector2 posInB = yPosInB;

                for (int xA = 0; xA < widthA; xA++) {
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);
                    if (0 <= xB && xB < widthB &&
                       0 <= yB && yB < heightB) {
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        if (colorA.A != 0 && colorB.A != 0) {
                            return true;
                        }
                    }

                    posInB += stepX;
                }

                yPosInB += stepY;

            }
            return false;
        }
    }
}
