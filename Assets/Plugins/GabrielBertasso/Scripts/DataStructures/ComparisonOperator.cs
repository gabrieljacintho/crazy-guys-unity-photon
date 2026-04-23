using System;

namespace GabrielBertasso.DataStructures
{
    [Serializable]
    public enum ComparisonOperator
    {
        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }

    public static class ComparisonOperatorExtensions
    {
        public static bool Compare(this ComparisonOperator op, int a, int b)
        {
            return op switch
            {
                ComparisonOperator.EqualTo => a == b,
                ComparisonOperator.NotEqualTo => a != b,
                ComparisonOperator.GreaterThan => a > b,
                ComparisonOperator.GreaterThanOrEqualTo => a >= b,
                ComparisonOperator.LessThan => a < b,
                ComparisonOperator.LessThanOrEqualTo => a <= b,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
    }
}