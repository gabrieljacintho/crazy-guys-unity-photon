#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace GabrielBertasso.UI
{
    public class FitImageToScreen : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _canvasRectTransform;

        private RectTransform _rectTransform;


        private void Awake()
        {
            _rectTransform = _image.rectTransform;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
                EditorUtility.SetDirty(this);
            }

            if (_canvasRectTransform == null)
            {
                _canvasRectTransform = GetComponentInParent<Canvas>(true).GetComponent<RectTransform>();
                EditorUtility.SetDirty(this);
            }
        }
#endif
        private void Update()
        {
            _rectTransform.anchorMin = Vector2.one * 0.5f;
            _rectTransform.anchorMax = Vector2.one * 0.5f;
            _rectTransform.anchoredPosition3D = Vector2.zero;
            _rectTransform.localScale = Vector3.one;

            Vector2 resolution = new(_canvasRectTransform.rect.width, _canvasRectTransform.rect.height);
            float screenRatio = resolution.x / resolution.y;
            float imageRatio = _image.sprite.bounds.size.x / _image.sprite.bounds.size.y;

            if (screenRatio > imageRatio)
            {
                // Tela mais larga → ajustar largura
                _rectTransform.sizeDelta = new Vector2(resolution.x, resolution.x / imageRatio);
            }
            else
            {
                // Tela mais alta → ajustar altura
                _rectTransform.sizeDelta = new Vector2(resolution.y * imageRatio, resolution.y);
            }
        }
    }
}