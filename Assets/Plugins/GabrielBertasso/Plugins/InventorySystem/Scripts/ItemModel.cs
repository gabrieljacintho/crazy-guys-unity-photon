using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace GabrielBertasso.InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "Gabriel Bertasso/Item")]
    public class ItemModel : ScriptableObject
    {
        [Tooltip("E.g. com.studio.mygame.gold_100")]
        [SerializeField] private string _id = Guid.NewGuid().ToString();
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private GameObject _viewPrefab;
        [SerializeField] private GameObject _gamePrefab;
        [Tooltip("Set negative to infinity.")]
        [SerializeField] private int _maxStackSize = 1;
        [SerializeField] private bool _isEquippable;
        [ShowIf(nameof(IsEquippable)), Tooltip("If empty, the id is used.")]
        [SerializeField] private string _equipmentCategory;
        [ShowIf(nameof(IsEquippable))]
        [SerializeField] private int _minEquippedSlotsInCategory;
        [ShowIf(nameof(IsEquippable)), Tooltip("Set negative to infinity.")]
        [SerializeField] private int _maxEquippedSlotsInCategory = 1;

        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public GameObject ViewPrefab => _viewPrefab;
        public GameObject GamePrefab => _gamePrefab;
        public int MaxStackSize => _maxStackSize;
        public bool IsEquippable => _isEquippable;
        public string EquipmentCategory => !string.IsNullOrEmpty(_equipmentCategory) ? _equipmentCategory : Id;
        public int MinEquippedSlotsInCategory => _minEquippedSlotsInCategory;
        public int MaxEquippedSlotsInCategory => _maxEquippedSlotsInCategory;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = Guid.NewGuid().ToString();
            }
        }

        public static implicit operator string(ItemModel model)
        {
            return model.Id;
        }
    }
}
