using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;

namespace SpaceEngineer
{
    // GameObject Interface; Inherits ICollisionActor from MonoGame.Extended
    public interface IEntity : ICollisionActor
    {
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

    }
}