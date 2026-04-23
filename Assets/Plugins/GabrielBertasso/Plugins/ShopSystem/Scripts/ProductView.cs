using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GabrielBertasso.ShopSystem
{
    public class ProductView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _iconImage;

        [Header("Purchase Button")]
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private Image _purchaseButtonImage;
        [SerializeField] private Color _defaultPurchaseColor = Color.green;
        [SerializeField] private Color _purchasedPurchaseColor = Color.blue;
        [SerializeField] private Color _unavailablePurchaseColor = Color.gray;
        [SerializeField] private Color _noSpaceAvailableInInventoryPurchaseColor = Color.gray;
        [SerializeField] private TMP_Text _purchaseButtonText;
        [SerializeField] private string _purchasedText = "Purchased";
        [SerializeField] private string _unavailableText = "Unavailable";
        [SerializeField] private string _noSpaceAvailableInInventoryText = "No space available";

        [Header("Equip Button")]
        [SerializeField] private Button _equipButton;
        [SerializeField] private Image _equipButtonImage;
        [SerializeField] private Color _equipEquipColor = Color.green;
        [SerializeField] private Color _unequipEquipColor = Color.blue;
        [SerializeField] private TMP_Text _equipButtonText;
        [SerializeField] private string _equipText = "Equip";
        [SerializeField] private string _unequipText = "Unequip";

        [ShowInInspector, ReadOnly] private ProductModel _model;


        private void Awake()
        {
            if (_purchaseButton != null)
            {
                _purchaseButton.onClick.AddListener(PurchaseButton_OnClick);
            }

            if (_equipButton != null)
            {
                _equipButton.onClick.AddListener(EquipButton_OnClick);
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
            if (_titleText != null)
            {
                _titleText.text = _model.Title;
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = _model.Description;
            }

            if (_iconImage != null)
            {
                _iconImage.sprite = _model.Icon;
            }

            UpdateEquipButton();
            UpdatePurchaseButton();
        }

        private void UpdateEquipButton()
        {
            if (_equipButton != null)
            {
                _equipButton.gameObject.SetActive(_model.CanBeEquipped());
                _equipButton.interactable = true;
            }

            bool isEquipButtonActive = _equipButton != null && _equipButton.gameObject.activeInHierarchy;
            bool isEquipped = _model.Inventory.IsEquipped(_model.Item);

            if (_equipButtonImage != null)
            {
                _equipButtonImage.gameObject.SetActive(isEquipButtonActive);

                if (isEquipped)
                {
                    _equipButtonImage.color = _unequipEquipColor;
                }
                else
                {
                    _equipButtonImage.color = _equipEquipColor;
                }
            }

            if (_equipButtonText != null)
            {
                _equipButtonText.gameObject.SetActive(isEquipButtonActive);

                if (isEquipped)
                {
                    _equipButtonText.text = _unequipText;
                }
                else
                {
                    _equipButtonText.text = _equipText;
                }
            }
        }

        private void UpdatePurchaseButton()
        {
            bool canBePurchased = _model.CanBePurchased();

            if (_purchaseButton != null)
            {
                _purchaseButton.gameObject.SetActive(canBePurchased || _equipButton == null || !_equipButton.gameObject.activeInHierarchy);
                _purchaseButton.interactable = canBePurchased;
            }

            bool isPurchaseButtonActive = _purchaseButton != null && _purchaseButton.gameObject.activeInHierarchy;
            bool hasAvailableSpaceInInventory = _model.HasEnoughAvailableSpaceInInventory();

            if (_purchaseButtonImage != null)
            {
                _purchaseButtonImage.gameObject.SetActive(isPurchaseButtonActive);

                if (!_model.IsAvailable)
                {
                    _purchaseButtonImage.color = _unavailablePurchaseColor;
                }
                else if (_model.IsEntitled)
                {
                    _purchaseButtonImage.color = _purchasedPurchaseColor;
                }
                else if (!hasAvailableSpaceInInventory)
                {
                    _purchaseButtonImage.color = _noSpaceAvailableInInventoryPurchaseColor;
                }
                else
                {
                    _purchaseButtonImage.color = _defaultPurchaseColor;
                }
            }

            if (_purchaseButtonText != null)
            {
                _purchaseButtonText.gameObject.SetActive(isPurchaseButtonActive);

                if (!_model.IsAvailable)
                {
                    _purchaseButtonText.text = _unavailableText;
                }
                else if (_model.IsEntitled)
                {
                    _purchaseButtonText.text = _purchasedText;
                }
                else if (!hasAvailableSpaceInInventory)
                {
                    _purchaseButtonText.text = _noSpaceAvailableInInventoryText;
                }
                else
                {
                    _purchaseButtonText.text = _model.Price;
                }
            }
        }

        private void PurchaseButton_OnClick()
        {
            _model.Shop.PurchaseProduct(_model.Id);
        }

        private void EquipButton_OnClick()
        {
            _model.Shop.TryToggleEquipProduct(_model.Id);
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