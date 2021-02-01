using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;

namespace SpaceEngineer.GameObjects.Ship
{
    public abstract class ShipComponent : IEntity
    {
        protected Sprite _sprite;
        protected Vector2 _position;

        public virtual IShapeF Bounds { get; }
        public abstract void Cancel();
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Interact();
        public abstract void Update(GameTime gameTime);

        public Vector2 GetPosition()
        {
            return _position;
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            
        }
    }
}
