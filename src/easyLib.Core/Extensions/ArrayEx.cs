using System.Runtime.CompilerServices;

namespace easyLib.Extensions;

public static class ArrayEx
{
    public static void ReverseSlice(this byte[] bytes, int szSlice, int sliceCount, int ndxStart = 0)
    {
        require(bytes != null);
        require(szSlice > 0);
        require(sliceCount >= 0);
        require(ndxStart >= 0);
        require(sliceCount * szSlice >= 0);
        require(sliceCount * szSlice <= bytes.Length - ndxStart);

        int n = 0;

        unsafe
        {
            fixed (byte* ptrSrc = bytes)
            {
                while (n < sliceCount)
                {
                    byte* ptrStart = ptrSrc + ndxStart + (szSlice * n);
                    byte* ptrEnd = ptrStart + szSlice - 1;

                    while (ptrStart < ptrEnd)
                    {
                        byte tmp = *ptrStart;
                        *ptrStart++ = *ptrEnd;
                        *ptrEnd-- = tmp;
                    }

                    ++n;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array)
    {
        require(array != null);

        return new ReadOnlySpan<T>(array);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array, int count, int ndxStart = 0)
    {
        require(array != null);
        require(count >= 0);
        require(ndxStart >= 0);
        require(count < array.Length - ndxStart);

        return new ReadOnlySpan<T>(array, ndxStart, count);
    }
}
