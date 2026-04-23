using System;

namespace GabrielBertasso.InventorySystem
{
    [Serializable]
    public class InventorySlot
    {
        public string Id = Guid.NewGuid().ToString();
        public ItemStack ItemStack;
        public bool IsEquipped;

        public ItemModel Item => ItemStack != null ? ItemStack.Item : null;
        public bool IsEmpty => ItemStack == null || ItemStack.IsEmpty;
        public bool IsFull => ItemStack != null && ItemStack.IsFull;

        public InventorySlot()
        {
            Id = Guid.NewGuid().ToString();
            ItemStack = null;
            IsEquipped = false;
        }

        public InventorySlot(ItemStack itemStack, bool isEquipped = false)
        {
            Id = Guid.NewGuid().ToString();
            ItemStack = itemStack;
            IsEquipped = isEquipped;
        }
    }
}