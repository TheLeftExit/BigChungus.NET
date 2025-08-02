using System.Runtime.CompilerServices;

public static class ReturnValueExceptionExtensions
{
    public static T ThrowIf<T>(this T value, T check, [CallerArgumentExpression(nameof(value))] string valueAsString = null!) where T : unmanaged, IEquatable<T>
    {
        return !value.Equals(check) ? value : throw new ReturnValueException(valueAsString);
    }

    public static bool ThrowIfFalse(this bool value, [CallerArgumentExpression(nameof(value))] string valueAsString = null!)
    {
        return value ? value : throw new ReturnValueException(valueAsString);
    }
}

public sealed class ReturnValueException(string message) : Exception(message);
