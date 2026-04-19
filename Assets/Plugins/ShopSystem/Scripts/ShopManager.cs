using GabrielBertasso.InventorySystem;
using GabrielBertasso.ShopSystem.IAP;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.ShopSystem
{
    public class ShopManager : Singleton<ShopManager>
    {
        [SerializeField] private bool _isGlobal = true;
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
            if (!IsInitialized)
            {
                Debug.LogWarning("[ShopManager] Shop is not initialized yet!", this);
                return;
            }

            ProductModel product = _products.Find(p => p.Id == productId);
            if (product == null)
            {
                Debug.LogError($"[ShopManager] Product with ID '{productId}' not found!", this);
                return;
            }

            if (!product.IsAvailable)
            {
                Debug.LogWarning($"[ShopManager] Product '{product.name}' is not available for purchase!", this);
                return;
            }

            if (product.IsIAP)
            {
                IAPManager.Instance.PurchaseProduct(productId);
                return;
            }

            ProductPurchased(productId);
        }

        public void ProductPurchased(string productId, int quantity = 1)
        {
            ProductModel product = _products.Find(p => p.Id == productId);
            if (product == null)
            {
                Debug.LogError($"[ShopManager] Product with ID '{productId}' not found!", this);
                return;
            }

            _inventory.AddItem(productId, quantity);

            product.PurchasedQuantity += quantity;

            PlayerPrefs.SetInt(product.Id, product.PurchasedQuantity);
            PlayerPrefs.Save();

            OnProductPurchased?.Invoke(product);
        }

        public void RestoreAllPurchases()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[ShopManager] Shop is not initialized yet!", this);
                return;
            }

            foreach (ProductModel product in _products)
            {
                product.PurchasedQuantity = 0;
                PlayerPrefs.DeleteKey(product.Id);
            }

            PlayerPrefs.Save();

            IAPManager.Instance.RestoreAllPurchases();

            OnPurchasesRestored?.Invoke();
        }

        private void Initialize()
        {
            InstantiateProducts();
            LoadProducts();

            IsInitialized = true;

            OnInitialized?.Invoke();
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
            }
        }
    }
}