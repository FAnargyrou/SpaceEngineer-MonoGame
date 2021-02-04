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
using SpaceEngineer.Hud;

namespace SpaceEngineer.GameObjects
{
    public enum Direction 
    {
        Right,
        Left
    }

    public class Player : IEntity
    {
        // Movement Variables

        private Vector2 _position;
        private Vector2 _movement;
        private Vector2 _velocity;
        private float _movementSpeed = 100f;
        private AnimatedSprite _sprite;
        private string _currentAnimation;
        private Direction _currentDir;
        public float interactRad = 30f;

        // Camera
        OrthographicCamera _camera;

        // Input helpers
        private MouseState _previousState;
        private MouseState _currentState;

        private ShipComponent _focus;
        private Inventory _inventory;

        public IShapeF Bounds { get; }

        public Player(SpriteSheet spriteSheet, Vector2 position, OrthographicCamera camera)
        {
            _movement = new Vector2(0f, 0f);
            _velocity = new Vector2(0f, 0f);
            _position = position;
            _sprite = new AnimatedSprite(spriteSheet);
            _currentAnimation = "idleRight";
            _sprite.Play(_currentAnimation);
            _camera = camera;
            Vector2 boundsStartPos = position;
            boundsStartPos.X -= _sprite.TextureRegion.Width / 4;
            Bounds = new RectangleF(boundsStartPos, new Size2(_sprite.TextureRegion.Width / 2, _sprite.TextureRegion.Height / 2));
        }

        public void Update(GameTime gameTime)
        {
            Move((float)gameTime.ElapsedGameTime.TotalSeconds);

            Interact();
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, _position);
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red, 3);
        }

        private void Move(float deltaSeconds)
        {
            KeyboardState kstate = Keyboard.GetState();
            _movement = new Vector2(0f, 0f);
            _velocity = new Vector2(0f, 0f);

            if (kstate.IsKeyDown(Keys.W))
                _movement.Y -= 1f;
            if (kstate.IsKeyDown(Keys.S))
                _movement.Y += 1f;
            if (kstate.IsKeyDown(Keys.A))
                _movement.X -= 1f;
            if (kstate.IsKeyDown(Keys.D))
                _movement.X += 1f;

            if (_movement.Length() > 0f)
                _movement.Normalize();
            if (_movement.X != 0f)
                _velocity.X += _movement.X * _movementSpeed * deltaSeconds;
            if (_movement.Y != 0f)
                _velocity.Y += _movement.Y * _movementSpeed * deltaSeconds;

            // Moves player and collision box
            _position += _velocity;
            Bounds.Position += _velocity;

            if (_movement.X != 0f)
            {
                _currentDir = _movement.X < 0f ? Direction.Left : Direction.Right;
            }

            if (_movement.X > 0f || _movement.Y > 0f)
                _currentAnimation = "walkRight";
            else if (_movement.X < 0f || _movement.Y < 0f)
                _currentAnimation = "walkLeft";
            else
                _currentAnimation = _currentDir == Direction.Right ? _currentAnimation = "idleRight" : "idleLeft";

            _sprite.Play(_currentAnimation);
            _sprite.Update(deltaSeconds);
            _camera.LookAt(_position);
        }

        public void Interact()
        {
            _previousState = _currentState;
            _currentState = Mouse.GetState();
            if (_focus != null)
            {
                float distance = Vector2.Distance(_position, _focus.GetPosition());
                if (distance > interactRad)
                {
                    // If player is too far from the Interactable Object (ie. ShipComponent)
                    // then cancel current action and reset focus to a null value to prevent it from being interacted again from a distance
                    _focus.Cancel();
                    _focus = null;
                    return;
                }

                if (_currentState.LeftButton == ButtonState.Released && _previousState.LeftButton == ButtonState.Pressed)
                {
                    if (_focus is BreakableComponent b)
                    {
                        Item item = _inventory.items.Find(i => i.itemType == b.GetRequiredItem());
                        if (item == null) return;
                    }
                    _focus.Interact();
                }

            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            // If collision is found, prevent player and collision box from moving through
            _position -= collisionInfo.PenetrationVector;
            Bounds.Position -= collisionInfo.PenetrationVector;

            // TODO - Add collision detection to check if player is close enough to interact with a component
        }

        public void SetFocus(ShipComponent comp)
        {
            _focus = comp;
        }

        public void SetInventory(Inventory inventory)
        {
            _inventory = inventory;
        }

        public Vector2 GetPosition()
        {
            return _position;
        }
    }
}
