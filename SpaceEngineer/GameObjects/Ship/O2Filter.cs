using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using SpaceEngineer.Hud;

namespace SpaceEngineer.GameObjects.Ship
{
    public class O2Filter : BreakableComponent
    {
        public O2Filter(Sprite sprite, Vector2 startingPosition, ItemType requiredItem) : base(sprite, startingPosition, requiredItem)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Interact()
        {
            base.Interact();
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
