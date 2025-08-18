using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> eventArgsCache = new();

    protected void RaisePropertyChanged(string? propertyName = null)
    {
        var args = eventArgsCache.GetOrAdd(propertyName ?? "$null", x => new PropertyChangedEventArgs(x));
        PropertyChanged?.Invoke(this, args);
    }
    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if(EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        RaisePropertyChanged(propertyName);
    }
}

public class DialogViewModelBase<TView, TSelf> : BindableBase, IDialogRunner<TSelf>
    where TView : IDialogRunner<TSelf>, new()
    where TSelf : DialogViewModelBase<TView, TSelf>
{
    // For use in setups with `where TViewModel : IDialogRunner<TViewModel>` (to avoid having to specify TView as the generic type)
    DialogResult IDialogRunner<TSelf>.ShowDialog(TSelf viewModel, nint parentHandle)
    {
        var view = new TView();
        return view.ShowDialog(viewModel, parentHandle);
    }
}
