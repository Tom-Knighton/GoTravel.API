namespace GoTravel.API.Domain.Extensions;

public static class LinqExtensions
{
    public static bool Most<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var count = 0;
        var total = 0;

        foreach (var element in source)
        {
            if (predicate(element))
            {
                count++;
            }

            total++;
            if (count > total / 2)
            {
                return true;
            }

            if (total - count > total / 2)
            {
                return false;
            }
        }

        return total % 2 != 0 && count > total / 2;
    }
}