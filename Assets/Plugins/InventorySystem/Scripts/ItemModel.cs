using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso.InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "Gabriel Bertasso/Item")]
    public class ItemModel : ScriptableObject
    {
        public string Id;
        public string Name;
        public string Description;
        public Sprite Icon;
        public GameObject Prefab;
        [Min(1)]
        public int MaxStackSize = 1;
        public bool IsEquippable;
        [ShowIf(nameof(IsEquippable))]
        public string EquipamentCategory;
        [ShowIf(nameof(IsEquippable)), Tooltip("Set negative to infinity.")]
        public int MaxEquippedItemsInCategory = 1;
    }
}
