public unsafe class WmSizingHandler : IDialogBehavior<DialogEditorViewModel>
{
    // Ensures that the window is resized in `DLUGridSize` increments.
    // Requires a `WM_NCHITTEST` behavior to ensure that the window is only resized by bottom/right edges.
    nint? IDialogBehavior<DialogEditorViewModel>.OnMessageReceived(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        if (message.msg is not WM_SIZING) return null;

        var rect = (Win32.RECT*)message.lParam;
        WindowSizeToClientSize(ref *rect, context.DialogBoxHandle);

        var width = rect->right - rect->left;
        var height = rect->bottom - rect->top;
        var gridSize = context.ViewModel.DLUGridSize;
        var dluHelper = new DLUHelper(context.DialogBoxHandle);

        var newWidthDluDouble = dluHelper.PixelsToDLUHorz(width);
        var newWidthDlu = (int)Math.Round(newWidthDluDouble / gridSize.Width) * gridSize.Width;
        var newWidth = dluHelper.DLUToPixelsHorz(newWidthDlu);

        var newHeightDluDouble = dluHelper.PixelsToDLUVert(height);
        var newHeightDlu = (int)Math.Round(newHeightDluDouble / gridSize.Height) * gridSize.Height;
        var newHeight = dluHelper.DLUToPixelsVert(newHeightDlu);

        rect->right = rect->left + newWidth;
        rect->bottom = rect->top + newHeight;

        ClientSizeToWindowSize(ref *rect, context.DialogBoxHandle);
        return null;
    }

    // This remains here, because only WM_SIZING presents a case where I can't call GetClientRect to get the client area associated with a window rectangle.
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
