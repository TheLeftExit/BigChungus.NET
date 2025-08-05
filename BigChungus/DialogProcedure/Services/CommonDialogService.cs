public interface ICommonDialogService
{
    MessageBoxResult ShowMessageBox(
        string text,
        string? caption = "",
        MessageBoxButtons buttons = MessageBoxButtons.Ok,
        MessageBoxIcon icon = MessageBoxIcon.None
    );
}

public class CommonDialogService : IDialogService<CommonDialogService>
{
    private readonly nint _dialogBoxHandle;
    public CommonDialogService(nint dialogBoxHandle)
    {
        _dialogBoxHandle = dialogBoxHandle;
    }
    public static CommonDialogService Create(nint dialogBoxHandle) => new(dialogBoxHandle);

    public unsafe MessageBoxResult ShowMessageBox(
        string text,
        string? caption = "",
        MessageBoxButtons buttons = MessageBoxButtons.Ok,
        MessageBoxIcon icon = MessageBoxIcon.None)
    {
        fixed(char* textPtr = text, captionPtr = caption)
        {
            return (MessageBoxResult)Win32.MessageBox(_dialogBoxHandle, textPtr, captionPtr, (uint)buttons | (uint)icon);
        }
    }
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

public enum MessageBoxResult : int
{
    Abort = IDABORT,
    Cancel = IDCANCEL,
    Continue = IDCONTINUE,
    Ignore = IDIGNORE,
    No = IDNO,
    Ok = IDOK,
    Retry = IDRETRY,
    Yes = IDYES
}
