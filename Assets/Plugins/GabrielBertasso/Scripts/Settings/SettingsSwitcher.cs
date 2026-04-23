using GabrielBertasso.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso.Settings
{
    public abstract class SettingsSwitcher : Switcher, ISettingsHandler
    {
        [SerializeField] private bool _autoApplyChanges;
        [SerializeField] private bool _loadAppliedSettingsOnEnable;

        public GameObject GameObject => gameObject;
        public virtual bool IsPlatformCompatible => true;
        protected abstract int AppliedIndex { get; }
        protected abstract int DefaultIndex { get; }


        protected override void Awake()
        {
            base.Awake();

            if (_autoApplyChanges)
            {
                OnValueChanged.AddListener(Apply);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_loadAppliedSettingsOnEnable && HasChanged())
            {
                Cancel();
            }
        }

        [Button]
        public void Apply()
        {
            Apply(CurrentIndex);
        }

        [Button]
        public void Cancel() => CurrentIndex = AppliedIndex;

        [Button]
        public void ResetSettings() => CurrentIndex = DefaultIndex;

        public bool IsApplied() => AppliedIndex == CurrentIndex;
        public bool IsDefault() => CurrentIndex == DefaultIndex;

        public bool HasChanged() => AppliedIndex != CurrentIndex;

        protected abstract void Apply(int index);
    }
}