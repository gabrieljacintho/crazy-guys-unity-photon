using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GabrielBertasso.UI.Scroll
{
    public class ScrollRectInputController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private bool _getInParent = true;

        [Header("Inputs")]
        [SerializeField] private InputActionReference _scrollInput;

        [Header("Settings")]
        [Min(0f), SerializeField] private float _scrollSensitivity = 100f;
        [SerializeField] private bool _invert = true;
        [SerializeField] private bool _useDeltaTime;


        private void Awake()
        {
            if (_getInParent && _scrollRect == null)
            {
                _scrollRect = GetComponentInParent<ScrollRect>();
            }
        }

        private void LateUpdate()
        {
            if (_scrollRect == null || _scrollInput == null)
            {
                return;
            }

            float inputY = _scrollInput.action.ReadValue<Vector2>().y;
            if (Mathf.Abs(inputY) <= 0.01f)
            {
                return;
            }

            float delta = _scrollSensitivity * (_invert ? -inputY : inputY);
            if (_useDeltaTime)
            {
                delta *= Time.unscaledDeltaTime;
            }

            Scroll(delta);
        }

        private void Scroll(float delta)
        {
            if (delta == 0f)
            {
                return;
            }

            RectTransform content = _scrollRect.content;
            RectTransform viewport = _scrollRect.viewport;
            if (content == null)
            {
                return;
            }

            if (viewport == null)
            {
                viewport = _scrollRect.GetComponent<RectTransform>();
            }

            Canvas.ForceUpdateCanvases();

            float scrollableHeight = content.rect.height - viewport.rect.height;
            if (scrollableHeight <= 0f)
            {
                return;
            }

            float normalizedDelta = delta / scrollableHeight;
            float newPosition = _scrollRect.verticalNormalizedPosition - normalizedDelta;

            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newPosition);
        }
    }
}
