﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using SpaceEngineer.Hud;

namespace SpaceEngineer.GameObjects.Ship
{
    public class BreakableComponent : ShipComponent
    {
        private bool _isFixing = false;
        private bool _isBroken = false;
        private float _fixTime = 50f;
        private float _currentFixTime = 0f;
        private float _fixRate = 10f;
        private ItemType _requiredItem;

        public override void Cancel()
        {
            _currentFixTime = 0f;
            _isFixing = false;
        }

        public BreakableComponent(Sprite sprite, Vector2 position, ItemType requiredItem) : base(sprite, position)
        {
            _requiredItem = requiredItem;
            _isBroken = true;
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
                }
            }
        }

        public ItemType GetRequiredItem()
        {
            return _requiredItem;
        }

        public virtual void ActivateEvent()
        {
            _isBroken = true;
            _currentFixTime = 0f;
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
            Console.WriteLine(_currentFixTime / _fixTime);
            return _currentFixTime / _fixTime;
        }
    }
}
