using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GabrielBertasso
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private AssetReferenceScene _scene;


        private void Start()
        {
            Load();
        }

        public void Load()
        {
            Addressables.LoadSceneAsync(_scene);
        }
    }
}