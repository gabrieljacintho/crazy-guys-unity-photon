using GabrielBertasso.InventorySystem;
using GabrielBertasso.Patterns;
using GabrielBertasso.ShopSystem.IAP;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GabrielBertasso.ShopSystem
{
    public class ShopManager : Singleton<ShopManager>
    {
        [SerializeField] private bool _isGlobal = true;
        [SerializeField] private bool _autoEquipItemOnPurchase = true;
        [SerializeField] private InventoryManager _inventory;
        [SerializeField] private List<ProductModel> _products;

        public List<ProductModel> Products => _products;
        public bool IsInitialized { get; private set; }

        public Action OnInitialized;
        public Action<ProductModel> OnProductPurchased;
        public Action OnPurchasesRestored;


        protected override void Awake()
        {
            if (_isGlobal)
            {
                base.Awake();
                DontDestroyOnLoad(gameObject);
                return;
            }

            if (_inventory == null)
            {
                _inventory = InventoryManager.Instance;
            }

            Initialize();
        }

        public void PurchaseProduct(string productId)
        {
            ProductModel product = TryGetProduct(productId);
            if (product == null)
            {
                return;
            }

            if (TryToggleEquipProduct(productId))
            {
                return;
            }

            if (product.IsIAP)
            {
                IAPManager.PurchaseProduct(productId);
            }
            else
            {
                int currencyQuantity = _inventory.GetItemQuantity(product.PurchaseCurrency);
                if (currencyQuantity < product.PriceInCurrency)
                {
                    Debug.LogWarning("Not enough coins!");
                    return;
                }

                _inventory.RemoveItem(product.PurchaseCurrency, product.PriceInCurrency);

                ProductPurchased(productId);
            }
        }

        private ProductModel TryGetProduct(string productId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[ShopManager] Shop has not yet been initialized!", this);
                return null;
            }

            ProductModel product = _products.Find(p => p.Id == productId);
            if (product == null)
            {
                Debug.LogError($"[ShopManager] Product with ID '{productId}' not found!", this);
                return null;
            }

            if (!product.IsAvailable)
            {
                Debug.LogWarning($"[ShopManager] Product '{product.name}' is not available for purchase!", this);
                return null;
            }

            return product;
        }

        public void ProductPurchased(string productId, int quantity = 1)
        {
            ProductModel product = _products.Find(p => p.Id == productId);
            if (product == null)
            {
                Debug.LogError($"[ShopManager] Product with ID '{productId}' not found!", this);
                return;
            }

            int quantityToAdd = product.Quantity * quantity;
            int quantityAdded = _inventory.AddItem(product.Item, quantityToAdd);
            if (quantityAdded != quantityToAdd)
            {
                Debug.LogError($"[ShopManager] Quantity to be added ({quantityToAdd}) does not match quantity added ({quantityAdded})!", this);
            }

            product.PurchasedQuantity += quantity;

            PlayerPrefs.SetInt(product.Id, product.PurchasedQuantity);
            PlayerPrefs.Save();

            OnProductPurchased?.Invoke(product);

            Debug.Log($"[ShopManager] Product with ID '{productId}' has been purchased.", this);

            if (_autoEquipItemOnPurchase)
            {
                TryEquipProduct(productId);
            }
        }

        private bool TryEquipProduct(string productId)
        {
            ProductModel product = _products.Find(p => p.Id == productId);
            if (product == null || !product.CanBeEquipped())
            {
                return false;
            }

            return _inventory.TryEquipItem(product.Item);
        }

        public bool TryToggleEquipProduct(string productId)
        {
            ProductModel product = TryGetProduct(productId);
            if (product == null || !product.CanBeEquipped())
            {
                return false;
            }

            return _inventory.TryToggleEquipItem(product.Item);
        }

        public void RestoreAllPurchases()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[ShopManager] Shop has not yet been initialized!", this);
                return;
            }

            foreach (ProductModel product in _products)
            {
                product.PurchasedQuantity = 0;
                PlayerPrefs.DeleteKey(product.Id);
            }

            PlayerPrefs.Save();

            IAPManager.RestoreAllPurchases();

            OnPurchasesRestored?.Invoke();

            Debug.Log("[ShopManager] All purchases restored.", this);
        }

        private void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning("ShopManager has already been initialized!", this);
                return;
            }

            InstantiateProducts();
            LoadProducts();

            IsInitialized = true;

            OnInitialized?.Invoke();

            Debug.Log("[ShopManager] Shop has been initialized.", this);
        }

        private void InstantiateProducts()
        {
            List<ProductModel> products = new List<ProductModel>();

            foreach (var product in _products)
            {
                products.Add(Instantiate(product));
            }

            _products = products;
        }

        private void LoadProducts()
        {
            foreach (ProductModel product in _products)
            {
                product.PurchasedQuantity = PlayerPrefs.GetInt(product.Id, 0);
                product.Shop = this;
                product.Inventory = _inventory;
            }
        }
    }
}