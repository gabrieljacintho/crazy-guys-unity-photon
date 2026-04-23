using GabrielBertasso.Patterns;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GabrielBertasso.ShopSystem.IAP
{
    public class IAPManager : PersistentSingleton<IAPManager>
    {
        public static bool IsInitialized { get; private set; }

        private static StoreController s_storeController;

        private static List<ProductModel> _products;

        public static Action OnInitialized;
        public static Action<FailedOrder> OnPurchaseFailed;
        public static Action<DeferredOrder> OnPurchaseDeferred;


        protected override async void Awake()
        {
            base.Awake();

            if (ShopManager.Instance.IsInitialized)
            {
                await Initialize();
            }
            else
            {
                ShopManager.Instance.OnInitialized += async () => await Initialize();
            }
        }

        public static void PurchaseProduct(string productId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[IAPManager] Store is not initialized yet!");
                return;
            }

            s_storeController.PurchaseProduct(productId);

            Debug.Log($"[IAPManager] Purchase product: {productId}");
        }

        // Only for Apple platforms. Google Play restores purchases automatically on new installs and app reinstalls.
        public static void RestoreAllPurchases()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[IAPManager] Store is not initialized yet!");
                return;
            }

            s_storeController.RestoreTransactions((success, error) =>
            {
                if (success)
                {
                    Debug.Log("[IAPManager] Restore purchases successful.");
                }
                else
                {
                    Debug.LogWarning($"[IAPManager] Restore purchases failed! Reason: {error}");
                }
            });
        }

        private static async Task Initialize()
        {
            try
            {
                var option = new InitializationOptions().SetEnvironmentName("production");
                await UnityServices.InitializeAsync(option);

                s_storeController = UnityIAPServices.StoreController();

                s_storeController.OnStoreConnected += StoreController_OnStoreConnected;
                s_storeController.OnStoreDisconnected += StoreController_OnStoreDisconnected;

                s_storeController.OnProductsFetched += StoreController_OnProductsFetched;
                s_storeController.OnProductsFetchFailed += StoreController_OnProductsFetchFailed;

                s_storeController.OnPurchasesFetched += StoreController_OnPurchasesFetched;
                s_storeController.OnPurchasesFetchFailed += StoreController_OnPurchasesFetchFailed;

                s_storeController.OnPurchasePending += StoreController_OnPurchasePending;
                s_storeController.OnPurchaseConfirmed += StoreController_OnPurchaseConfirmed;
                s_storeController.OnPurchaseFailed += StoreController_OnPurchaseFailed;
                s_storeController.OnPurchaseDeferred += StoreController_OnPurchaseDeferred;

                s_storeController.OnCheckEntitlement += StoreController_OnCheckEntitlement;

                _products = ShopManager.Instance.Products.FindAll(x => x.IsIAP);

                await s_storeController.Connect();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private static void StoreController_OnStoreConnected()
        {
            Debug.Log("[IAPManager] Store connected successfully.");

            s_storeController.FetchProducts(BuildProductDefinitions());
        }

        private static List<ProductDefinition> BuildProductDefinitions()
        {
            List<ProductDefinition> productDefinitions = new List<ProductDefinition>();
            foreach (ProductModel product in _products)
            {
                productDefinitions.Add(new ProductDefinition(product.Id, product.Type));
            }

            return productDefinitions;
        }

        private static void StoreController_OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            Debug.LogWarning($"[IAPManager] Store disconnected! Reason: {description?.Message}");

            if (description.isRetryable)
            {
                s_storeController.Connect();
            }
        }

        private static void StoreController_OnProductsFetched(List<Product> products)
        {
            Debug.Log($"[IAPManager] Products fetched successfully. Total products: {products?.Count}");

            foreach (var product in products)
            {
                ProductModel model = _products.Find(p => p.Id == product.definition.id);
                model.IAPProduct = product;
            }

            s_storeController.FetchPurchases();
        }

        private static void StoreController_OnProductsFetchFailed(ProductFetchFailed fetchFailed)
        {
            Debug.LogWarning($"[IAPManager] Failed to fetch products! Reason: {fetchFailed?.FailureReason}");
        }

        private static void StoreController_OnPurchasesFetched(Orders orders)
        {
            Debug.Log("[IAPManager] Purchases fetched successfully.");

            IsInitialized = true;

            foreach (var product in s_storeController.GetProducts())
            {
                s_storeController.CheckEntitlement(product);
            }

            OnInitialized?.Invoke();

            Debug.Log("[IAPManager] Shop has been initialized.");
        }

        private static void StoreController_OnPurchasesFetchFailed(PurchasesFetchFailureDescription description)
        {
            Debug.LogWarning($"[IAPManager] Failed to fetch purchases! Reason: {description?.Message}");
        }

        private static void StoreController_OnPurchasePending(PendingOrder order)
        {
            Debug.Log($"[IAPManager] Purchase pending for order: {order}");

            s_storeController.ConfirmPurchase(order);
        }

        private static void StoreController_OnPurchaseConfirmed(Order order)
        {
            Debug.Log($"[IAPManager] Purchase confirmed for order: {order}");

            if (order?.Info?.PurchasedProductInfo == null)
            {
                Debug.LogWarning($"[IAPManager] No purchased product info found for order: {order}!");
                return;
            }

            foreach (var product in order.Info.PurchasedProductInfo)
            {
                ProductModel purchasedProduct = _products.Find(p => p.Id == product.productId);
                if (purchasedProduct == null)
                {
                    Debug.LogWarning($"[IAPManager] No matching product found for purchase: {product.productId}!");
                    continue;
                }

                ShopManager.Instance.ProductPurchased(purchasedProduct.Id, GetPurchaseQuantity(order.Info.Receipt));
            }
        }

        private static int GetPurchaseQuantity(string receipt)
        {
            int quantity = 1;

            if (!string.IsNullOrEmpty(receipt))
            {
                IAPPayData payData = JsonUtility.FromJson<IAPPayData>(receipt);
                if (payData.Store != "fake")
                {
                    IAPPayload payload = JsonUtility.FromJson<IAPPayload>(payData.Payload);
                    IAPPayloadData payloadData = JsonUtility.FromJson<IAPPayloadData>(payload.json);
                    quantity = payloadData.quantity;
                }
            }

            return quantity;
        }

        private static void StoreController_OnPurchaseFailed(FailedOrder order)
        {
            Debug.LogWarning($"[IAPManager] Purchase failed for order: {order}! Details: {order?.Details}");

            // Show UI like "Purchase failed. Please try again."
            OnPurchaseFailed?.Invoke(order);
        }

        private static void StoreController_OnPurchaseDeferred(DeferredOrder order)
        {
            Debug.Log($"[IAPManager] Purchase deferred for order: {order}.");

            // Show UI like "Purchase pending approval."
            OnPurchaseDeferred?.Invoke(order);
        }

        private static void StoreController_OnCheckEntitlement(Entitlement entitlement)
        {
            Product product = entitlement.Product;
            var status = entitlement.Status;

            Debug.Log($"[IAPManager] Checking entitlement for product: {product}, Status: {status}");

            // Only for non-consumable products, as consumables are typically consumed immediately after purchase
            bool isEntitled = status == EntitlementStatus.FullyEntitled;
            if (isEntitled)
            {
                ProductModel entitledProduct = _products.Find(p => p.Id == product.definition.id);
                if (entitledProduct == null)
                {
                    Debug.LogWarning($"[IAPManager] No matching product found for entitlement: {product.definition.id}!");
                    return;
                }

                // Grant the entitlement to the user. For non-consumables, this might be a one-time grant.
                if (entitledProduct.PurchasedQuantity < 1)
                {
                    ShopManager.Instance.ProductPurchased(entitledProduct.Id);
                }
            }
        }
    }
}
