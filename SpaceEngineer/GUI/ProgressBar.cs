using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceEngineer.GUI
{
    public class ProgressBar : IGuiComponent
    {
        private Sprite _sprite;
        private Sprite _borderSprite;
        private RectangleF _progressRect;
        private Vector2 _position;
        private Vector2 _scale;
        // Progress from 0f to 1f
        private float _maxWidth;
        private float _progress = 1f;
        private bool _active = true;

        public ProgressBar(Sprite sprite, Vector2 position, Vector2 scale, Sprite borderSprite = null)
        { 
            _sprite = sprite;
            _position = position;
            _scale = scale;
            _maxWidth = _sprite.TextureRegion.Width;
            _borderSprite = borderSprite;
            Transform2 transform = new Transform2(Vector2.Zero);
            transform.Scale = _scale;
            _progressRect = _sprite.GetBoundingRectangle(transform);
            _progressRect.X = 0f;
            _progressRect.Y = 0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_active) return;
            spriteBatch.Draw(_sprite.TextureRegion.Texture, _position, _progressRect.ToRectangle(), Color.White, 0f, _sprite.Origin, _scale, SpriteEffects.None, 0);
            spriteBatch.Draw(_borderSprite, _position, 0f, _scale);
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public Size2 GetSize()
        {
            Size2 size = new Size2();
            size.Width = _sprite.TextureRegion.Width * _scale.X;
            size.Height = _sprite.TextureRegion.Height * _scale.Y;
            return size;
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void SetScale(Vector2 scale)
        {
            _scale = scale;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        private void UpdateBar()
        {
            float progress = _progress * _maxWidth;
            _progressRect.Width = progress;
        }

        public void UpdateProgress(float progress)
        {
            _progress = progress;
            UpdateBar();
        }

        /// <summary>
        /// Sets the bar visibility
        /// </summary>
        /// <param name="toogle">True for visible; False for not visible</param>
        public void SetActive(bool toggle)
        {
            _active = toggle;
        }
    }
}
