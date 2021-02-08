using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using SpaceEngineer.Hud;

namespace SpaceEngineer.GameObjects.Ship
{
    public class Toolbox : ShipComponent
    {
        private InventoryHUD _playerInventory;
        private InventoryHUD _toolboxInventory;

        public Toolbox(Sprite sprite, Vector2 position, InventoryHUD playerInventory, InventoryHUD toolboxInventory) 
            : base (sprite, position)
        {
            _playerInventory = playerInventory;
            _toolboxInventory = toolboxInventory;
            _playerInventory.ToggleButtons(false);
            _toolboxInventory.SetActive(false);
        }

        public override void Cancel()
        {
            _toolboxInventory.SetActive(false);
            _playerInventory.ToggleButtons(false);
        }

        public override void Interact()
        {
            _playerInventory.ToggleButtons(true);
            _toolboxInventory.SetActive(true);
        }

        public override void Update(GameTime gameTime)
        {
            return;
        }
    }
}
