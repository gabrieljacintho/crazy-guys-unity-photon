using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GabrielBertasso.UI.Scroll
{
    public class FixScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        [SerializeField] private ScrollRect _scrollRect;


        private void Awake()
        {
            if (_scrollRect == null)
            {
                _scrollRect = GetComponentInParent<ScrollRect>();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_scrollRect != null)
            {
                _scrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_scrollRect != null)
            {
                _scrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_scrollRect != null)
            {
                _scrollRect.OnEndDrag(eventData);
            }
        }

        public void OnScroll(PointerEventData data)
        {
            if (_scrollRect != null)
            {
                _scrollRect.OnScroll(data);
            }
        }
    }
}