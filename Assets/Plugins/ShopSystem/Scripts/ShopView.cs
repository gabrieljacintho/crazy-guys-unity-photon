using GabrielBertasso.ShopSystem.IAP;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace GabrielBertasso.ShopSystem
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private ShopManager _localShop;
        [SerializeField] private ProductView _productViewPrefab;
        [SerializeField] private Transform _productViewParent;
        [SerializeField] private string _requiredCategory;

        private List<ProductModel> _localProducts;
        private List<ProductView> _productViews;

        private bool _isShopInitialized;
        private bool _isIAPInitialized;

        [Space]
        public UnityEvent OnInitialized;
        public UnityEvent<string> OnIAPPurchaseFailed;
        public UnityEvent OnIAPPurchaseDeferred;


        private void Awake()
        {
            if (_localShop == null)
            {
                _localShop = ShopManager.Instance;
            }
        }


        private void OnEnable()
        {
            if (_localShop.IsInitialized)
            {
                ShopManager_OnInitialized();
            }
            else
            {
                _localShop.OnInitialized += ShopManager_OnInitialized;
            }

            _localShop.OnPurchasesRestored += UpdateAllProductViews;

            IAPManager.OnPurchaseFailed += IAPManager_OnPurchaseFailed;
            IAPManager.OnPurchaseDeferred += IAPManager_OnPurchaseDeferred;

            if (IAPManager.IsInitialized)
            {
                IAPManager_OnInitialized();
            }
            else
            {
                IAPManager.OnInitialized += IAPManager_OnInitialized;
            }
        }

        private void OnDisable()
        {
            if (_localShop != null)
            {
                _localShop.OnInitialized -= ShopManager_OnInitialized;
                _localShop.OnPurchasesRestored -= UpdateAllProductViews;
            }

            IAPManager.OnInitialized -= IAPManager_OnInitialized;
            IAPManager.OnPurchaseFailed -= IAPManager_OnPurchaseFailed;
            IAPManager.OnPurchaseDeferred -= IAPManager_OnPurchaseDeferred;
        }

        private void ShopManager_OnInitialized()
        {
            if (_isShopInitialized)
            {
                return;
            }

            _isShopInitialized = true;

            _localProducts = GetProducts(_localShop);
            
            foreach (ProductModel product in _localProducts)
            {
                if (!product.IsIAP)
                {
                    CreateProductView(product);
                }
            }

            if (_isShopInitialized && _isIAPInitialized)
            {
                OnInitialized?.Invoke();
            }
        }

        private void IAPManager_OnInitialized()
        {
            if (_isIAPInitialized)
            {
                return;
            }

            _isIAPInitialized = true;

            // IAPs are always global
            List<ProductModel> products = GetProducts(ShopManager.Instance);

            foreach (ProductModel product in products)
            {
                if (product.IsIAP)
                {
                    CreateProductView(product);
                }
            }

            if (_isShopInitialized && _isIAPInitialized)
            {
                OnInitialized?.Invoke();
            }
        }

        private void IAPManager_OnPurchaseFailed(FailedOrder order)
        {
            OnIAPPurchaseFailed?.Invoke(order?.Details);
        }

        private void IAPManager_OnPurchaseDeferred(DeferredOrder order)
        {
            OnIAPPurchaseDeferred?.Invoke();
        }

        private void CreateProductView(ProductModel product)
        {
            ProductView instance = Instantiate(_productViewPrefab, _productViewParent);
            instance.Initialize(product);
            instance.transform.SetSiblingIndex(_localProducts.IndexOf(product));

            _productViews.Add(instance);
        }

        private void UpdateAllProductViews()
        {
            foreach (ProductView view in _productViews)
            {
                view.UpdateView();
            }
        }

        private List<ProductModel> GetProducts(ShopManager shop)
        {
            if (string.IsNullOrEmpty(_requiredCategory))
            {
                return shop.Products;
            }
            else
            {
                return shop.Products.FindAll(p => p.Category == _requiredCategory);
            }
        }
    }
}