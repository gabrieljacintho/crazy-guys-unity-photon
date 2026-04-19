using GabrielBertasso.ShopSystem.IAP;
using UnityEngine;

namespace GabrielBertasso.ShopSystem
{
    public class ProductSpawner : MonoBehaviour
    {
        [SerializeField] private ProductModel _product;
        [SerializeField] private Transform _parent;


        private void Awake()
        {
            IAPManager.OnInitialized += IAPManager_OnInitialized;
            if (IAPManager.IsInitialized)
            {
                IAPManager_OnInitialized();
            }
        }

        private void OnDestroy()
        {
            IAPManager.OnInitialized -= IAPManager_OnInitialized;
        }

        private void IAPManager_OnInitialized()
        {
            _product = ShopManager.Instance.Products.Find(p => p.Id == _product.Id);
            if (_product == null)
            {
                Debug.LogWarning($"[ProductSpawner] IAPProduct with ID '{_product.Id}' not found in IAPManager.", this);
                return;
            }

            if (_product.IsPurchased)
            {
                SpawnReward();
            }
            else
            {
                //_product.OnPurchased += SpawnReward;
            }
        }

        private void SpawnReward()
        {
            if (_product.Item.Prefab == null)
            {
                Debug.LogWarning($"[ProductSpawner] No reward prefab assigned for product: {_product.Id}", this);
                return;
            }

            Instantiate(_product.Item.Prefab, _parent);
        }
    }
}