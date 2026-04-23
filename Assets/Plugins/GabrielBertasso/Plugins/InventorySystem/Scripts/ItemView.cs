using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GabrielBertasso.InventorySystem
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private InventoryManager _inventory;
        [SerializeField] private ItemModel _model;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _quantityText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private bool _canInstantiatePrefab;
        [ShowIf(nameof(_canInstantiatePrefab))]
        [SerializeField] private Transform _instanceParent;
        [ShowIf(nameof(_canInstantiatePrefab))]
        [SerializeField] private bool _useGamePrefab;
        [ShowIf(nameof(_canInstantiatePrefab))]
        [SerializeField] private bool _canInstantiateOnePrefabPerUnit;
        [ShowIf(nameof(_canInstantiatePrefab))]
        [SerializeField] private bool _canForceInstantiatePrefab;
        [ShowIf("@nameof(_canInstantiatePrefab) && !_canForceInstantiatePrefab")]
        [SerializeField] private bool _canOnlyInstantiateIfEquipped;

        private List<GameObject> _instances = new List<GameObject>();


        private void Awake()
        {
            if (_inventory == null)
            {
                _inventory = InventoryManager.Instance;
            }
        }

        private void OnEnable()
        {
            if (_inventory.IsInitialized)
            {
                UpdateView();
            }
            else
            {
                _inventory.OnInitialized += UpdateView;
            }

            _inventory.OnItemQuantityChanged += InventoryManager_OnItemQuantityChanged;
            _inventory.OnItemsRestored += UpdateView;
        }

        private void OnDisable()
        {
            if (_inventory != null)
            {
                _inventory.OnInitialized -= UpdateView;
                _inventory.OnItemQuantityChanged -= InventoryManager_OnItemQuantityChanged;
                _inventory.OnItemsRestored -= UpdateView;
            }
        }

        private void UpdateView()
        {
            if (_nameText != null)
            {
                _nameText.text = _model.Name;
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = _model.Description;
            }

            if (_quantityText != null)
            {
                _quantityText.text = _inventory.GetItemQuantity(_model).ToString();
            }

            if (_iconImage != null)
            {
                _iconImage.sprite = _model.Icon;
            }

            UpdateInstances();
        }

        private void UpdateInstances()
        {
            int quantity = GetTargetInstancesCount();

            GameObject prefab = _useGamePrefab ? _model.GamePrefab : _model.ViewPrefab;
            
            for (int i = _instances.Count; i < quantity; i++)
            {
                GameObject instance = Instantiate(prefab, _instanceParent);
                _instances.Add(instance);
            }

            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                GameObject instance = _instances[i];
                if (instance != null)
                {
                    Destroy(instance);
                }
                _instances.RemoveAt(i);
            }
        }

        private int GetTargetInstancesCount()
        {
            int quantity = _inventory.GetItemQuantity(_model);

            if (!_canInstantiateOnePrefabPerUnit)
            {
                quantity = Mathf.Min(quantity, 1);
            }

            if (_canForceInstantiatePrefab)
            {
                quantity = Mathf.Max(quantity, 1);
            }
            else if (_canOnlyInstantiateIfEquipped && !_inventory.IsEquipped(_model))
            {
                quantity = 0;
            }

            return quantity;
        }

        private void InventoryManager_OnItemQuantityChanged(ItemModel item)
        {
            if (_model == item)
            {
                UpdateView();
            }
        }
    }
}