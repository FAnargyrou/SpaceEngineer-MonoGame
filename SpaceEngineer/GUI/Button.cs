using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceEngineer.GUI
{
    public class Button : IGuiComponent
    {
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private bool _isHovering;
        private Sprite _btnSprite;
        private Vector2 _position;
        private RectangleF _rectangle;
        private Vector2 _scale;

        public string text;

        // Button click delegate
        public event EventHandler OnClick;
        


        public Button(Sprite sprite, Vector2 position)
        {
            _position = position;
            _btnSprite = sprite;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color colour = Color.White;

            if (_isHovering)
                colour = Color.Gray;

            _btnSprite.Color = colour;

            spriteBatch.Draw(_btnSprite, _position, 0f, _scale);
            spriteBatch.DrawRectangle(_rectangle, Color.Red);
        }

        public void Update(GameTime gameTime)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            RectangleF mouseRect = new RectangleF(_currentMouseState.X, _currentMouseState.Y, 1f, 1f);

            _isHovering = false;

            if (mouseRect.Intersects(_rectangle))
            {
                _isHovering = true;

                if (_currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    OnClick?.Invoke(this, new EventArgs());
                }
            }
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public void SetPosition(Vector2 position)
        {
            /*float widthScale = _btnSprite.TextureRegion.Width * _scale.X;
            float heightScale = _btnSprite.TextureRegion.Height * _scale.Y;
            position.X += widthScale;
            position.Y += heightScale;*/
            _position = position;
            CreateRectangle();
        }

        public Size2 GetSize()
        {
            Size2 size = new Size2();
            size.Width = _btnSprite.TextureRegion.Width * _scale.X;
            size.Height = _btnSprite.TextureRegion.Height * _scale.Y;
            return size;
        }

        public void SetScale(Vector2 scale)
        {
            _scale = scale;
            CreateRectangle();
        }

        private void CreateRectangle()
        {
            Transform2 transform = new Transform2(_position);
            transform.Scale = _scale;
            _rectangle = _btnSprite.GetBoundingRectangle(transform);
        }
    }
}
