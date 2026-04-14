using UnityEngine;
using UnityEngine.Purchasing;

namespace GabrielBertasso.IAP
{
    public class IAPManager : MonoBehaviour
    {
        private StoreController _storeController;


        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            var catalogProvider = new CatalogProvider();
        }
    }
}
