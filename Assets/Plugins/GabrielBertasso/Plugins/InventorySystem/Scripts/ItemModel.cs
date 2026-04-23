using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace GabrielBertasso.InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "Gabriel Bertasso/Item")]
    public class ItemModel : ScriptableObject
    {
        public string Id = Guid.NewGuid().ToString();
        public string Name;
        public string Description;
        public Sprite Icon;
        public GameObject ViewPrefab;
        public GameObject GamePrefab;
        [Tooltip("Set negative to infinity.")]
        public int MaxStackSize = 1;
        public bool IsEquipable;
        [ShowIf(nameof(IsEquipable))]
        public string EquipmentCategory;
        [ShowIf(nameof(IsEquipable)), Tooltip("Set negative to infinity.")]
        public int MaxEquippedItemsInCategory = 1;

        public static implicit operator string(ItemModel model)
        {
            return model.Id;
        }
    }
}
