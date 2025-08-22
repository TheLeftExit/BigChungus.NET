using System.Drawing;

public class ViewModelInitializerBehavior : IDialogBehavior<DialogEditorViewModel>
{
    nint? IDialogBehavior<DialogEditorViewModel>.OnMessageReceived(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        if (message.msg is not WM_INITDIALOG) return null;
        var viewModel = context.ViewModel;


        var style = (uint)Win32.GetWindowLongPtr(context.DialogBoxHandle, GWLx_STYLE);
        style |= WS_OVERLAPPEDWINDOW;
        Win32.SetWindowLongPtr(context.DialogBoxHandle, GWLx_STYLE, (nint)style);


        viewModel.DLUXPoints = CreateDLUPoints(context.DialogBoxHandle, DLUDirection.X, viewModel.MaxDialogSize.Width);
        viewModel.DLUYPoints = CreateDLUPoints(context.DialogBoxHandle, DLUDirection.Y, viewModel.MaxDialogSize.Height);

        viewModel.DLUGridBitmap = CreateDLUGridBitmap(context.DialogBoxHandle, viewModel.DLUXPoints, viewModel.DLUYPoints, viewModel.DLUGridSize);

        return null;
    }

    private static int[] CreateDLUPoints(nint dialogHandle, DLUDirection direction, int length)
    {
        var rect = new Win32.RECT();
        var points = new int[length];
        for (var x = 0; x < length; x++)
        {
            rect = new();
            if (direction == DLUDirection.X)
                rect.right = x;
            else
                rect.bottom = x;
            Win32.MapDialogRect(dialogHandle, ref rect);
            points[x] = direction == DLUDirection.X ? rect.right : rect.bottom;
        }
        return points;
    }

    

    private static (nint HBITMAP, nint HDC, Size Size) CreateDLUGridBitmap(nint dialogHandle, int[] dluXPoints, int[] dluYPoints, SizeDLU gridSize)
    {
        var hdc = 0;
        var size = new Size(dluXPoints.Last(), dluYPoints.Last());
        var rect = new Win32.RECT() { right = size.Width, bottom = size.Height };
        var memoryDC = Win32.CreateCompatibleDC(hdc);
        var bitmap = Win32.CreateCompatibleBitmap(memoryDC, rect.right, rect.bottom);
        Win32.SelectObject(memoryDC, bitmap);
        Win32.FillRect(memoryDC, rect, Win32.GetSysColorBrush(5));

        for (var x = 0; x < dluXPoints.Length; x += gridSize.Width)
        {
            for (var y = 0; y < dluYPoints.Length; y += gridSize.Height)
            {
                Win32.SetPixel(memoryDC, dluXPoints[x], dluYPoints[y], 0);
            }
        }

        return (bitmap, memoryDC, size);
    }
}
