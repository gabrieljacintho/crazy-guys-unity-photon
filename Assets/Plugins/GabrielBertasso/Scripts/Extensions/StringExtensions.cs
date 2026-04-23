using System.Text.RegularExpressions;

namespace GabrielBertasso.Extensions
{
    public static class StringExtensions
    {
        public static string ToUpperFirstChar(this string text)
        {
            if (text.Length < 2)
            {
                return text;
            }

            return text[0].ToString().ToUpper() + text.Substring(1);
        }

        public static string ToLowerFirstChar(this string text)
        {
            if (text.Length < 2)
            {
                return text;
            }

            return text[0].ToString().ToLower() + text.Substring(1);
        }

        public static string GetUntil(this string text, string stopAt)
        {
            if (!text.Contains(stopAt))
            {
                return text;
            }

            int endIndex = text.IndexOf(stopAt);

            return text.Substring(0, endIndex);
        }

        public static string RemoveExtraSpaces(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // 1. Trim spaces at the start and end
            string result = text.Trim();

            // 2. Replace multiple spaces or tabs with a single space
            result = Regex.Replace(result, @"[ \t]{2,}", " ");

            return result;
        }


        public static string CapitalizeSentences(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            // Don't lowercase everything; preserve existing capitalization for proper nouns, acronyms, etc.
            return Regex.Replace(
                text,
                @"(^[a-z])|(?<=[\.!\?]\s+)[a-z]|(?<=[\.!\?]\r?\n\s*)[a-z]|(?<=[\.!\?]['"")\s+])[a-z]",
                m => m.Value.ToUpper()
            );
        }

        public static string FixText(this string text)
        {
            return text.RemoveExtraSpaces().CapitalizeSentences();
        }
    }
}