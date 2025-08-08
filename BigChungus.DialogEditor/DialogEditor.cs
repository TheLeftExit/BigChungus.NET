using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Win32Macros;

internal static unsafe class DialogEditorHelper
{
    private static readonly nint HINSTANCE = NativeLibrary.GetMainProgramHandle();

    private static readonly Lazy<ushort> _classAtom = new(() =>
    {
        Win32.WNDCLASSEX classDefinition;
        fixed (char* className = "DialogEditor")
        {
            classDefinition = new Win32.WNDCLASSEX
            {
                cbSize = (uint)sizeof(Win32.WNDCLASSEX),
                hInstance = HINSTANCE,
                lpszClassName = className,
                style = CS_HREDRAW | CS_VREDRAW,
                lpfnWndProc = &WndProc,
                hCursor = Win32.LoadCursor(0, IDC_ARROW)
            };
        }
        return Win32.RegisterClassEx(classDefinition).ThrowIf<ushort>(0);
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly int[] _dluXPoints = CreateDLUPoints(2000, DLUDirection.X);
    private static readonly int[] _dluYPoints = CreateDLUPoints(2000, DLUDirection.Y);

    private const int _dluGridSize = 4;
    private static readonly int[] _dluXGridPoints = _dluXPoints.Where((_, index) => index % _dluGridSize == 0).ToArray();
    private static readonly int[] _dluYGridPoints = _dluYPoints.Where((_, index) => index % _dluGridSize == 0).ToArray();

    private static readonly Lazy<(nint HBITMAP, nint HDC, Size Size)> _gridBitmap = new(() =>
    {
        var hdc = 0;
        var size = new Size(_dluXPoints.Last(), _dluYPoints.Last());
        var rect = new Win32.RECT() { right = size.Width, bottom = size.Height };
        var memoryDC = Win32.CreateCompatibleDC(hdc);
        var bitmap = Win32.CreateCompatibleBitmap(memoryDC, rect.right, rect.bottom);
        Win32.SelectObject(memoryDC, bitmap);
        Win32.FillRect(memoryDC, rect, Win32.GetSysColorBrush(5));

        for (var x = 0; x < _dluXPoints.Length; x += _dluGridSize)
        {
            for (var y = 0; y < _dluYPoints.Length; y += _dluGridSize)
            {
                Win32.SetPixel(memoryDC, _dluXPoints[x], _dluYPoints[y], 0);
            }
        }
        

        return (bitmap, memoryDC, size);
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    private static Lazy<(int Left, int Top, int Right, int Bottom)> _clientAreaPadding = new(() =>
    {
        var dialogHandle = DialogBoxHelper.CreateDialog(new DlgTemplate()
        {
            Style = _windowStyle,
            ExStyle = _windowExStyle
        }.ToMemory().Span);
        Win32.GetWindowRect(dialogHandle, out var windowRect);
        Win32.GetClientRect(dialogHandle, out var clientRect);
        Win32.DestroyWindow(dialogHandle);
        return (
            windowRect.left - clientRect.left,
            windowRect.top - clientRect.top,
            windowRect.right - clientRect.right,
            windowRect.bottom - clientRect.bottom
        );
    }, LazyThreadSafetyMode.ExecutionAndPublication);
    private static void ClientSizeToWindowSize(ref Win32.RECT rect)
    {
        rect.left += _clientAreaPadding.Value.Left;
        rect.top += _clientAreaPadding.Value.Top;
        rect.right += _clientAreaPadding.Value.Right;
        rect.bottom += _clientAreaPadding.Value.Bottom;
    }
    private static void WindowSizeToClientSize(ref Win32.RECT rect)
    {
        rect.left -= _clientAreaPadding.Value.Left;
        rect.top -= _clientAreaPadding.Value.Top;
        rect.right -= _clientAreaPadding.Value.Right;
        rect.bottom -= _clientAreaPadding.Value.Bottom;
    }

    private const uint _windowExStyle = WS_EX_OVERLAPPEDWINDOW;
    private const uint _windowStyle = WS_OVERLAPPEDWINDOW & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX;

    private static SizeDLU _size;

    public static void Show(SizeDLU size)
    {
        var windowSize = new Win32.RECT
        {
            left = 0,
            top = 0,
            right = _dluXPoints[size.Width],
            bottom = _dluYPoints[size.Height]
        };
        Win32.AdjustWindowRectEx(ref windowSize, _windowStyle, false, _windowExStyle);
        nint handle;
        fixed (char* windowText = "Dialog Editor")
        {
            handle = Win32.CreateWindowEx(
                _windowExStyle,
                (char*)_classAtom.Value,
                windowText,
                _windowStyle,
                int.MinValue,
                int.MinValue,
                windowSize.right - windowSize.left,
                windowSize.bottom - windowSize.top,
                0,
                0,
                HINSTANCE,
                null
            );
        }
        Win32.ShowWindow(handle, SW_SHOW);

        Win32.MSG msg;
        while (Win32.GetMessage(out msg, 0, 0, 0))
        {
            Win32.TranslateMessage(msg);
            Win32.DispatchMessage(msg);
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static nint WndProc(nint hWnd, uint msg, nuint wParam, nint lParam)
    {
        if (msg is WM_ERASEBKGND)
        {
            var hdc = (nint)wParam;
            Win32.BitBlt(hdc, 0, 0, _gridBitmap.Value.Size.Width, _gridBitmap.Value.Size.Height, _gridBitmap.Value.HDC, 0, 0, 0x00CC0020);
            return 1;
        }
        if (msg is WM_SIZING)
        {
            EnforceDialogGrid(wParam, lParam);
        }
        if (msg is WM_CLOSE)
        {
            Win32.PostQuitMessage(0);
        }
        return Win32.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private static void EnforceDialogGrid(nuint wParam, nint lParam)
    {
        var rect = (Win32.RECT*)lParam;
        WindowSizeToClientSize(ref *rect);

        var variablePointX = wParam switch
        {
            WMSZ_LEFT or WMSZ_TOPLEFT or WMSZ_BOTTOMLEFT => VariablePoint.Start,
            WMSZ_RIGHT or WMSZ_TOPRIGHT or WMSZ_BOTTOMRIGHT => VariablePoint.End,
            _ => VariablePoint.None
        };
        var variablePointY = wParam switch
        {
            WMSZ_TOP or WMSZ_TOPLEFT or WMSZ_TOPRIGHT => VariablePoint.Start,
            WMSZ_BOTTOM or WMSZ_BOTTOMLEFT or WMSZ_BOTTOMRIGHT => VariablePoint.End,
            _ => VariablePoint.None
        };
        var (newWidth, newHeight) = _size;
        if (variablePointX is not VariablePoint.None)
        {
            (rect->left, rect->right) = RoundToDLUGrid(rect->left, rect->right, variablePointX, DLUDirection.X, out newWidth);
        }
        if (variablePointY is not VariablePoint.None)
        {
            (rect->top, rect->bottom) = RoundToDLUGrid(rect->top, rect->bottom, variablePointY, DLUDirection.Y, out newHeight);
        }
        _size = new(newWidth, newHeight);
        ClientSizeToWindowSize(ref *rect);
    }

    private static (int, int) RoundToDLUGrid(int start, int end, VariablePoint variablePoint, DLUDirection direction, out short lengthIndex)
    {
        var points = direction == DLUDirection.X ? _dluXPoints : _dluYPoints;
        var index = Array.BinarySearch(points, end - start);
        if (index < 0) index = ~index;
        index += (_dluGridSize - (index % _dluGridSize)) % _dluGridSize;
        lengthIndex = (short)index;
        return variablePoint switch
        {
            VariablePoint.Start => (end - points[index], end),
            VariablePoint.End => (start, start + points[index]),
            _ => throw new ArgumentException(nameof(variablePoint))
        };
    }

    private static int[] CreateDLUPoints(int count, DLUDirection direction)
    {
        var handle = DialogBoxHelper.CreateDialog(new DlgTemplate().ToMemory().Span);
        var rect = new Win32.RECT();
        var points = new int[count];
        for (var x = 0; x < count; x++)
        {
            rect = new();
            if (direction == DLUDirection.X)
                rect.right = x;
            else
                rect.bottom = x;
            Win32.MapDialogRect(handle, ref rect);
            points[x] = direction == DLUDirection.X ? rect.right : rect.bottom;
        }
        Win32.DestroyWindow(handle);
        return points;
    }

    private enum VariablePoint { None, Start, End }
    private enum DLUDirection { X, Y }
}

public static unsafe class DialogEditor
{
    public static void Show(SizeDLU size)
    {
        DialogEditorHelper.Show(size);
    }
}

/*
Control interactions (WinForms designer works well, we'll copy it):
- Selection
    - Drag-drop edges to resize
    - Drag-drop body to move
    - Arrows/WASD to move by 1 DLU
    - Tab to move to the next control
    - Copy/cut/paste/delete keyboard shortcuts
    - PgUp/PgDown/Home/End to move in the Z-order
- Resize on edges

Window Menu:
- Add an item (sub-menu)
- Open item list for add/delete/reorder
- Preview
- Import a template
- Export a template

Control Menu:
- Properties (default for double-click)
- Copy
- Cut
- Paste
- Delete

Dialogs:
- Select dialog item type: ComboBox, OK, Cancel
- Configure dialog item properties: ListView, OK, Cancel
- View/add/remove/reorder dialog items: ListView, OK, Cancel
- View final dialog code: TextEdit, OK
*/
