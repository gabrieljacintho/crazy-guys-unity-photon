using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GabrielBertasso
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private AssetReferenceScene _scene;


        private void Start()
        {
            StartCoroutine(Load());
        }

        public IEnumerator Load()
        {
            var handle = Addressables.LoadSceneAsync(_scene);
            yield return handle;
            Debug.Log("Scene loaded.");
        }
    }
}