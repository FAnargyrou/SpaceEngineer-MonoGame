using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceEngineer.Hud
{
    public class Item
    {
        public Sprite itemSprite { get; }
        public Item(Sprite sprite)
        {
            itemSprite = sprite;
        }
    }
}
