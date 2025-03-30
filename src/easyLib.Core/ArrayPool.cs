using System.Runtime.CompilerServices;

namespace easyLib;

public static class ArrayPool
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Rent<T>(int len)
    {
        require(len >= 0);
        require(len <= Array.MaxLength);

        return System.Buffers.ArrayPool<T>.Shared.Rent(len);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return<T>(T[] array)
    {
        if (array != null)
            System.Buffers.ArrayPool<T>.Shared.Return(array);
    }
}
