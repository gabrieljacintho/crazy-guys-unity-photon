using GabrielBertasso.InventorySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GabrielBertasso.ShopSystem
{
    [CreateAssetMenu(fileName = "Product", menuName = "Gabriel Bertasso/Product")]
    public class ProductModel : ScriptableObject
    {
        [SerializeField] private ItemModel _item;
        [Min(1)]
        [SerializeField] private int _quantity = 1;
        [SerializeField] private ProductType _type;
        [SerializeField] private string _category;
        [SerializeField] private bool _isIAP;
        [ShowIf(nameof(IsIAP)), ReadOnly]
        [SerializeField] private Product _iapProduct;
        [HideIf(nameof(IsIAP))]
        [SerializeField] private ItemModel _purchaseCurrency;
        [HideIf(nameof(IsIAP))]
        [SerializeField] private int priceInCurrency = 10;
        [ShowInInspector, ReadOnly] private int _purchasedQuantity;
        [ShowInInspector, ReadOnly] private ShopManager _shop;
        [ShowInInspector, ReadOnly] private InventoryManager _inventory;

        public ItemModel Item => _item;
        public int Quantity => _quantity;
        public ProductType Type => _type;
        public string Category => _category;
        public bool IsIAP => _isIAP;
        public Product IAPProduct { get => _iapProduct; set => _iapProduct = value; }
        public ItemModel PurchaseCurrency => _purchaseCurrency;
        public int PriceInCurrency => priceInCurrency;
        public int PurchasedQuantity { get => _purchasedQuantity; set => _purchasedQuantity = value; }
        public ShopManager Shop { get => _shop; set => _shop = value; }
        public InventoryManager Inventory { get => _inventory; set => _inventory = value; }

        public string Id => Item != null ? Item.Id : string.Empty;
        public string Title => IsIAP ? IAPProduct?.metadata?.localizedTitle : (Item != null ? Item.Name : string.Empty);
        public string Description => IsIAP ? IAPProduct?.metadata?.localizedDescription : (Item != null ? Item.Description : string.Empty);
        public string Price => IsIAP ? IAPProduct?.metadata?.localizedPriceString : PriceInCurrency.ToString();
        public Sprite Icon => Item != null ? Item.Icon : null;

        public bool IsAvailable => !IsIAP || (IAPProduct != null && IAPProduct.availableToPurchase);
        public bool IsPurchased => PurchasedQuantity > 0;
        public bool IsEntitled => IsPurchased && (Type == ProductType.NonConsumable || Type == ProductType.Subscription);
        public bool IsEquippable => IsAvailable && IsPurchased && Inventory != null && Item != null && Item.IsEquippable;

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
            return IsEquippable && Inventory.CanBeEquipped(Item);
        }

        public bool CanBeUnequipped()
        {
            return IsEquippable && Inventory.CanBeUnequipped(Item);
        }

        public static implicit operator string(ProductModel model)
        {
            return model.Id;
        }
    }
}