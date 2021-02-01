using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceEngineer.GUI
{
    public enum Orientation
    {
        Vertical,
        Horizontal
    }
    public class StackPanel : IGuiComponent
    {
        private readonly List<IGuiComponent> _childComponents = new List<IGuiComponent>();
        private RectangleF _panelSize;
        private Orientation _orientation;
        private float _margin = 0f;
        private Size2 _scale;


        public StackPanel(float x, float y, float width, float height, float margin = 0f, Orientation orientation = Orientation.Horizontal)
        {
            _panelSize = new RectangleF(x, y, width, height);
            _orientation = orientation;
            _margin = margin;
        }
        public StackPanel(Vector2 position, Size2 size, float margin = 0f, Orientation orientation = Orientation.Horizontal)
        {
            _panelSize = new RectangleF(position, size);
            _orientation = orientation;
            _margin = margin;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (IGuiComponent child in _childComponents)
                child.Draw(spriteBatch);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            foreach (IGuiComponent child in _childComponents)
            {
                child.Update(gameTime);
            }
        }

        public void AddComponent(IGuiComponent component)
        {
            if (component == null) return;

            Vector2 newPosition = GetPosition();

            foreach (IGuiComponent child in _childComponents)
            {
                Size2 childSize = child.GetSize();

                if (_orientation == Orientation.Horizontal)
                {
                    newPosition.X += _margin + childSize.Width;
                }
                else
                {
                    newPosition.Y += _margin + childSize.Height;
                }
            }

            component.SetPosition(newPosition);
            _childComponents.Add(component);
        }

        public Vector2 GetPosition()
        {
            return new Vector2(_panelSize.X, _panelSize.Y);
        }

        public void SetPosition(Vector2 position)
        {
            _panelSize.X = position.X;
            _panelSize.Y = position.Y;
        }

        public Size2 GetSize()
        {
            return new Size2(_panelSize.Width, _panelSize.Height);
        }

        private void UpdateSize()
        {
            // TODO - Update size based on _childComponents
        }

        public void SetScale(Vector2 scale)
        {
            _scale = scale;
        }
    }
}
