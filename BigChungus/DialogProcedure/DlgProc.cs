using System;
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
    CancellationToken CancellationToken { get; }
}

public sealed class DlgProc<TViewModel> : IDlgProc, IDialogContext<TViewModel>
    where TViewModel : class
{
    private readonly TViewModel _viewModel;
    private readonly IDialogBehavior<TViewModel>[] _behaviors;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private nint? _dialogBoxHandle;

    public DlgProc(TViewModel viewModel, IDialogBehavior<TViewModel>[] behaviors)
    {
        _viewModel = viewModel;
        _behaviors = behaviors;
        _cancellationTokenSource = new();
    }

    TViewModel IDialogContext<TViewModel>.ViewModel => _viewModel;
    nint IDialogContext<TViewModel>.DialogBoxHandle => _dialogBoxHandle ?? throw new InvalidOperationException();
    CancellationToken IDialogContext<TViewModel>.CancellationToken => _cancellationTokenSource.Token;

    nint? IDlgProc.DlgProc(Message m)
    {
        if (m.msg is WM_INITDIALOG)
        {
            _dialogBoxHandle = m.hWnd;
            (_viewModel as INotifyPropertyChanged)?.PropertyChanged += OnPropertyChanged;
            DispatcherHelper.ProcessWmInitDialog(m);
        }
        if (_dialogBoxHandle is null) return null;

        if (m.msg is WM_DESTROY) _cancellationTokenSource.Cancel(); // WM_CLOSE is unreliable, so we allow WM_DESTROY handlers to clean up during this one message.
        if (DispatcherHelper.TryProcessWmInvoke(m)) return null;

        var returnValue = InvokeBehaviors(m);

        if (m.msg is WM_CLOSE && returnValue is null)
        {
            Win32.EndDialog(_dialogBoxHandle.Value, (int)DialogResult.Close).ThrowIfFalse();
        }

        if (m.msg is WM_DESTROY)
        {
            DispatcherHelper.ProcessWmDestroy(m);
            _cancellationTokenSource.Dispose();
            (_viewModel as INotifyPropertyChanged)?.PropertyChanged -= OnPropertyChanged;
            _dialogBoxHandle = null;
        }

        return returnValue;
    }

    private nint? InvokeBehaviors(Message message)
    {
        nint? returnValue = null;
        foreach (var behavior in _behaviors)
        {
            try
            {
                var behaviorResult = behavior.OnMessageReceived(message, this);
                if (behaviorResult is nint behaviorReturnValue)
                {
                    returnValue = returnValue is null
                        ? behaviorReturnValue
                        : throw new InvalidOperationException("Multiple behaviors returned a value for the same message.");
                }
            }
            catch (Exception e)
            {
                var viewModel = new MessageBoxViewModel()
                {
                    Caption = "Unhandled exception",
                    Text = $"{e.GetType()}: {e.Message}\r\n\r\n{e.StackTrace}",
                    Icon = MessageBoxIcon.Error
                };
                var view = new MessageBoxView();
                view.ShowDialog(viewModel, _dialogBoxHandle.GetValueOrDefault());
            }
        }
        return returnValue;
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
