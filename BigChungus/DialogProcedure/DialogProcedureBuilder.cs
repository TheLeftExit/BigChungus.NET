using System.ComponentModel;
using System.Diagnostics;

public interface IDialogProcedureBuilder<TViewModel>
    where TViewModel : class
{
    void AddBehavior(IDialogBehavior<TViewModel> behavior);
}

public static partial class DialogProcedureBuilderExtensions;

public interface IDialogBehavior<TViewModel>
    where TViewModel : class
{
    nint? OnMessageReceived(Message message, nint dialogBoxHandle, TViewModel viewModel);
    void OnPropertyChanged(string? propertyName, nint dialogBoxHandle, TViewModel viewModel);
}

public class DialogProcedureBuilder<TViewModel> : IDialogProcedureBuilder<TViewModel>
    where TViewModel : class
{
    private readonly List<IDialogBehavior<TViewModel>> _behaviors = new();

    public void AddBehavior(IDialogBehavior<TViewModel> behavior)
    {
        ArgumentNullException.ThrowIfNull(behavior);
        _behaviors.Add(behavior);
    }

    public IDlgProc Build(TViewModel viewModel)
    {
        return new DialogView<TViewModel>(viewModel, _behaviors.ToArray());
    }
}

public sealed class DialogView<TViewModel> : IDlgProc
    where TViewModel : class
{
    private readonly TViewModel _viewModel;
    private readonly IDialogBehavior<TViewModel>[] _behaviors;

    private nint? _dialogBoxHandle;

    public DialogView(TViewModel viewModel, IDialogBehavior<TViewModel>[] bindingPrototypes)
    {
        _viewModel = viewModel;
        _behaviors = bindingPrototypes;
    }

    public nint? DlgProc(Message m)
    {
        if (m.msg is WM_INITDIALOG)
        {
            _dialogBoxHandle = m.hWnd;
            (_viewModel as INotifyPropertyChanged)?.PropertyChanged += OnPropertyChanged;
        }

        if (_dialogBoxHandle is 0)
        {
            return null;
        }

        foreach (var behavior in _behaviors)
        {
            behavior.OnMessageReceived(m, m.hWnd, _viewModel);
        }

        if (m.msg is WM_CLOSE)
        {
            DialogBoxHelper.EndDialog(m.hWnd);
        }

        if (m.msg is WM_DESTROY)
        {
            _dialogBoxHandle = 0;
            (_viewModel as INotifyPropertyChanged)?.PropertyChanged -= OnPropertyChanged;
        }

        return null;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        foreach (var behavior in _behaviors)
        {
            behavior.OnPropertyChanged(e.PropertyName, _dialogBoxHandle ?? throw new UnreachableException(), _viewModel);
        }
    }
}
