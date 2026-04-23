using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GabrielBertasso.UI.Tab
{
    [HelpURL("https://www.youtube.com/watch?v=211t6r12XPQ")]
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private TabGroup _tabGroup;
        [SerializeField] private GameObject _content;

        [Space]
        public UnityEvent OnHover;
        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;

        private Image _image;

        public GameObject Content => _content;

        public Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }

                return _image;
            }
        }


        private void Awake()
        {
            if (_tabGroup != null)
            {
                _tabGroup.Subscribe(this);
            }
        }

        private void OnDestroy()
        {
            if (_tabGroup != null)
            {
                _tabGroup.Unsubscribe(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_tabGroup != null)
            {
                _tabGroup.OnTabEnter(this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_tabGroup != null)
            {
                _tabGroup.OnTabSelected(this);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_tabGroup != null)
            {
                _tabGroup.OnTabExit(this);
            }
        }
    }
}