﻿using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceEngineer.Hud
{
    public enum ItemType
    {
        Screwdriver,
        Wrench,
        O2Filter,
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
