using MonoGame.Extended.Sprites;

namespace SpaceEngineer.Hud
{
    public enum ItemType
    {
        Screwdriver,
        Wrench,
        Filter,
        Drill
    }

    public class Item
    {
        public Sprite itemSprite { get; }
        public ItemType itemType { get; }
        public Item(Sprite sprite, ItemType type)
        {
            itemSprite = sprite;
            itemType = type;
        }
    }
}
