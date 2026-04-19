using GabrielBertasso.InventorySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GabrielBertasso.ShopSystem
{
    [CreateAssetMenu(fileName = "IAPProduct", menuName = "Gabriel Bertasso/IAPProduct")]
    public class ProductModel : ScriptableObject
    {
        public ItemModel Item;
        public ProductType Type;
        public string Category;
        public bool IsIAP;
        [ShowIf(nameof(IsIAP)), ReadOnly]
        public Product IAPProduct;
        [HideIf(nameof(IsIAP))]
        public ItemModel Currency;
        [HideIf(nameof(IsIAP))]
        public int PriceInCurrency;
        [ReadOnly] public int PurchasedQuantity;
        [ReadOnly] public ShopManager Shop;

        public string Id => Item != null ? Item.Id : string.Empty;
        public string Title => IsIAP ? IAPProduct?.metadata?.localizedTitle : (Item != null ? Item.Name : string.Empty);
        public string Description => IsIAP ? IAPProduct?.metadata?.localizedDescription : (Item != null ? Item.Description : string.Empty);
        public string Price => IsIAP ? IAPProduct?.metadata?.localizedPriceString : PriceInCurrency.ToString();
        public Sprite Icon => Item != null ? Item.Icon : null;

        public bool IsAvailable => !IsIAP || (IAPProduct != null && IAPProduct.availableToPurchase);
        public bool IsEntitled => IsPurchased && (Type == ProductType.NonConsumable || Type == ProductType.Subscription);
        public bool IsPurchased => PurchasedQuantity > 0;
    }
}