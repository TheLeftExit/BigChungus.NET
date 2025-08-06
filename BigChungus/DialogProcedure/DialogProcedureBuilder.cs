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

        nint? returnValue = null;
        foreach (var behavior in _behaviors)
        {
            var behaviorResult = behavior.OnMessageReceived(m, m.hWnd, _viewModel);
            if (behaviorResult is nint behaviorReturnValue)
            {
                returnValue = returnValue is null
                    ? behaviorReturnValue
                    : throw new InvalidOperationException("Multiple behaviors returned a value for the same message.");
            }
        }

        if (m.msg is WM_CLOSE)
        {
            DialogBoxHelper.EndDialog(m.hWnd);
        }

        if (m.msg is WM_DESTROY)
        {
            (_viewModel as INotifyPropertyChanged)?.PropertyChanged -= OnPropertyChanged;
            _dialogBoxHandle = 0;
        }

        return null;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var dialogBoxHandle = _dialogBoxHandle ?? throw new UnreachableException();

        if (Win32.GetCurrentThreadId() != Win32.GetWindowThreadProcessId(dialogBoxHandle, out _))
        {
            throw new InvalidOperationException("Cross-thread operation detected.");
        }

        foreach (var behavior in _behaviors)
        {
            behavior.OnPropertyChanged(e.PropertyName, dialogBoxHandle, _viewModel);
        }
    }
}
