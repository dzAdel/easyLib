namespace easyLib.Extensions;

public static class EnumerableEx
{
    public static bool All<T>(this IEnumerable<T> src, Func<T, int, bool> predicate)
    {
        require(src != null);
        require(predicate != null);

        using IEnumerator<T> enumerator = src.GetEnumerator();

        int ndx = 0;

        while (enumerator.MoveNext())
            if (!predicate(enumerator.Current, ndx++))
                return false;

        return true;
    }

    public static bool IsSorted<T>(this IEnumerable<T> src, Comparison<T>? compare = null)
    {
        require(src != null);
        require(compare != null || typeof(T).Implements<IComparable<T>>() || typeof(T).Implements<IComparable>());

        using IEnumerator<T> enumerator = src.GetEnumerator();
        compare ??= Comparer<T>.Default.Compare;

        if (enumerator.MoveNext())
        {
            T prevItem = enumerator.Current;

            int cmpResult = 0;
            while (enumerator.MoveNext() && (cmpResult = compare(prevItem, enumerator.Current)) == 0)
                ;   //nop

            if (cmpResult == 0)   //all items are equal or src count == 1
                return true;

            prevItem = enumerator.Current;

            if (cmpResult < 0)    //use ascending order 
                while (enumerator.MoveNext())
                {
                    T elt = enumerator.Current;

                    if (compare(elt, prevItem) < 0)
                        return false;

                    prevItem = elt;
                }
            else    //use descending order
                while (enumerator.MoveNext())
                {
                    T elt = enumerator.Current;

                    if (compare(prevItem, elt) < 0)
                        return false;

                    prevItem = elt;
                }
        }

        return true;
    }

    public static bool IsOrdered<T>(this IEnumerable<T> src, Func<T, T, bool> precedes)
    {
        require(src != null);
        require(precedes != null);

        using IEnumerator<T> enumerator = src.GetEnumerator();

        if (enumerator.MoveNext())
        {
            T prevElt = enumerator.Current;

            while (enumerator.MoveNext())
            {
                T elt = enumerator.Current;

                if (precedes(elt, prevElt))
                    return false;

                prevElt = elt;
            }
        }

        return true;
    }

    public static int IndexOf<T>(this IEnumerable<T> src, T item, Func<T, T, bool>? eqls = null)
    {
        require(src != null);

        eqls ??= EqualityComparer<T>.Default.Equals;
        int ndx = 0;
        using IEnumerator<T> enumerator = src.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (eqls(enumerator.Current, item))
                return ndx;

            ++ndx;
        }

        return -1;
    }

    public static (T min, T max) MinMax<T>(this IEnumerable<T> src, Comparison<T>? compare = null)
    {
        require(src != null);
        require(src.Any());
        require(compare != null || typeof(T).Implements<IComparable<T>>());
        require(compare != null || typeof(T).Implements<IComparable>());

        compare ??= Comparer<T>.Default.Compare;
        using IEnumerator<T> enumerator = src.GetEnumerator();

        enumerator.MoveNext();
        T min = enumerator.Current;
        T max = min;

        while (enumerator.MoveNext())
        {
            T elt = enumerator.Current;
            if (compare(elt, min) < 0)
                min = elt;
            else if (compare(max, elt) < 0)
                max = elt;
        }

        return (min, max);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> src)
    {
        require(src != null);

        Random rand = new();

        return src.OrderBy(_ => rand.Next());
    }

    public static IEnumerable<T> Emit<T>(Func<T, T> generate, T initValue, T stopValue) //TODO: not an extension method. move it.
    {
        require(generate != null);

        while (!Equals(initValue, stopValue))
        {
            yield return initValue;

            initValue = generate(initValue);
        }
    }
}

