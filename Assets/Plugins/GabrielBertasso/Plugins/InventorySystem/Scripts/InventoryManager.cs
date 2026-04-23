using GabrielBertasso.Patterns;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GabrielBertasso.InventorySystem
{
    public class InventoryManager : PersistentSingleton<InventoryManager>
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
            }

            Initialize();
        }

        #region Item

        public int AddItem(ItemModel item, int quantity = 1, bool force = false)
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
            List<InventorySlot> remainingSlots = _slots.FindAll(s => (s.IsEmpty || s.ItemStack.Item == item)
                && (s.ItemStack.Item.MaxStackSize < 0 || s.ItemStack.Quantity < s.ItemStack.Item.MaxStackSize));
            remainingSlots = remainingSlots.OrderByDescending(x => x.ItemStack.Quantity).ToList();

            do
            {
                InventorySlot slot;
                ItemStack itemStack;

                if (remainingSlots.Count > 0)
                {
                    slot = remainingSlots[0];
                    slot.ItemStack ??= new ItemStack(item);
                    itemStack = slot.ItemStack;
                    remainingSlots.Remove(slot);
                }
                else if (_maxSlotCount < 0 || _slots.Count < _maxSlotCount)
                {
                    itemStack = new ItemStack(item);
                    slot = CreateSlot(itemStack);
                }
                else if (force)
                {
                    itemStack = new ItemStack(item);
                    slot = CreateSlot(itemStack);
                    Debug.LogWarning($"[InventoryManager] Forcing the creation of a new slot for the item '{item.name}'!", this);
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

            if (item.IsEquippable)
            {
                TryEquipSlotsMinQuantity(item);
            }

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
            List<InventorySlot> remainingSlots = _slots.FindAll(s => !s.IsEmpty && s.ItemStack.Item == item);
            remainingSlots = remainingSlots.OrderBy(x => x.ItemStack.Quantity).ToList();

            do
            {
                if (remainingSlots.Count == 0)
                {
                    Debug.LogWarning($"[InventoryManager] Not enough quantity of item \"{item.name}\" to remove!", this);
                    return quantity - remainingQuantity;
                }

                InventorySlot slot = remainingSlots[0];
                ItemStack itemStack = slot.ItemStack;

                int quantityToRemove = Mathf.Min(remainingQuantity, itemStack.Quantity);
                itemStack.Quantity -= quantityToRemove;
                remainingQuantity -= quantityToRemove;

                if (itemStack.IsEmpty)
                {
                    RemoveSlot(slot);
                    remainingSlots.Remove(slot);
                }
            }
            while (remainingQuantity > 0);

            OnItemQuantityChanged?.Invoke(item);

            Debug.Log($"[InventoryManager] Item '{item.name}' removed from inventory. ({quantity - remainingQuantity})", this);

            if (item.IsEquippable)
            {
                TryEquipSlotsMinQuantity(item);
            }

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

            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty && slot.ItemStack.Item == item)
                {
                    quantity += slot.ItemStack.Quantity;
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

        #endregion

        #region Equip

        public bool TryEquipItem(ItemModel item)
        {
            InventorySlot slot = _slots.Find(s => s.Item == item);
            if (slot == null)
            {
                Debug.LogWarning($"[InventoryManager] Unable to equip the '{item.name}' item: No item in inventory!", this);
                return false;
            }

            if (!slot.ItemStack.Item.IsEquippable)
            {
                Debug.LogWarning($"[InventoryManager] Unable to equip the '{item.name}' item: Item is not equippable!", this);
                return false;
            }

            if (!CanBeEquipped(item))
            {
                TryUnequipLastEquippedItemInCategory(item);
                if (!CanBeEquipped(item))
                {
                    Debug.LogWarning($"[InventoryManager] Unable to equip the '{item.name}' item: Maximum number of equipped slots in the equipment category reached!", this);
                    return false;
                }
            }

            EquipSlot(slot);

            return true;
        }

        private bool TryEquipSlotsMinQuantity(ItemModel item)
        {
            int remainingQuantity = item.MinEquippedSlotsInCategory - GetEquippedSlotsCount(item.EquipmentCategory);
            if (remainingQuantity <= 0)
            {
                return true;
            }

            List<InventorySlot> remainingSlots = _slots.FindAll(x => !x.IsEmpty && !x.IsEquipped && x.Item.EquipmentCategory == item.EquipmentCategory);
            for (int i = 0; i < remainingQuantity; i++)
            {
                if (remainingSlots.Count == 0)
                {
                    Debug.LogWarning($"[InventoryManager] Item '{item.name}' has fewer equipped slots ({GetEquippedSlotsCount(item.EquipmentCategory)}) than the minimum number of equipped slots in the category ({item.MinEquippedSlotsInCategory})!", this);
                    return false;
                }

                InventorySlot slot = remainingSlots[0];
                remainingSlots.Remove(slot);

                EquipSlot(slot);
            }

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
                slot = _slots.FindLast(x => !x.IsEmpty && x.IsEquipped && x.Item.EquipmentCategory == item.EquipmentCategory);
            }

            if (slot != null)
            {
                UnequipSlot(slot);
                return true;
            }

            return false;
        }

        public bool TryUnequipItem(ItemModel item)
        {
            InventorySlot slot = _slots.Find(s => s.Item == item);
            if (slot == null)
            {
                Debug.LogWarning($"[InventoryManager] Unable to unequip the '{item.name}' item: No item in inventory!", this);
                return false;
            }

            if (GetEquippedSlotsCount(item.EquipmentCategory) <= item.MinEquippedSlotsInCategory)
            {
                Debug.LogWarning($"[InventoryManager] Unable to unequip the '{item.name}' item: Minimum number of equipped slots in the equipment category reached!");
                return false;
            }

            UnequipSlot(slot);

            return true;
        }

        public bool TryToggleEquipItem(ItemModel item)
        {
            InventorySlot slot = _slots.Find(s => s.ItemStack.Item == item);
            if (slot == null)
            {
                Debug.LogWarning($"[InventoryManager] No '{item.name}' item in inventory!", this);
                return false;
            }

            if (slot.IsEquipped)
            {
                UnequipSlot(slot);
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

        public bool CanBeEquipped(ItemModel item)
        {
            return item.IsEquippable && (item.MaxEquippedSlotsInCategory < 0 || GetEquippedSlotsCount(item.EquipmentCategory) < item.MaxEquippedSlotsInCategory);
        }

        public bool CanBeUnequipped(ItemModel item)
        {
            return IsEquipped(item) && GetEquippedSlotsCount(item.EquipmentCategory) > item.MinEquippedSlotsInCategory;
        }

        public int GetEquippedSlotsCount(string equipmentCategory)
        {
            return _slots.FindAll(x => !x.IsEmpty && x.IsEquipped && x.Item.EquipmentCategory == equipmentCategory).Count;
        }

        #endregion

        #region Slot

        private void EquipSlot(InventorySlot slot)
        {
            slot.IsEquipped = true;

            Debug.Log($"[InventoryManager] Slot with item '{slot.Item.name}' was equipped.", this);
        }

        private void UnequipSlot(InventorySlot slot)
        {
            slot.IsEquipped = false;

            Debug.Log($"[InventoryManager] Slot with item '{slot.Item.name}' was unequipped.", this);
        }

        private InventorySlot CreateSlot(ItemStack itemStack = null)
        {
            InventorySlot slot = new InventorySlot(itemStack);
            _slots.Add(slot);

            return slot;
        }

        private bool RemoveSlot(InventorySlot slot)
        {
            return _slots.Remove(slot);
        }

        #endregion

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
