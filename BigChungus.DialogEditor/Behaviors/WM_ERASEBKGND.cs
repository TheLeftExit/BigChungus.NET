public class WmEraseBkgndHandler : IDialogBehavior<DialogEditorViewModel>
{
    nint? IDialogBehavior<DialogEditorViewModel>.OnMessageReceived(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        if (message.msg is not WM_ERASEBKGND) return null;

        var hdc = (nint)message.wParam;
        var bitmap = context.ViewModel.DLUGridBitmap;
        Win32.BitBlt(hdc, 0, 0, bitmap.Size.Width, bitmap.Size.Height, bitmap.HDC, 0, 0, 0x00CC0020);
        return 1;
    }
}
