using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace SpaceEngineer.GameObjects
{
    // Class used for TileMap walls
    // TODO - Implement class and create alongside Collision layer
    public class Wall : IEntity
    {
        public IShapeF Bounds { get; }

        public Wall(Vector2 position, Size2 size)
        {
            RectangleF r = new RectangleF(position, size);
            Bounds = r;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Blue);
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            return;
        }
    }
}
