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
        private float _fixTime = 100f;
        private float _currentFixTime = 0f;
        private float _fixRate = 10f;
        private ItemType _requiredItem;

        public override void Cancel()
        {
            // TODO - Add cancel action when player moves away
        }

        public BreakableComponent(Sprite sprite, Vector2 position, ItemType requiredItem) : base(sprite, position)
        {
            _requiredItem = requiredItem;
        }

        public override void Interact()
        {
            Console.WriteLine("Interacting...");
            // TODO = Add interaction logic

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
                    _currentFixTime = 0f;
                }
            }
        }

        public ItemType GetRequiredItem()
        {
            return _requiredItem;
        }

        public void BreakComponent(bool toggle)
        {
            _isBroken = toggle;
        }
    }
}
