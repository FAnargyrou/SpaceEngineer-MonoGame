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

        public ShipComponent(Sprite sprite, Vector2 position)
        {
            _position = position;
            _sprite = sprite;


            // Translates position into Transform and then converts that to a RectangleF; 
            // Used to get correct sprite bounds for collision detection
            Transform2 transform = new Transform2(position);
            RectangleF rectangle = _sprite.GetBoundingRectangle(transform);

            Bounds = rectangle;
        }

        public virtual IShapeF Bounds { get; }
        public abstract void Cancel();
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, _position);
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Blue);
        }
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
