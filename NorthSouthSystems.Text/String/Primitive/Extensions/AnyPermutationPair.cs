namespace NorthSouthSystems.Text;

public static partial class StringExtensions
{
    internal static bool AnyPermutationPairContains(
        IEnumerable<string> values,
        StringComparison comparison = default) =>
        AnyPermutationPair(values, (x, y) => x.Contains(y, comparison));

    internal static bool AnyPermutationPairStartsWith(
        IEnumerable<string> values,
        StringComparison comparison = default) =>
        AnyPermutationPair(values, (x, y) => x.StartsWith(y, comparison));

    internal static bool AnyPermutationPairEndsWith(
        IEnumerable<string> values,
        StringComparison comparison = default) =>
        AnyPermutationPair(values, (x, y) => x.EndsWith(y, comparison));

    internal static bool AnyPermutationPair(IEnumerable<string> values, Func<string, string, bool> operation)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(operation);

        string[] valuesLengthDescending = values.OrderByDescending(s => s.Length).ToArray();

        for (int i = 0; i < valuesLengthDescending.Length - 1; i++)
        for (int j = i + 1; j < valuesLengthDescending.Length; j++)
        {
            if (operation(valuesLengthDescending[i], valuesLengthDescending[j]))
                return true;
        }

        return false;
    }
}
