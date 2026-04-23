using GabrielBertasso.Cache;
using UnityEngine;

namespace GabrielBertasso.UI.Scroll
{
    public class AutoScrollToSelectedObject : MonoBehaviour
    {
        [SerializeField] private RectTransform _scrollRectTransform;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;
        [SerializeField] private float _smoothTime = 0.3f;

        private Vector2 _velocity;


        private void LateUpdate()
        {
            GameObject selectedObject = EventManager.CurrentSelectedObject;
            if (selectedObject == null || !selectedObject.transform.IsChildOf(transform))
            {
                return;
            }

            RectTransform rectTransform = ComponentCacheManager.GetComponent<RectTransform>(selectedObject);

            SnapTo(rectTransform);
        }

        public void SnapTo(RectTransform target)
        {
            if (_content == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            Vector2 contentPosition = _scrollRectTransform.InverseTransformPoint(_content.position);
            Vector2 viewportPosition = _scrollRectTransform.InverseTransformPoint(_viewport.position);
            Vector2 targetPosition = _scrollRectTransform.InverseTransformPoint(target.position);

            targetPosition = contentPosition - targetPosition - viewportPosition;
            targetPosition.x = _content.anchoredPosition.x;

            if (targetPosition.y < 0f)
            {
                targetPosition.y = 0f;
            }

            float maxY = _content.rect.height - _viewport.rect.height;
            if (targetPosition.y > maxY)
            {
                targetPosition.y = maxY;
            }

            targetPosition = Vector2.SmoothDamp(_content.anchoredPosition, targetPosition, ref _velocity, _smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

            _content.anchoredPosition = targetPosition;
        }
    }
}