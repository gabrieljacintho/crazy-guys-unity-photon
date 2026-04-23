using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.ShopSystem
{
    public class PurchaseEvent : MonoBehaviour
    {
        [SerializeField] private ShopManager _shop;
        [SerializeField] private ProductModel _product;

        [Space]
        public UnityEvent OnPurchased;
        public UnityEvent OnPurchasesRestored;


        private void Awake()
        {
            if (_shop == null)
            {
                _shop = ShopManager.Instance;
            }
        }

        private void OnEnable()
        {
            _shop.OnProductPurchased += OnProductPurchased;
            _shop.OnPurchasesRestored += OnPurchasesRestored.Invoke;
        }

        private void OnDisable()
        {
            _shop.OnProductPurchased -= OnProductPurchased;
            _shop.OnPurchasesRestored -= OnPurchasesRestored.Invoke;
        }


        private void OnProductPurchased(ProductModel model)
        {
            if (model == _product)
            {
                OnPurchased?.Invoke();
            }
        }
    }
}