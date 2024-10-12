using System.Diagnostics.CodeAnalysis;

namespace ApogeeDev.IdentityProvider.Host;

public class GenericEqualityComparer<T> : IEqualityComparer<T>
{
    private readonly Func<T, T, bool> compareFn;

    public GenericEqualityComparer(Func<T, T, bool> compareFn)
    {
        this.compareFn = compareFn;
    }

    public bool Equals(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            return false;

        return compareFn(x, y);
    }

    public int GetHashCode([DisallowNull] T obj)
    {
        return 0;
    }
}

public static class GenericEqualityComparerExtensions
{
    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, IEnumerable<T> second,
        Func<T, T, bool> comparer)
    {
        return source.Except(second, new GenericEqualityComparer<T>(comparer));
    }
}