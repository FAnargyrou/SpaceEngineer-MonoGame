using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using SpaceEngineer.GameObjects;
using SpaceEngineer.GUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceEngineer.Hud
{
    public class InventorySlot
    {
        Item _item;
        Inventory _inventory;
        public Button button;

        public InventorySlot(Sprite btnSprite, Inventory inventory, Vector2 scale)
        {
            button = new Button(btnSprite, Vector2.Zero);
            button.SetScale(scale);
            button.OnClick += Button_OnClick;
            _inventory = inventory;
        }

        private void Button_OnClick(object sender, EventArgs e)
        {
            if (_item == null || _inventory == null) return;
            _inventory.RemoveItem(_item);
        }

        public void AssignItem(Item item)
        {
            _item = item;
            button.AddIconSprite(item.itemSprite);
            button.SetActive(true);
        }

        public void ClearItem()
        {
            _item = null;
            button.AddIconSprite(null);
            button.SetActive(true);
        }
    }
}
