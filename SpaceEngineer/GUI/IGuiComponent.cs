using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceEngineer.GUI
{
    public interface IGuiComponent
    {
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch);
        public Vector2 GetPosition();
        public void SetPosition(Vector2 position);
        public Size2 GetSize();
        public void SetScale(Vector2 scale);
    }
}
