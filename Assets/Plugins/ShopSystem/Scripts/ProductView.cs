using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GabrielBertasso.ShopSystem
{
    public class ProductView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Image _icon;

        [ShowInInspector, ReadOnly] private ProductModel _model;


        private void Awake()
        {
            if (_buyButton != null)
            {
                _buyButton.onClick.AddListener(OnBuyButtonClicked);
            }
        }

        public void Initialize(ProductModel model)
        {
            if (_model != null)
            {
                _model.Shop.OnProductPurchased -= OnProductPurchased;
            }

            _model = model;
            _model.Shop.OnProductPurchased += OnProductPurchased;

            UpdateView();
        }

        public void UpdateView()
        {
            if (_title != null)
            {
                _title.text = _model.Title;
            }

            if (_price != null)
            {
                if (!_model.IsAvailable)
                {
                    _price.text = "Unavailable";
                }
                else if (_model.IsEntitled)
                {
                    _price.text = "Purchased";
                }
                else
                {
                    _price.text = _model.Price;
                }
            }

            if (_description != null)
            {
                _description.text = _model.Description;
            }

            if (_buyButton != null)
            {
                _buyButton.interactable = _model.IsAvailable && !_model.IsEntitled;
            }

            if (_icon != null)
            {
                _icon.sprite = _model.Icon;
            }
        }

        private void OnBuyButtonClicked()
        {
            _model.Shop.PurchaseProduct(_model.Id);
        }

        private void OnProductPurchased(ProductModel model)
        {
            if (model == _model)
            {
                UpdateView();
            }
        }
    }
}