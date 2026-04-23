using UnityEngine;

namespace GabrielBertasso.Settings
{
    public interface ISettingsHandler
    {
        GameObject GameObject { get; }

        bool IsPlatformCompatible { get; }

        void Apply();

        void Cancel();

        void ResetSettings();

        bool IsApplied();

        bool IsDefault();

        bool HasChanged();
    }
}