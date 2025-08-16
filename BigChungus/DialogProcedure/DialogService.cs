public readonly struct DialogService
{
    private readonly nint _handle;
    public DialogService(nint handle)
    {
        _handle = handle;
    }

    public void ShowDialog<TViewModel>(IDialogRunner<TViewModel> view, TViewModel viewModel)
        where TViewModel : class
    {
        view.ShowDialog(viewModel, _handle);
    }

    public void ShowDialog<TViewModel>(TViewModel viewModel)
        where TViewModel : class, IDialogRunner<TViewModel>
    {
        viewModel.ShowDialog(viewModel, _handle);
    }

    public void Close(DialogResult result)
    {
        Win32.EndDialog(_handle, (int)result);
    }

    public Task<T> InvokeAsync<T>(Func<T> callback)
    {
        throw new NotImplementedException();
    }

    public Task InvokeAsync(Action callback)
    {
        throw new NotImplementedException();
    }
}

public enum DialogResult
{
    OK = IDOK,
    Cancel = IDCANCEL,
    Abort = IDABORT,
    Retry = IDRETRY,
    Ignore = IDIGNORE,
    Yes = IDYES,
    No = IDNO,
    Close = IDCLOSE,
    Help = IDHELP,
    TryAgain = IDTRYAGAIN,
    Continue = IDCONTINUE
}
