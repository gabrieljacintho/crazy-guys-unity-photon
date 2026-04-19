using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.InventorySystem
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private bool _isGlobal = true;
        [SerializeField] private List<ItemModel> _itemModels = new List<ItemModel>();
        [SerializeField] private List<ItemStack> _itemStacks = new List<ItemStack>();

        private bool _isInitialized = false;


        protected override void Awake()
        {
            if (_isGlobal)
            {
                base.Awake();
                DontDestroyOnLoad(gameObject);
            }

            Initialize();
        }

        private void Initialize()
        {
            Dictionary<ItemModel, int> quantityByItemModel = new Dictionary<ItemModel, int>();
            foreach (var item in _itemStacks)
            {
                if (quantityByItemModel.ContainsKey(item.Item))
                {
                    quantityByItemModel[item.Item] += item.Quantity;
                }
                else
                {
                    quantityByItemModel.Add(item.Item, item.Quantity);
                }
            }

            _itemStacks = new List<ItemStack>();
            foreach (var kvp in quantityByItemModel)
            {
                AddItem(kvp.Key.Id, kvp.Value);
            }

            _isInitialized = true;
        }

        public void AddItem(string itemId, int quantity = 1)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("InventoryManager is not initialized!");
                return;
            }

            if (quantity <= 0)
            {
                Debug.LogWarning("Quantity to add must be greater than zero!");
                return;
            }

            ItemModel item = _itemModels.Find(i => i.Id == itemId);
            if (item == null)
            {
                Debug.LogWarning($"Item with ID {itemId} not found in inventory.");
                return;
            }

            do
            {
                ItemStack itemStack = _itemStacks.Find(s => s.Item.Id == itemId && s.Quantity < s.Item.MaxStackSize);
                if (itemStack == null)
                {
                    itemStack = new ItemStack(item, 0);
                    _itemStacks.Add(itemStack);
                }

                int addQuantity = Mathf.Min(quantity, item.MaxStackSize - itemStack.Quantity);
                itemStack.Quantity += addQuantity;
                quantity -= addQuantity;
            }
            while (quantity > 0);
        }

        public void RemoveItem(string itemId, int quantity = 1)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("InventoryManager is not initialized!");
                return;
            }

            if (quantity <= 0)
            {
                Debug.LogWarning("Quantity to add must be greater than zero!");
                return;
            }

            ItemModel item = _itemModels.Find(i => i.Id == itemId);
            if (item == null)
            {
                Debug.LogWarning($"Item with ID {itemId} not found in inventory.");
                return;
            }

            do
            {
                ItemStack itemStack = _itemStacks.Find(s => s.Item.Id == itemId);
                if (itemStack == null)
                {
                    Debug.LogWarning($"Not enough quantity of item with ID \"{itemId}\" to remove!");
                    return;
                }

                int removeQuantity = Mathf.Min(quantity, itemStack.Quantity);
                itemStack.Quantity -= removeQuantity;
                quantity -= removeQuantity;

                if (itemStack.Quantity <= 0)
                {
                    _itemStacks.Remove(itemStack);
                }
            }
            while (quantity > 0);
        }

        public int GetItemQuantity(string itemId)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("InventoryManager is not initialized!");
                return 0;
            }

            int quantity = 0;

            foreach (var itemStack in _itemStacks)
            {
                if (itemStack.Item.Id == itemId)
                {
                    quantity += itemStack.Quantity;
                }
            }

            return quantity;
        }
    }
}
