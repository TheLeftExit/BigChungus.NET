using System.Numerics;

public static class StyleHelper
{
    public static bool GetStyle<TValue>(TValue value, TValue styleMask, TValue style) where TValue : IBinaryInteger<TValue>
    {
        return (value & styleMask) == style;
    }
    public static void SetStyle<TValue>(ref TValue value, TValue styleMask, TValue style) where TValue : IBinaryInteger<TValue>
    {
        value = (value & ~styleMask) | style;
    }

    public static bool GetFlag<TValue>(TValue value, TValue flag) where TValue : IBinaryInteger<TValue>
    {
        return GetStyle(value, flag, flag);
    }
    public static void SetFlag<TValue>(ref TValue value, TValue flag, bool set) where TValue : IBinaryInteger<TValue>
    {
        SetStyle(ref value, flag, set ? flag : TValue.Zero);
    }

    public static TResult GetStyle<TValue, TResult>(TValue value, TValue styleMask, params ReadOnlySpan<(TResult Key, TValue Style)> map) where TValue : IBinaryInteger<TValue>
    {
        foreach (var (key, style) in map)
        {
            if (GetStyle(value, styleMask, style))
            {
                return key;
            }
        }
        throw new ArgumentException($"No matching style found for value: {value}");
    }
    public static void SetStyle<TValue, TResult>(ref TValue value, TValue styleMask, TResult newValue, params ReadOnlySpan<(TResult Key, TValue Style)> map) where TValue : IBinaryInteger<TValue>
    {
        foreach (var (key, style) in map)
        {
            var equals = EqualityComparer<TResult>.Default.Equals(newValue, key);
            if (equals)
            {
                SetStyle(ref value, styleMask, style);
                return;
            }
        }
        SetStyle(ref value, styleMask, TValue.Zero);
    }

    private static TValue GetMask<TValue, TResult>(params ReadOnlySpan<(TResult Key, TValue Flag)> map) where TValue : IBinaryInteger<TValue>
    {
        var mask = TValue.Zero;
        foreach (var (_, flag) in map)
        {
            mask |= flag;
        }
        return mask;
    }

    public static TResult GetFlag<TValue, TResult>(TValue value, params ReadOnlySpan<(TResult Key, TValue Flag)> map) where TValue : IBinaryInteger<TValue>
    {
        var mask = GetMask(map);
        return GetStyle(value, mask, map);
    }
    public static void SetFlag<TValue, TResult>(ref TValue value, TResult newValue, params ReadOnlySpan<(TResult Key, TValue Flag)> map) where TValue : IBinaryInteger<TValue>
    {
        var mask = GetMask(map);
        SetStyle(ref value, mask, newValue, map);
    }
}
