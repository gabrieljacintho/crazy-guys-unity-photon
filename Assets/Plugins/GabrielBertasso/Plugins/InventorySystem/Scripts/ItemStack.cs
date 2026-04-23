using System;

namespace GabrielBertasso.InventorySystem
{
    [Serializable]
    public class ItemStack
    {
        public ItemModel Item;
        public int Quantity;

        public bool IsEmpty => Item == null || Quantity <= 0;
        public bool IsFull => Item != null && Item.MaxStackSize >= 0 && Quantity >= Item.MaxStackSize;

        public ItemStack(ItemModel item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
