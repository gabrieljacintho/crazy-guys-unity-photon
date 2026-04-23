using System;
using System.Collections.Generic;
#if FMOD
using GabrielBertasso.FMODIntegration;
using FMODUnity;
#endif
using GabrielBertasso.Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GabrielBertasso.UI.Tab
{
    [Serializable]
    public struct TabState
    {
        public Color Color;
    }

    [HelpURL("https://www.youtube.com/watch?v=211t6r12XPQ")]
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private List<TabButton> _tabButtons;
        [SerializeField] private TabState _tabIdleState;
        [SerializeField] private TabState _tabHoverState;
        [SerializeField] private TabState _tabActiveState;
        [SerializeField] private bool _resetOnEnable = true;

        [Header("Inputs")]
        [SerializeField] private InputActionReference _previousTabInput;
        [SerializeField] private InputActionReference _nextTabInput;

        [Header("SFX")]
#if FMOD
        [SerializeField] private FMODEventSettings _tabChangeSFX;
#else
        [SerializeField] private AudioClip _tabChangeSFX;
#endif


        private TabButton _selectedTab;


        private void OnEnable()
        {
            if (_resetOnEnable)
            {
                UpdateTabs();
            }

            if (_nextTabInput != null)
            {
                _nextTabInput.action.performed += NextTab;
            }

            if (_previousTabInput != null)
            {
                _previousTabInput.action.performed += PreviousTab;
            }
        }

        private void OnDisable()
        {
            if (_nextTabInput != null)
            {
                _nextTabInput.action.performed -= NextTab;
            }

            if (_previousTabInput != null)
            {
                _previousTabInput.action.performed -= PreviousTab;
            }
        }

        public void Subscribe(TabButton button)
        {
            if (_tabButtons == null)
            {
                _tabButtons = new List<TabButton>();
            }

            if (!_tabButtons.Contains(button))
            {
                _tabButtons.Add(button);
            }
        }

        public void Unsubscribe(TabButton button)
        {
            _tabButtons?.Remove(button);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTabs();

            if (_selectedTab != button)
            {
                button.Image.color = _tabHoverState.Color;

                button.OnHover?.Invoke();
            }
        }

        public void OnTabSelected(TabButton button)
        {
            if (_selectedTab == button)
            {
                return;
            }

            if (_selectedTab != null)
            {
                _selectedTab.OnDeselect?.Invoke();
            }

            _selectedTab = button;

            ResetTabs();

            if (_selectedTab == null)
            {
                return;
            }

            if (_selectedTab.Content != null)
            {
                _selectedTab.Content.SetActive(true);
            }

            _selectedTab.Image.color = _tabActiveState.Color;

            _selectedTab.OnSelect?.Invoke();
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }

        private void ResetTabs()
        {
            foreach (TabButton button in _tabButtons)
            {
                if (_selectedTab == button)
                {
                    continue;
                }

                if (button.Content != null)
                {
                    button.Content.SetActive(false);
                }

                button.Image.color = _tabIdleState.Color;
            }
        }

        public void NextTab()
        {
            if (!ChangeTab() || !_tabButtons.Exists(x => x.gameObject.activeSelf))
            {
                return;
            }

            int i;
            do
            {
                i = _tabButtons.IndexOf(_selectedTab) + 1;
                i = MathHelper.WrapIndex(_tabButtons.Count, i);
            }
            while (!_tabButtons[i].gameObject.activeSelf);

            OnTabSelected(_tabButtons[i]);

            PlayTabChangeSFX();
        }

        private void NextTab(InputAction.CallbackContext context)
        {
            NextTab();
        }

        public void PreviousTab()
        {
            if (!ChangeTab() || !_tabButtons.Exists(x => x.gameObject.activeSelf))
            {
                return;
            }

            int i;
            do
            {
                i = _tabButtons.IndexOf(_selectedTab) - 1;
                i = MathHelper.WrapIndex(_tabButtons.Count, i);
            }
            while (!_tabButtons[i].gameObject.activeSelf);

            OnTabSelected(_tabButtons[i]);

            PlayTabChangeSFX();
        }

        private void PreviousTab(InputAction.CallbackContext context)
        {
            PreviousTab();
        }

        private bool ChangeTab()
        {
            if (_tabButtons == null || _tabButtons.Count == 0)
            {
                return false;
            }

            if (_selectedTab == null || _tabButtons.Count == 1)
            {
                OnTabSelected(_tabButtons[0]);
                return false;
            }

            if (!_tabButtons.Contains(_selectedTab))
            {
                Debug.LogError("Selected tab not found!", this);
                return false;
            }

            return true;
        }

        private void PlayTabChangeSFX()
        {
#if FMOD
            if (_tabChangeSFX.IsValid)
            {
                _tabChangeSFX.Play();
            }
#else
            if (_tabChangeSFX != null)
            {
                AudioSource.PlayClipAtPoint(_tabChangeSFX, default);
            }
#endif
        }

        public void UpdateTabs()
        {
            if (_tabButtons != null && _tabButtons.Count > 0)
            {
                OnTabSelected(_tabButtons[0]);
            }
            else
            {
                OnTabSelected(null);
            }
        }
    }
}