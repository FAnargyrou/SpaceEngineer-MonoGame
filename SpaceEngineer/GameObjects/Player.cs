using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using SpaceEngineer.GameObjects.Ship;
using SpaceEngineer.GUI;
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
        private Vector2 _position;
        private Vector2 _movement;
        private Vector2 _velocity;
        private float _movementSpeed = 100f;
        private float _noGravityMovementSpeed = 3f;
        private AnimatedSprite _sprite;
        private string _currentAnimation;
        // Current direction the player sprite should be looking at
        private Direction _currentDir;

        private float _maxHp = 20f;
        private float _currentHp;

        // Timer for next damage (in seconds)
        private float _damageTimer = 5f;
        private float _currentDamageTimer = 0f;

        // Interact trigger radius
        public float interactRad = 30f;
        // Player's interactable object if one exists within 'interactRad'
        private ShipComponent _focus;

        // Camera
        OrthographicCamera _camera;

        // Input helpers
        private MouseState _previousState;
        private MouseState _currentState;

        private Inventory _inventory;

        private ProgressBar _fixProgress;
        // True = O2 is working; False = O2 needs fixing
        private bool _o2status = true;
        private bool _gravityStatus = true;

        public IShapeF Bounds { get; }

        public Player(SpriteSheet spriteSheet, Vector2 position, OrthographicCamera camera, ProgressBar fixProgress)
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
            _fixProgress = fixProgress;
            Vector2 progressBarPos = _fixProgress.GetPosition();
            progressBarPos.Y -= _sprite.TextureRegion.Height;
            _fixProgress.SetPosition(progressBarPos);
            _fixProgress.SetActive(false);
            _currentHp = _maxHp;
        }

        public void Update(GameTime gameTime)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Move(deltaSeconds);
            Interact();
            UpdateDamage(deltaSeconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, _position);
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red, 3);

            _fixProgress.Draw(spriteBatch);
        }

        private void Move(float deltaSeconds)
        {
            KeyboardState kstate = Keyboard.GetState();

            _movement = new Vector2(0f, 0f);

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

            if (_gravityStatus)
            {
                _velocity = new Vector2(0f, 0f);

                if (_movement.X != 0f)
                    _velocity.X += _movement.X * _movementSpeed * deltaSeconds;
                if (_movement.Y != 0f)
                    _velocity.Y += _movement.Y * _movementSpeed * deltaSeconds;
            }
            else
            {
                if (_movement.X != 0f)
                    _velocity.X += _movement.X * _noGravityMovementSpeed * deltaSeconds;
                if (_movement.Y != 0f)
                    _velocity.Y += _movement.Y * _noGravityMovementSpeed * deltaSeconds;
            }

            // Moves player, collision box and Progress Bar
            _position += _velocity;
            Bounds.Position += _velocity;
            Vector2 progressBarPos = _position;
            progressBarPos.Y -= _sprite.TextureRegion.Height;
            // progressBarPos.X -= _sprite.TextureRegion.Width; 
            _fixProgress.SetPosition(progressBarPos);

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
                    _fixProgress.SetActive(false);
                    return;
                }

                BreakableComponent b = _focus as BreakableComponent;

                if (_currentState.LeftButton == ButtonState.Released && _previousState.LeftButton == ButtonState.Pressed)
                {
                    if (b != null)
                    {
                        if (b.IsBroken())
                        {
                            Item item = _inventory.items.Find(i => i.itemType == b.GetRequiredItem());
                            if (item == null) return;
                            _fixProgress.SetActive(true);
                        }
                    }
                    _focus.Interact();
                }

                if (b != null)
                {
                    _fixProgress.UpdateProgress(b.GetFixProgress());
                }

            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            // If collision is found, prevent player and collision box from moving through
            _position -= collisionInfo.PenetrationVector;
            Bounds.Position -= collisionInfo.PenetrationVector;
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

        /// <summary>
        /// Toggles O2 status for the player between true and false
        /// </summary>
        /// <param name="toggle">True = O2 is fixed and working; False = O2 needs fixing and player will lose Health</param>
        public void ToggleO2(bool toggle)
        {
            _o2status = toggle;
        }

        private void UpdateDamage(float deltaSeconds)
        {
            // If _o2status is true, then O2 filters are OK, therefore, we do not deduct health from the player
            if (_o2status) return;

            _currentDamageTimer = Math.Clamp(_currentDamageTimer + deltaSeconds, 0f, _damageTimer);
            if (_currentDamageTimer >= _damageTimer)
            {
                _currentHp = Math.Clamp(_currentHp - 1, 0, _maxHp);
                _currentDamageTimer = 0f;
            }
        }

        public void HealDamage()
        {
            _currentHp = _maxHp;
        }

        /// <summary>
        /// Get player health in percentage
        /// </summary>
        /// <returns>Returns _currentHp / _maxHp</returns>
        public float GetHealthPercentage()
        {
            return _currentHp / _maxHp;
        }

        /// <summary>
        /// Toggles Gravity status for the player between true and false
        /// </summary>
        /// <param name="toggle">True = Gravity is fixed and working; False = Gravity needs fixing and player movement will be affected</param>
        public void ToggleGravity(bool toggle)
        {
            _gravityStatus = toggle;
        }
    }
}
