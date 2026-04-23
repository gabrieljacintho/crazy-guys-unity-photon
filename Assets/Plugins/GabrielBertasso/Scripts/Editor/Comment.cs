using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso
{
    public class Comment : MonoBehaviour
    {
        [TextArea(1, 5), HideLabel]
        [SerializeField] private string _text;
    }
}