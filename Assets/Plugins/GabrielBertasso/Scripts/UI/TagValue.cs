using System;
#if I2_LOCALIZATION
using I2.Loc;
#endif
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace GabrielBertasso.UI
{
    [Serializable]
    public enum TagType
    {
        String,
#if I2_LOCALIZATION
        LocalizedString,
#endif
        Float,
        Int,
    }

    [Serializable]
    public struct TagValue
    {
        public string Tag;
        [EnumToggleButtons, HideLabel]
        public TagType Type;
        [ShowIf("@Type == TagType.String")]
        public StringVariable StringVariable;
#if I2_LOCALIZATION
        [ShowIf("@Type == TagType.LocalizedString")]
        public LocalizedString LocalizedString;
#endif
        [ShowIf("@Type == TagType.Float")]
        public FloatVariable FloatVariable;
        [ShowIf("@Type == TagType.Float")]
        [Range(0, 3)] public int DecimalDigits;
        [ShowIf("@Type == TagType.Int")]
        public IntVariable IntVariable;


        public readonly string GetText()
        {
            switch (Type)
            {
                case TagType.String:
                    return StringVariable.Value;
#if I2_LOCALIZATION
                case TagType.LocalizedString:
                    return LocalizedString;
#endif
                case TagType.Float:
                    string format = "F" + DecimalDigits;
                    return FloatVariable.Value.ToString(format);

                case TagType.Int:
                    return IntVariable.Value.ToString();

                default:
                    return null;
            }
        }
    }
}