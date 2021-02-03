using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using SpaceEngineer.GameObjects;
using SpaceEngineer.GUI;

namespace SpaceEngineer.Hud
{
    public class InventoryHUD
    {

        private StackPanel _inventoryHud;
        private Inventory _inventory;
        private List<InventorySlot> _slots;
        private Vector2 _scale;
        private bool _active = true;
        private Sprite _slotSprite;

        public InventoryHUD(Vector2 position, Sprite slotSprite, Vector2 scale, Inventory inventory, float margin = 10f)
        {
            Size2 size = new Size2(0f, 0f);
            _inventoryHud = new StackPanel(position, size, margin);

            _inventory = inventory;
            _inventory.OnItemChangedCallback += UpdateUI;

            _slotSprite = slotSprite;
            _scale = scale;

            _slots = new List<InventorySlot>();
            for (int i = 1; i <= _inventory.slots; ++i)
            {
                InventorySlot slot = new InventorySlot(slotSprite, _inventory, _scale);
                _slots.Add(slot);
            }

            CreateUI();
        }

        public void Update(GameTime gameTime)
        {
            if (!_active) return;
            _inventoryHud.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_active) return;
            _inventoryHud.Draw(spriteBatch);
        }

        private void CreateUI()
        {
            for (int i = 0; i < _inventory.slots; ++i)
            {
                if (i < _inventory.items.Count)
                    _slots[i].AssignItem(_inventory.items[i]);
                else
                    _slots[i].ClearItem();
                _inventoryHud.AddComponent(_slots[i].button);
            }
        }

        // Only called when items are changed; There is no need to update this every frame
        private void UpdateUI()
        {
            for (int i = 0; i < _inventory.slots; ++i)
            {
                if (i < _inventory.items.Count)
                    _slots[i].AssignItem(_inventory.items[i]);
                else
                    _slots[i].ClearItem();
            }
        }

        /// <summary>
        /// Toggles the entire HUD to be shown.
        /// </summary>
        /// <param name="toggle">True everything is rendered and updated; False nothing is rendered or updated</param>
        public void SetActive(bool toggle)
        {
            _active = toggle;
        }

        /// <summary>
        /// Toggles the inventory buttons to be interactable.
        /// Please note that this will only disable the buttons, the Inventory slots will still be shown
        /// </summary>
        /// <param name="toggle">True enables player to interact with button; False disables interaction completely</param>
        public void ToggleButtons(bool toggle)
        {
            for (int i = 0; i < _inventory.slots; ++i)
            {
                _slots[i].button.SetActive(toggle);
            }
        }
    }
}
