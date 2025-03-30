namespace easyLib.Extensions;

public static class ListEx
{
    public static void Put<T>(this IList<T> list, T item, int ndx)
    {
        require(list != null);
        require(ndx >= 0);
        require(ndx <= list.Count);

        if (ndx == list.Count)
            list.Add(item);
        else
            list[ndx] = item;
    }

    public static int Put<T>(this IList<T> list, IEnumerable<T> items, int ndx)
    {
        require(list != null);
        require(items != null);
        require(ndx >= 0);
        require(ndx <= list.Count);

        using IEnumerator<T> enumerator = items.GetEnumerator();
        int n = 0;

        while (ndx < list.Count && enumerator.MoveNext())
        {
            list[ndx++] = enumerator.Current;
            ++n;
        }

        while (enumerator.MoveNext())
        {
            list.Add(enumerator.Current);
            ++n;
        }

        return n;
    }

    public static int IndexOf<T>(this IReadOnlyList<T> list, T item, int ndxStart, Func<T, T, bool>? eqls = null)
    {
        require(list != null);
        require(ndxStart >= 0);
        require(ndxStart <= list.Count);

        eqls ??= EqualityComparer<T>.Default.Equals;

        for (int i = ndxStart; i < list.Count; ++i)
            if (eqls(list[i], item))
                return i;

        return -1;
    }
}
