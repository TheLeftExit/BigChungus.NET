using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class BindableViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> eventArgsCache = new();

    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
        where T : IEquatable<T>
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        if (field is null && value is null || (field?.Equals(value) ?? false)) return;

        field = value;
        var args = eventArgsCache.GetOrAdd(propertyName, new PropertyChangedEventArgs(propertyName));
        PropertyChanged?.Invoke(this, args);
    }
}


