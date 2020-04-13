using System;
using System.Collections.Generic;

namespace Game
{
    public class InventoryItemModel
    {
        public string Classname { get; private set; }
        public int Amount { get; private set; }
        public Asset Asset { get; private set; }

        public event Action<InventoryItemModel, int, int> OnIncrease;
        public event Action<InventoryItemModel, int, int> OnDecrease;
        
        internal InventoryItemModel(string classname, int amount)
        {
            Classname = classname;
            Amount = amount;

            Init();
        }

        private void Init()
        {
            Asset = G.self.asset_resource_manager.GetResource(Classname);
        }

        internal void Increase(int amount)
        {
            Amount += amount;
            OnIncrease?.Invoke(this, Amount, amount);
        }

        internal void Decrease(int amount)
        {
            Amount -= amount;
            OnDecrease?.Invoke(this, Amount, amount);
        }
    }
    
    public class UserInventory
    {
        public event Action<InventoryItemModel, int, int, bool> OnIncrease;
        public event Action<InventoryItemModel, int, int, bool> OnDecrease;

        private Dictionary<string, InventoryItemModel> items;
        
        public UserInventory()
        {
            items = new Dictionary<string, InventoryItemModel>();
        }

        public int GetItemAmount(string classname)
        {
            return items.ContainsKey(classname) ? items[classname].Amount : 0;
        }

        public void Increase(string classname, int amount = 1)
        {
            if (amount == 0)
                return;
            
            if (!items.TryGetValue(classname, out InventoryItemModel item))
            {
                item = new InventoryItemModel(classname, amount);
                items.Add(classname, item);
            }
            else
                item.Increase(amount);

            OnIncrease?.Invoke(item, items[classname].Amount, amount, items[classname].Amount - amount == 0);
        }

        public void Decrease(string classname, int amount = 1)
        {
            if (amount == 0)
                return;

            if (!items.TryGetValue(classname, out InventoryItemModel item))
                UnityEngine.Debug.LogError(string.Format("Ассета нет в инвентаре classname : {0}", classname));
            
            if(item.Amount < amount)
                UnityEngine.Debug.LogError(string.Format("Недостаточно количества для уменьшения classname : {0}, amount : {1}, current_amount : {2}", classname, amount, items[classname]));

            item.Decrease(amount);
            
            if(item.Amount == 0)
                items.Remove(classname);

            OnDecrease?.Invoke(item, item.Amount, amount, item.Amount == 0);   
        }

        public List<InventoryItemModel> GetItems()
        {
            List<InventoryItemModel> inventoryItems = new List<InventoryItemModel>();

            foreach (KeyValuePair<string, InventoryItemModel> pair in items)
                inventoryItems.Add(pair.Value);

            return inventoryItems;
        }
    }
}