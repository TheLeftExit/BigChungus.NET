public unsafe class WmNcHitTestHandler : IDialogBehavior<DialogEditorViewModel>
{
    // Disables resizing by top/left edges.
    nint? IDialogBehavior<DialogEditorViewModel>.OnMessageReceived(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        if (message.msg is not WM_NCHITTEST) return null;

        var result = (uint)Win32.DefWindowProc(message.hWnd, message.msg, message.wParam, message.lParam);
        return (nint)(result switch
        {
            HTBOTTOMLEFT => HTBOTTOM,
            HTTOPRIGHT => HTRIGHT,
            HTLEFT or HTTOPLEFT or HTTOP => HTCLIENT,
            _ => result
        });
    }
}
