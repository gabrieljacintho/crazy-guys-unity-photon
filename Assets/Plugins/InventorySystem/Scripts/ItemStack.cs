using System;

namespace GabrielBertasso.InventorySystem
{
    [Serializable]
    public class ItemStack
    {
        public ItemModel Item;
        public int Quantity;

        public ItemStack(ItemModel item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
