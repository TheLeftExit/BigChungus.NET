public static partial class Win32Macros
{
    public const int GWLx_STYLE = (-16);
    public const int GWLx_EXSTYLE = (-20);
    public const int GWLx_WNDPROC = (-4);
    public const int GWLx_HINSTANCE = (-6);
    public const int GWLx_HWNDPARENT = (-8);
    public const int GWLx_USERDATA = (-21);
    public const int GWLx_ID = (-12);

    public static unsafe readonly int DWLx_MSGRESULT = 0;
    public static unsafe readonly int DWLx_DLGPROC = DWLx_MSGRESULT + sizeof(LRESULT);
    public static unsafe readonly int DWLx_USER = DWLx_DLGPROC + sizeof(DLGPROC);
}

public static unsafe partial class Win32
{
    public static LONG_PTR GetWindowLongPtr(HWND hWnd, int nIndex)
    {
        if (nint.Size == 8)
        {
           return GetWindowLongPtrW(hWnd, nIndex);
        }
        else
        {
            return GetWindowLongW(hWnd, nIndex);
        }
    }

    public static LONG_PTR SetWindowLongPtr(HWND hWnd, int nIndex, LONG_PTR dwNewLong)
    {
        if (nint.Size == 8)
        {
            return SetWindowLongPtrW(hWnd, nIndex, dwNewLong);
        }
        else
        {
            return SetWindowLongW(hWnd, nIndex, (LONG)dwNewLong);
        }
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptrw
    [LibraryImport("user32.dll")]
    private static partial LONG_PTR GetWindowLongPtrW(
        HWND hWnd,
        int nIndex
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongw
    [LibraryImport("user32.dll")]
    private static partial LONG GetWindowLongW(
        HWND hWnd,
        int nIndex
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongptrw
    [LibraryImport("user32.dll")]
    private static partial LONG_PTR SetWindowLongPtrW(
        HWND hWnd,
        int nIndex,
        LONG_PTR dwNewLong
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongw
    [LibraryImport("user32.dll")]
    private static partial LONG SetWindowLongW(
        HWND hWnd,
        int nIndex,
        LONG dwNewLong
    );
}
