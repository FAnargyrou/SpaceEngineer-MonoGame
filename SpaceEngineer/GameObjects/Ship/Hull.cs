using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using SpaceEngineer.Hud;

namespace SpaceEngineer.GameObjects.Ship
{
    public class Hull : BreakableComponent
    {
        private bool _hasCollision;
        public Hull(Sprite sprite, Vector2 position, ItemType requiredItem = ItemType.Screwdriver, string name = "DefaultComponent", Sprite brokenSprite = null) 
            : base(sprite, position, requiredItem, name, brokenSprite)
        {
            RemoveCollisionBounds();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IsBroken() && _hasCollision)
                RemoveCollisionBounds();
        }

        public override void ActivateEvent()
        {
            base.ActivateEvent();
            AddCollisionBounds();

        }

        private void RemoveCollisionBounds()
        {
            Bounds = RectangleF.Empty;
            _hasCollision = false;
        }

        private void AddCollisionBounds()
        {
            CreateBoundsShape();
            _hasCollision = true;
        }

    }
}
