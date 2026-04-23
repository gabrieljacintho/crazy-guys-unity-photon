using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GabrielBertasso.UI
{
    public class CreditsPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _pointerBlockObject;
        [SerializeField] private bool _loop = true;

        [Header("Inputs")]
        [SerializeField] private InputActionReference _navigateInput;
        [SerializeField] private InputActionReference _pointerInput;
        [SerializeField] private InputActionReference _scrollInput;

        [Header("SettingsSettings")]
        [SerializeField] private float _autoScrollSpeed = 20f;
        [SerializeField] private float _autoScrollDelay = 5f;
        [SerializeField] private float _scrollSensitivity = 100f;
        [SerializeField] private float _screenHeight = 1080f;

        private Selectable[] _allSelectables;
        private GameObject _lastSelectedObject;
        private float _currentAutoScrollSpeed;
        private float _autoScrollDelayTime;
        private bool _ended;

        [Space]
        public UnityEvent OnEnd;


        private void Awake()
        {
            _allSelectables = _content.GetComponentsInChildren<Selectable>(true);
        }

        private void OnEnable()
        {
            EventManager.CurrentSelectedObject = null;
            _autoScrollDelayTime = 0f;
            _currentAutoScrollSpeed = _autoScrollSpeed;
            RestartScroll();
        }

        private void OnDisable()
        {
            RestartScroll();
        }

        private void LateUpdate()
        {
            UpdatePointerBlock();
            UpdateScrollInput();

            if (_autoScrollDelayTime > 0f)
            {
                _autoScrollDelayTime -= Time.unscaledDeltaTime;
            }

            GameObject selectedObject = EventManager.CurrentSelectedObject;
            if (selectedObject != null)
            {
                UpdateAutoDeselect(selectedObject);
                return;
            }

            UpdateNavigateInput();

            if (_autoScrollDelayTime <= 0f)
            {
                UpdateAutoScroll();
            }
        }

        public void MultiplySpeed(float value)
        {
            _currentAutoScrollSpeed *= value;
        }

        private void UpdateNavigateInput()
        {
            if (_navigateInput == null)
            {
                return;
            }

            if (_navigateInput.action.ReadValue<Vector2>().magnitude >= 0.01f)
            {
                SelectCenterUIElement();
            }
        }

        private void SelectCenterUIElement()
        {
            if (_allSelectables.Length == 0)
            {
                return;
            }

            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            Selectable closest = _allSelectables
                .Where(s => s.IsInteractable() && s.gameObject.activeInHierarchy
                    && Vector2.Distance(screenCenter, s.transform.position) <= screenCenter.y)
                .OrderBy(s => Vector2.Distance(screenCenter, s.transform.position))
                .FirstOrDefault();

            if (closest != null)
            {
                EventManager.CurrentSelectedObject = closest.gameObject;
                _autoScrollDelayTime = _autoScrollDelay;
            }
        }

        private void UpdatePointerBlock()
        {
            if (_pointerBlockObject == null || _pointerInput == null)
            {
                return;
            }

            bool active = EventManager.CurrentSelectedObject == null && _pointerInput.action.ReadValue<Vector2>().magnitude <= 0.01f;
            _pointerBlockObject.SetActive(active);
        }

        private void UpdateScrollInput()
        {
            if (_scrollInput == null)
            {
                return;
            }

            float delta = _scrollSensitivity * -_scrollInput.action.ReadValue<Vector2>().y;
            Scroll(delta);

            if (delta != 0f)
            {
                _autoScrollDelayTime = _autoScrollDelay;
            }
        }

        private void UpdateAutoScroll()
        {
            Scroll(_currentAutoScrollSpeed * Time.unscaledDeltaTime);
        }

        private void Scroll(float delta)
        {
            if (delta == 0f)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            Vector2 position = _content.anchoredPosition;
            position.y += delta;

            float height = _content.rect.height + _screenHeight;

            if (position.y >= _content.rect.height)
            {
                if (!_ended)
                {
                    _ended = true;
                    OnEnd?.Invoke();
                }

                if (_loop)
                {
                    position.y -= height;
                    _ended = false;
                }
                else
                {
                    position.y = _content.rect.height;
                }
            }
            else if (position.y <= -_screenHeight)
            {
                position.y += height;
            }

            _content.anchoredPosition = position;
        }

        private void RestartScroll()
        {
            _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, -_screenHeight);
            _ended = false;
        }

        private void UpdateAutoDeselect(GameObject selectedObject)
        {
            if (selectedObject != _lastSelectedObject)
            {
                _lastSelectedObject = selectedObject;
                _autoScrollDelayTime = _autoScrollDelay;
            }
            else if (_autoScrollDelayTime <= 0f)
            {
                EventManager.CurrentSelectedObject = null;
            }
        }
    }
}