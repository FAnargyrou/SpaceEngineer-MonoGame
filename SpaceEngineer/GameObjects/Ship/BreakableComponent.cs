using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using SpaceEngineer.Hud;

namespace SpaceEngineer.GameObjects.Ship
{
    public class BreakableComponent : ShipComponent
    {
        private Sprite _brokenSprite;
        private bool _isFixing = false;
        private bool _isBroken = false;
        private float _fixTime = 50f;
        private float _currentFixTime = 0f;
        private float _fixRate = 10f;
        private ItemType _requiredItem;
        public string name { get; protected set; }

        public delegate void OnComponentActivatedLogic();
        public OnComponentActivatedLogic OnComponentActivated;
        public delegate void OnComponentFixedLogic();
        public OnComponentFixedLogic OnComponentFixed;

        public override void Cancel()
        {
            _currentFixTime = 0f;
            _isFixing = false;
        }
        /// <summary>
        /// Constructor for BreakableComponent class
        /// </summary>
        /// <param name="sprite">Component's base sprite</param>
        /// <param name="position">Component's starting position</param>
        /// <param name="requiredItem">Component's required Item; This can also be set through SetRequiredItem function</param>
        /// <param name="name">Component's name as shown to the player; Can also be set through SetName</param>
        public BreakableComponent(Sprite sprite, Vector2 position, ItemType requiredItem = ItemType.Screwdriver, string name = "DefaultComponent", Sprite brokenSprite = null) : base(sprite, position)
        {
            _requiredItem = requiredItem;
            _brokenSprite = brokenSprite;
            this.name = name;
        }

        public override void Interact()
        {
            _isFixing = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (_isBroken && _isFixing)
            {
                float fix = _fixRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _currentFixTime = Math.Clamp(_currentFixTime + fix, 0f, _fixTime);

                if (_currentFixTime >= _fixTime)
                {
                    _isBroken = false;
                    _isFixing = false;
                    if (OnComponentFixed != null)
                        OnComponentFixed.Invoke();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_brokenSprite == null || (_brokenSprite != null && !_isBroken))
                base.Draw(spriteBatch);
            else
                spriteBatch.Draw(_brokenSprite, _position);

        }

        public ItemType GetRequiredItem()
        {
            return _requiredItem;
        }

        public virtual void ActivateEvent()
        {
            _isBroken = true;
            _currentFixTime = 0f;
            if (OnComponentActivated != null)
                OnComponentActivated.Invoke();
        }

        /// <summary>
        /// Gets _isBroken value
        /// </summary>
        /// <returns>True if component needs fixing; false if not</returns>
        public bool IsBroken()
        {
            return _isBroken;
        }

        /// <summary>
        /// Gets fix progress in percentage
        /// </summary>
        /// <returns>return value between 0f and 1f</returns>
        public float GetFixProgress()
        {
            return _currentFixTime / _fixTime;
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        public void SetRequiredItem(ItemType type)
        {
            _requiredItem = type;
        }
    }
}
