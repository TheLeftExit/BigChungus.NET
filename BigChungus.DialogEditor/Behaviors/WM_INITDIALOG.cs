using System.Drawing;

public class WmInitDialogHandler : IDialogBehavior<DialogEditorViewModel>
{
    nint? IDialogBehavior<DialogEditorViewModel>.OnMessageReceived(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        if (message.msg is not WM_INITDIALOG) return null;
        var viewModel = context.ViewModel;

        var style = (uint)Win32.GetWindowLongPtr(context.DialogBoxHandle, GWLx_STYLE);
        style |= WS_OVERLAPPEDWINDOW & ~WS_MAXIMIZEBOX;
        Win32.SetWindowLongPtr(context.DialogBoxHandle, GWLx_STYLE, (nint)style);

        viewModel.DLUGridBitmap = CreateDLUGridBitmap(context.DialogBoxHandle, viewModel.DLUGridSize);

        var dluHelper = new DLUHelper(context.DialogBoxHandle);
        Win32.GetClientRect(context.DialogBoxHandle, out var clientRect);
        var clientRectDLU = dluHelper.PixelsToDLU(clientRect);
        viewModel.Size = new SizeDLU((short)(clientRectDLU.right - clientRectDLU.left), (short)(clientRectDLU.bottom - clientRectDLU.top));

        return null;
    }
    
    private static (nint HBITMAP, nint HDC, Size Size) CreateDLUGridBitmap(nint dialogHandle, SizeDLU gridSize)
    {
        var hdc = 0;

        var dluHelper = new DLUHelper(dialogHandle);

        var maxX = dluHelper.DLUToPixelsHorz(DLUHelper.MaxDLU);
        var maxY = dluHelper.DLUToPixelsVert(DLUHelper.MaxDLU);
        var size = new Size(maxX, maxY);
        var rect = new Win32.RECT() { right = size.Width, bottom = size.Height };
        var memoryDC = Win32.CreateCompatibleDC(hdc);
        var bitmap = Win32.CreateCompatibleBitmap(memoryDC, rect.right, rect.bottom);
        Win32.SelectObject(memoryDC, bitmap);
        Win32.FillRect(memoryDC, rect, Win32.GetSysColorBrush(5));

        for (var x = 0; x < DLUHelper.MaxDLU; x += gridSize.Width)
        {
            for (var y = 0; y < DLUHelper.MaxDLU; y += gridSize.Height)
            {
                Win32.SetPixel(
                    memoryDC,
                    dluHelper.DLUToPixelsHorz(x),
                    dluHelper.DLUToPixelsVert(y),
                    0
                );
            }
        }

        return (bitmap, memoryDC, size);
    }
}
