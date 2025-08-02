using System.Numerics;

public static class FlagHelper
{
    public static bool GetFlag<TValue>(TValue value, TValue flag) where TValue : IBinaryInteger<TValue>
    {
        return (value & flag) == flag;
    }
    public static TResult GetFlag<TValue, TResult>(TValue value, params ReadOnlySpan<(TResult Key, TValue Flag)> map) where TValue : IBinaryInteger<TValue>
    {
        foreach (var (key, flag) in map)
        {
            if (GetFlag(value, flag))
            {
                return key;
            }
        }
        throw new ArgumentException($"No matching flag found for value: {value}");
    }

    public static void SetFlag<TValue>(ref TValue value, TValue flag, bool set) where TValue : IBinaryInteger<TValue>
    {
        value = set ? (value | flag) : (value & ~flag);
    }

    public static void SetFlag<TValue, TResult>(ref TValue value, TResult newValue, params ReadOnlySpan<(TResult Key, TValue Flag)> map) where TValue : IBinaryInteger<TValue>
    {
        var matchFound = false;
        foreach (var (key, flag) in map)
        {
            var equals = EqualityComparer<TResult>.Default.Equals(newValue, key);
            SetFlag(ref value, flag, equals);
            matchFound |= equals;
        }
        if(!matchFound)
        {
            throw new ArgumentException($"No matching flag found for value: {newValue}");
        }
    }
}
