public unsafe class WindowSizeBehavior : IDialogBehavior<DialogEditorViewModel>
{
    nint? IDialogBehavior<DialogEditorViewModel>.OnMessageReceived(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        if (message.msg is not WM_SIZING) return null;

        var rect = (Win32.RECT*)message.lParam;
        WindowSizeToClientSize(ref *rect, context.DialogBoxHandle);

        var variablePointX = message.wParam switch
        {
            WMSZ_LEFT or WMSZ_TOPLEFT or WMSZ_BOTTOMLEFT => VariablePoint.Start,
            WMSZ_RIGHT or WMSZ_TOPRIGHT or WMSZ_BOTTOMRIGHT => VariablePoint.End,
            _ => VariablePoint.None
        };
        var variablePointY = message.wParam switch
        {
            WMSZ_TOP or WMSZ_TOPLEFT or WMSZ_TOPRIGHT => VariablePoint.Start,
            WMSZ_BOTTOM or WMSZ_BOTTOMLEFT or WMSZ_BOTTOMRIGHT => VariablePoint.End,
            _ => VariablePoint.None
        };
        var (newWidth, newHeight) = context.ViewModel.Size;
        if (variablePointX is not VariablePoint.None)
        {
            (rect->left, rect->right) = context.ViewModel.RoundToDLUGrid(rect->left, rect->right, variablePointX, DLUDirection.X, out newWidth);
        }
        if (variablePointY is not VariablePoint.None)
        {
            (rect->top, rect->bottom) = context.ViewModel.RoundToDLUGrid(rect->top, rect->bottom, variablePointY, DLUDirection.Y, out newHeight);
        }
        context.ViewModel.Size = new(newWidth, newHeight);
        ClientSizeToWindowSize(ref *rect, context.DialogBoxHandle);
        return null;
    }

    private static (int Left, int Top, int Right, int Bottom) GetClientAreaPadding(nint dialogHandle)
    {
        Win32.GetWindowRect(dialogHandle, out var windowRect);
        Win32.GetClientRect(dialogHandle, out var clientRect);
        return (
            windowRect.left - clientRect.left,
            windowRect.top - clientRect.top,
            windowRect.right - clientRect.right,
            windowRect.bottom - clientRect.bottom
        );
    }

    private static void WindowSizeToClientSize(ref Win32.RECT rect, nint dialogHandle)
    {
        var (left, top, right, bottom) = GetClientAreaPadding(dialogHandle);
        rect.left -= left;
        rect.top -= top;
        rect.right -= right;
        rect.bottom -= bottom;
    }

    private static void ClientSizeToWindowSize(ref Win32.RECT rect, nint dialogHandle)
    {
        var (left, top, right, bottom) = GetClientAreaPadding(dialogHandle);
        rect.left += left;
        rect.top += top;
        rect.right += right;
        rect.bottom += bottom;
    }
}
