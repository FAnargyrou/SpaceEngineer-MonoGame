using SpaceEngineer.Hud;
using System.Collections.Generic;

namespace SpaceEngineer.GameObjects
{
    public class Inventory
    {
       
        public List<Item> items;
        public int slots = 2;

        // Inventory that the instance will exchange items with
        Inventory _deposit;

        public delegate void OnItemChanged();
        public OnItemChanged OnItemChangedCallback;

        public Inventory()
        {
            items = new List<Item>();

        }

        public void SetDepositInventory(Inventory deposit)
        {
            _deposit = deposit;
        }

        public bool AddItem(Item item)
        {
            if (items.Count >= slots) return false;

            items.Add(item);
            if (OnItemChangedCallback != null)
                OnItemChangedCallback.Invoke();

            return true;
        }

        public void RemoveItem(Item item)
        {
            if (!items.Contains(item)) return;

            if (_deposit.AddItem(item))
            {
                items.Remove(item);
                if (OnItemChangedCallback != null)
                    OnItemChangedCallback.Invoke();
            }
        }       
    }
}
