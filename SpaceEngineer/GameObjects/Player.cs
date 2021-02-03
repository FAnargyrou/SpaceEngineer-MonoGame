using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using SpaceEngineer.GameObjects.Ship;

namespace SpaceEngineer
{
    public enum Direction 
    {
        Right,
        Left
    }

    public class Player : IEntity
    {
        // Movement Variables

        Vector2 position;
        Vector2 movement;
        Vector2 velocity;
        float movementSpeed = 100f;
        AnimatedSprite sprite;
        string currentAnimation;
        Direction currentDir;
        public float interactRad = 30f;

        // Camera
        OrthographicCamera camera;

        // Input helpers
        private MouseState _previousState;
        private MouseState _currentState;

        ShipComponent focus;

        public IShapeF Bounds { get; }

        public Player(SpriteSheet spriteSheet, Vector2 position, OrthographicCamera camera)
        {
            movement = new Vector2(0f, 0f);
            velocity = new Vector2(0f, 0f);
            this.position = position;
            sprite = new AnimatedSprite(spriteSheet);
            currentAnimation = "idleRight";
            sprite.Play(currentAnimation);
            this.camera = camera;
            Vector2 boundsStartPos = position;
            boundsStartPos.X -= sprite.TextureRegion.Width / 4;
            Bounds = new RectangleF(boundsStartPos, new Size2(sprite.TextureRegion.Width / 2, sprite.TextureRegion.Height / 2));
        }

        public void Update(GameTime gameTime)
        {
            Move((float)gameTime.ElapsedGameTime.TotalSeconds);

            Interact();
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position);
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red, 3);
        }

        private void Move(float deltaSeconds)
        {
            KeyboardState kstate = Keyboard.GetState();
            movement = new Vector2(0f, 0f);
            velocity = new Vector2(0f, 0f);

            if (kstate.IsKeyDown(Keys.W))
                movement.Y -= 1f;
            if (kstate.IsKeyDown(Keys.S))
                movement.Y += 1f;
            if (kstate.IsKeyDown(Keys.A))
                movement.X -= 1f;
            if (kstate.IsKeyDown(Keys.D))
                movement.X += 1f;

            if (movement.Length() > 0f)
                movement.Normalize();
            if (movement.X != 0f)
                velocity.X += movement.X * movementSpeed * deltaSeconds;
            if (movement.Y != 0f)
                velocity.Y += movement.Y * movementSpeed * deltaSeconds;

            // Moves player and collision box
            position += velocity;
            Bounds.Position += velocity;

            if (movement.X != 0f)
            {
                currentDir = movement.X < 0f ? Direction.Left : Direction.Right;
            }

            if (movement.X > 0f || movement.Y > 0f)
                currentAnimation = "walkRight";
            else if (movement.X < 0f || movement.Y < 0f)
                currentAnimation = "walkLeft";
            else
                currentAnimation = currentDir == Direction.Right ? currentAnimation = "idleRight" : "idleLeft";

            sprite.Play(currentAnimation);
            sprite.Update(deltaSeconds);
            camera.LookAt(position);
        }

        public void Interact()
        {
            _previousState = _currentState;
            _currentState = Mouse.GetState();
            if (focus != null)
            {
                float distance = Vector2.Distance(position, focus.GetPosition());
                if (distance > interactRad)
                {
                    // If player is too far from the Interactable Object (ie. ShipComponent)
                    // then cancel current action and reset focus to a null value to prevent it from being interacted again from a distance
                    focus.Cancel();
                    focus = null;
                    return;
                }
                
                if (_currentState.LeftButton == ButtonState.Released && _previousState.LeftButton == ButtonState.Pressed)
                    focus.Interact();

            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            // If collision is found, prevent player and collision box from moving through
            position -= collisionInfo.PenetrationVector;
            Bounds.Position -= collisionInfo.PenetrationVector;

            // TODO - Add collision detection to check if player is close enough to interact with a component
        }

        public void SetFocus(ShipComponent comp)
        {
            focus = comp;
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}
