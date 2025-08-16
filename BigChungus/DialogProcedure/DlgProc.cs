using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public interface IDialogContext<TViewModel>
    where TViewModel : class
{
    TViewModel ViewModel { get; }
    nint DialogBoxHandle { get; }
}

public sealed class DlgProc<TViewModel> : IDlgProc, IDialogContext<TViewModel>
    where TViewModel : class
{
    private readonly TViewModel _viewModel;
    private readonly IDialogBehavior<TViewModel>[] _behaviors;

    private nint? _dialogBoxHandle;

    public DlgProc(TViewModel viewModel, IDialogBehavior<TViewModel>[] behaviors)
    {
        _viewModel = viewModel;
        _behaviors = behaviors;
    }

    TViewModel IDialogContext<TViewModel>.ViewModel => _viewModel;
    nint IDialogContext<TViewModel>.DialogBoxHandle => _dialogBoxHandle ?? throw new InvalidOperationException();

    nint? IDlgProc.DlgProc(Message m)
    {
        if (m.msg is WM_INITDIALOG)
        {
            _dialogBoxHandle = m.hWnd;
            (_viewModel as INotifyPropertyChanged)?.PropertyChanged += OnPropertyChanged;
            DialogTracker.Add(_viewModel, _dialogBoxHandle.Value);
        }

        if (_dialogBoxHandle is null)
        {
            return null;
        }

        nint? returnValue = null;
        foreach (var behavior in _behaviors)
        {
            var behaviorResult = behavior.OnMessageReceived(m, this);
            if (behaviorResult is nint behaviorReturnValue)
            {
                returnValue = returnValue is null
                    ? behaviorReturnValue
                    : throw new InvalidOperationException("Multiple behaviors returned a value for the same message.");
            }
        }

        if (m.msg is WM_CLOSE)
        {
            Win32.EndDialog(_dialogBoxHandle.Value, 0).ThrowIfFalse();
        }

        if (m.msg is WM_DESTROY)
        {
            DialogTracker.Remove(_viewModel);
            (_viewModel as INotifyPropertyChanged)?.PropertyChanged -= OnPropertyChanged;
            _dialogBoxHandle = null;
        }

        return null;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var dialogBoxHandle = _dialogBoxHandle ?? throw new UnreachableException();

        var gcHandle = GCHandle.Alloc(e);
        Win32.SendMessage(dialogBoxHandle, WM_VIEWMODEL_PROPERTYCHANGED, 0, GCHandle.ToIntPtr(gcHandle)); 
        gcHandle.Free();
    }
}

public static class PropertyChangedEventArgsExtensions
{
    extension(PropertyChangedEventArgs)
    {
        public static bool Parse(Message message, [NotNullWhen(true)] out PropertyChangedEventArgs? e)
        {
            if(message.msg is not WM_VIEWMODEL_PROPERTYCHANGED)
            {
                e = null;
                return false;
            }
            if(GCHandle.FromIntPtr(message.lParam).Target is not PropertyChangedEventArgs args)
            {
                throw new UnreachableException();
            }
            e = args;
            return true;
        }
    }
}

public static class DialogTracker
{
    private static readonly ConcurrentDictionary<object, nint> _dialogHandlesByViewModel = new();

    public static void Add(object viewModel, nint dialogHandle)
    {
        if (!_dialogHandlesByViewModel.TryAdd(viewModel, dialogHandle))
        {
            throw new InvalidOperationException();
        }
    }

    public static void Remove(object viewModel)
    {
        if (!_dialogHandlesByViewModel.TryRemove(viewModel, out _))
        {
            throw new InvalidOperationException();
        }
    }

    public static void Get(object viewModel)
    {
        if (!_dialogHandlesByViewModel.TryGetValue(viewModel, out _))
        {
            throw new InvalidOperationException();
        }
    }
}
