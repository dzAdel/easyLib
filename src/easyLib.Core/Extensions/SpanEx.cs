namespace easyLib.Extensions;

public static class SpanEx
{
    public static IEnumerable<T> ToEnumerable<T>(this ReadOnlySpan<T> span)
    {
        int len = span.Length;
        T[] items = ArrayPool.Alloc<T>(len);
        span.CopyTo(items);

        return enumerate(items, len);

        //local:
        static IEnumerable<T> enumerate(T[] src, int count)
        {
            for (int i = 0; i < count; ++i)
                yield return src[i];

            ArrayPool.Free(src);
        }
    }

    public static void ReverseSlice(this Span<byte> bytes, int szSlice)
    {
        require(szSlice > 0);
        require(bytes.Length % szSlice == 0);

        int len = bytes.Length;
        int count = 0;

        if (len > 0)
            unsafe
            {
                fixed (byte* ptrSrc = bytes)
                {
                    byte* ptrStart = ptrSrc;

                    do
                    {
                        byte* ptrEnd = ptrStart + szSlice - 1;

                        while (ptrStart <= ptrEnd)
                        {
                            byte tmp = *ptrStart;
                            *ptrStart++ = *ptrEnd;
                            *ptrEnd-- = tmp;
                        }

                        count += szSlice;
                        ptrStart = ptrSrc + count;
                    }
                    while (count < len);
                }
            }
    }
}

