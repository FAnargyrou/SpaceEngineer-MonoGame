using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using SpaceEngineer.Hud;

namespace SpaceEngineer.GameObjects.Ship
{
    /// <summary>
    /// An extension of BreakableComponent; Medbay behaves differently from other ship objects.
    /// Not only the player can fix it, the player should also be able to heal if this is not broken
    /// </summary>
    public class Medbay : BreakableComponent
    {
        public Medbay(Sprite sprite, Vector2 position, ItemType requiredItem = ItemType.Screwdriver, string name = "DefaultComponent", Sprite brokenSprite = null) 
            : base(sprite, position, requiredItem, name, brokenSprite)
        {
        }

        public delegate void OnComponentInteractedLogic();
        public OnComponentInteractedLogic OnComponentInteracted;

        public override void Interact()
        {
            if (!IsBroken() && OnComponentInteracted != null)
                OnComponentInteracted.Invoke();
            else if (IsBroken())
                base.Interact();
        }
    }
}
