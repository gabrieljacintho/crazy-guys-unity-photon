using GabrielBertasso.GameManagement;
using GabrielBertasso.Settings;
using UnityEngine;

namespace GabrielBertasso.DevTool
{
    public class TimeScaleSwitcher : SettingsSwitcher
    {
        [Header("Time Scale")]
        [SerializeField] private float _maxTimeScale = 3f;
        [SerializeField] private float _delta = 0.1f;

        protected override int ValuesLength => Mathf.RoundToInt(_maxTimeScale / _delta);
        protected override int AppliedIndex => TimeScale <= 0f ? 0 : Mathf.RoundToInt(TimeScale / _delta);
        protected override int DefaultIndex => Mathf.RoundToInt(1f / _delta);
        private static float TimeScale
        {
            get => GameManager.TargetTimeScale;
            set => GameManager.TargetTimeScale = value;
        }


        protected override string GetText(int index)
        {
            return (index * _delta).ToString("F1");
        }

        protected override void Apply(int index)
        {
            TimeScale = index * _delta;
        }
    }
}