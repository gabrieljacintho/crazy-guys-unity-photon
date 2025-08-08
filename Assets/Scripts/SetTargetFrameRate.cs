using UnityEngine;

namespace GabrielBertasso
{
    public class SetTargetFrameRate : MonoBehaviour
    {
        [SerializeField] private int _targetFrameRate = 60;


        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}