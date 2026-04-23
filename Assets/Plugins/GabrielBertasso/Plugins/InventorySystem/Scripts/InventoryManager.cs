using GabrielBertasso.Patterns;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GabrielBertasso.InventorySystem
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private bool _isGlobal = true;
        [Tooltip("Set negative to infinity.")]
        [SerializeField] private int _maxSlotCount = -1;
        [SerializeField] private List<ItemStack> _defaultItemStacks = new List<ItemStack>();

        [ShowInInspector, ReadOnly] private List<InventorySlot> _slots = new List<InventorySlot>();

        private Dictionary<string, string> _lastEquippedSlotIdByCategory = new Dictionary<string, string>();

        public bool IsInitialized { get; private set; }

        public Action OnInitialized;
        public Action<ItemModel> OnItemQuantityChanged;
        public Action OnItemsRestored;


        protected override void Awake()
        {
            if (_isGlobal)
            {
                base.Awake();
                DontDestroyOnLoad(gameObject);
            }

            Initialize();
        }

        public int AddItem(ItemModel item, int quantity = 1)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[InventoryManager] Inventory has not yet been initialized!", this);
                return 0;
            }

            if (quantity <= 0)
            {
                Debug.LogWarning("[InventoryManager] Quantity to add must be greater than zero!", this);
                return 0;
            }

            int remainingQuantity = quantity;
            List<InventorySlot> slots = _slots.FindAll(s => s.ItemStack.Item == item
                && (s.ItemStack.Item.MaxStackSize < 0 || s.ItemStack.Quantity < s.ItemStack.Item.MaxStackSize));
            slots = slots.OrderByDescending(x => x.ItemStack.Quantity).ToList();

            do
            {
                InventorySlot slot;
                ItemStack itemStack;

                if (slots.Count > 0)
                {
                    slot = slots[0];
                    itemStack = slot.ItemStack;
                    slots.Remove(slot);
                }
                else if (_maxSlotCount < 0 || _slots.Count < _maxSlotCount)
                {
                    itemStack = new ItemStack(item, 0);
                    _slots.Add(new InventorySlot(itemStack));
                }
                else
                {
                    Debug.LogWarning("[InventoryManager] Maximum number of slots reached!", this);
                    break;
                }

                int quantityToAdd = itemStack.Item.MaxStackSize >= 0 ? Mathf.Min(remainingQuantity, item.MaxStackSize - itemStack.Quantity) : remainingQuantity;
                itemStack.Quantity += quantityToAdd;
                remainingQuantity -= quantityToAdd;
            }
            while (remainingQuantity > 0);

            OnItemQuantityChanged?.Invoke(item);

            Debug.Log($"[InventoryManager] Item '{item.name}' added to inventory. ({quantity - remainingQuantity})", this);

            return quantity - remainingQuantity;
        }

        public int RemoveItem(ItemModel item, int quantity = 1)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[InventoryManager] Inventory has not yet been initialized!", this);
                return 0;
            }

            if (quantity <= 0)
            {
                Debug.LogWarning("[InventoryManager] Quantity to remove must be greater than zero!", this);
                return 0;
            }

            int remainingQuantity = quantity;
            List<InventorySlot> slots = _slots.FindAll(s => s.ItemStack.Item == item);
            slots = slots.OrderBy(x => x.ItemStack.Quantity).ToList();

            do
            {
                if (slots.Count == 0)
                {
                    Debug.LogWarning($"[InventoryManager] Not enough quantity of item \"{item.name}\" to remove!", this);
                    return quantity - remainingQuantity;
                }

                InventorySlot slot = slots[0];
                ItemStack itemStack = slot.ItemStack;

                int quantityToRemove = Mathf.Min(remainingQuantity, itemStack.Quantity);
                itemStack.Quantity -= quantityToRemove;
                remainingQuantity -= quantityToRemove;

                if (itemStack.IsEmpty)
                {
                    _slots.Remove(slot);
                    slots.Remove(slot);
                }
            }
            while (remainingQuantity > 0);

            OnItemQuantityChanged?.Invoke(item);

            Debug.Log($"[InventoryManager] Item '{item.name}' removed from inventory. ({quantity - remainingQuantity})", this);

            return quantity - remainingQuantity;
        }

        public void RestoreAllItems()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[InventoryManager] Inventory has not yet been initialized!", this);
                return;
            }

            Dictionary<ItemModel, int> quantityByItemModel = new Dictionary<ItemModel, int>();
            foreach (var item in _defaultItemStacks)
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

            _slots.Clear();
            foreach (var kvp in quantityByItemModel)
            {
                AddItem(kvp.Key, kvp.Value);
            }

            OnItemsRestored?.Invoke();

            Debug.Log("[InventoryManager] All items restored.", this);
        }

        public int GetItemQuantity(ItemModel item)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[InventoryManager] Inventory has not yet been initialized!", this);
                return 0;
            }

            int quantity = 0;

            foreach (var itemStack in _slots)
            {
                if (itemStack.ItemStack.Item == item)
                {
                    quantity += itemStack.ItemStack.Quantity;
                }
            }

            return quantity;
        }

        public int GetAvailableSpace(ItemModel item)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[InventoryManager] Inventory has not yet been initialized!", this);
                return 0;
            }

            if (_maxSlotCount < 0)
            {
                return -1;
            }

            List<InventorySlot> notFullSlots = _slots.FindAll(x => x.Item == item && !x.IsFull);

            if (item.MaxStackSize < 0 && (_slots.Count < _maxSlotCount || notFullSlots.Count > 0))
            {
                return -1;
            }

            int slotEmptySpace = notFullSlots.Sum(x => item.MaxStackSize - x.ItemStack.Quantity);
            int emptySlotsCount = _maxSlotCount - _slots.Count;

            return slotEmptySpace + (emptySlotsCount * item.MaxStackSize);
        }

        public bool TryEquipItem(ItemModel item)
        {
            InventorySlot slot = _slots.Find(s => s.ItemStack.Item == item);
            if (slot == null)
            {
                Debug.LogWarning($"[InventoryManager] No \"{item.name}\" item in the inventory!", this);
                return false;
            }

            if (!slot.ItemStack.Item.IsEquipable)
            {
                Debug.LogWarning($"Item \"{item.name}\" cannot be equipped!", this);
                return false;
            }

            if (item.MaxEquippedItemsInCategory >= 0)
            {
                int equippedCount = _slots.FindAll(x => x.IsEquipped && x.ItemStack.Item == item).Count;
                if (equippedCount > item.MaxEquippedItemsInCategory)
                {
                    TryUnequipLastEquippedItemInCategory(item);
                }
            }

            slot.IsEquipped = true;

            Debug.Log($"[InventoryManager] Slot with item '{item.name}' was equipped.", this);

            return true;
        }

        private bool TryUnequipLastEquippedItemInCategory(ItemModel item)
        {
            InventorySlot slot;
            if (_lastEquippedSlotIdByCategory.TryGetValue(item.EquipmentCategory, out string slotId) && _slots.Exists(x => x.Id == slotId))
            {
                slot = _slots.Find(x => x.Id == slotId);
            }
            else
            {
                slot = _slots.FindLast(x => x.IsEquipped && x.ItemStack.Item.Id == item.Id);
            }

            if (slot != null)
            {
                UnequipItem(slot);
                return true;
            }

            return false;
        }

        public bool TryUnequipItem(ItemModel item)
        {
            InventorySlot slot = _slots.Find(s => s.ItemStack.Item == item);
            if (slot == null)
            {
                Debug.LogWarning($"[InventoryManager] No '{item.name}' item in the inventory!", this);
                return false;
            }

            UnequipItem(slot);

            return true;
        }

        private void UnequipItem(InventorySlot slot)
        {
            slot.IsEquipped = false;

            Debug.Log($"[InventoryManager] Slot with item '{slot.ItemStack.Item.name}' was unequipped.", this);
        }

        public bool TryToggleEquipItem(ItemModel item)
        {
            InventorySlot slot = _slots.Find(s => s.ItemStack.Item == item);
            if (slot == null)
            {
                Debug.LogWarning($"[InventoryManager] No '{item.name}' item in the inventory!", this);
                return false;
            }

            if (slot.IsEquipped)
            {
                UnequipItem(slot);
                return true;
            }
            else
            {
                return TryEquipItem(item);
            }
        }

        public bool IsEquipped(ItemModel item)
        {
            return _slots.Exists(x => x.IsEquipped && x.ItemStack.Item == item);
        }

        private void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[InventoryManager] Inventory has already been initialized!", this);
                return;
            }

            IsInitialized = true;

            RestoreAllItems();

            OnInitialized?.Invoke();

            Debug.Log("[InventoryManager] Inventory has been initialized.", this);
        }
    }
}
