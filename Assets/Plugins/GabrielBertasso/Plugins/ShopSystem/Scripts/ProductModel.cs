using GabrielBertasso.InventorySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GabrielBertasso.ShopSystem
{
    [CreateAssetMenu(fileName = "Product", menuName = "Gabriel Bertasso/Product")]
    public class ProductModel : ScriptableObject
    {
        public ItemModel Item;
        [Min(1)]
        public int Quantity = 1;
        public ProductType Type;
        public string Category;
        public bool IsIAP;
        [ShowIf(nameof(IsIAP)), ReadOnly]
        public Product IAPProduct;
        [HideIf(nameof(IsIAP))]
        public ItemModel PurchaseCurrency;
        [HideIf(nameof(IsIAP))]
        public int PriceInCurrency;
        [ReadOnly] public int PurchasedQuantity;
        [ReadOnly] public ShopManager Shop;
        [ReadOnly] public InventoryManager Inventory;

        public string Id => Item != null ? Item.Id : string.Empty;
        public string Title => IsIAP ? IAPProduct?.metadata?.localizedTitle : (Item != null ? Item.Name : string.Empty);
        public string Description => IsIAP ? IAPProduct?.metadata?.localizedDescription : (Item != null ? Item.Description : string.Empty);
        public string Price => IsIAP ? IAPProduct?.metadata?.localizedPriceString : PriceInCurrency.ToString();
        public Sprite Icon => Item != null ? Item.Icon : null;

        public bool IsAvailable => !IsIAP || (IAPProduct != null && IAPProduct.availableToPurchase);
        public bool IsPurchased => PurchasedQuantity > 0;
        public bool IsEntitled => IsPurchased && (Type == ProductType.NonConsumable || Type == ProductType.Subscription);


        public bool CanBePurchased()
        {
            return IsAvailable && !IsEntitled && HasEnoughAvailableSpaceInInventory();
        }

        public bool HasEnoughAvailableSpaceInInventory()
        {
            if (Inventory == null || Item == null)
            {
                return false;
            }

            int availableSpace = Inventory.GetAvailableSpace(Item);

            return availableSpace < 0 || availableSpace >= Quantity;
        }

        public bool CanBeEquipped()
        {
            return IsAvailable && IsPurchased && Item != null && Item.IsEquipable;
        }

        public static implicit operator string(ProductModel model)
        {
            return model.Id;
        }
    }
}