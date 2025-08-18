public sealed class MessageBoxView : IDialogRunner<MessageBoxViewModel>
{
    public DialogResult ShowDialog(MessageBoxViewModel viewModel, nint parentHandle = 0)
    {
        return (DialogResult)Win32.MessageBox(parentHandle, viewModel.Text, viewModel.Caption, (uint)viewModel.Buttons | (uint)viewModel.Icon);
    }
}

public sealed class MessageBoxViewModel : DialogViewModelBase<MessageBoxView, MessageBoxViewModel>
{
    public string? Text { get; set; }
    public string Caption { get; set; } = string.Empty;
    public MessageBoxButtons Buttons { get; set; } = MessageBoxButtons.Ok;
    public MessageBoxIcon Icon { get; set; } = MessageBoxIcon.None;
}

public enum MessageBoxButtons : uint
{
    AbortRetryIgnore = MB_ABORTRETRYIGNORE,
    CancelTryContinue = MB_CANCELTRYCONTINUE,
    Ok = MB_OK,
    OkCancel = MB_OKCANCEL,
    RetryCancel = MB_RETRYCANCEL,
    YesNo = MB_YESNO,
    YesNoCancel = MB_YESNOCANCEL
}

public enum MessageBoxIcon : uint
{
    None = 0,
    Warning = MB_ICONWARNING,
    Information = MB_ICONINFORMATION,
    Question = MB_ICONQUESTION,
    Error = MB_ICONERROR
}
