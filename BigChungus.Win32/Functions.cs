using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Win32;
using BOOL = bool;
using HRESULT = bool;

public static unsafe partial class Win32
{
    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-postquitmessage
    [LibraryImport("user32.dll")]
    public static partial void PostQuitMessage(
        int nExitCode
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-defwindowprocw
    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
    public static partial LRESULT DefWindowProc(
        HWND hWnd,
        UINT Msg,
        WPARAM wParam,
        LPARAM lParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmessagew
    [LibraryImport("user32.dll", EntryPoint = "GetMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL GetMessage(
        out MSG lpMsg,
        HWND hWnd,
        UINT wMsgFilterMin,
        UINT wMsgFilterMax
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-translatemessage
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL TranslateMessage(
        in MSG lpMsg
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-dispatchmessagew
    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    public static partial LRESULT DispatchMessage(
        in MSG lpMsg
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsyscolorbrush
    [LibraryImport("user32.dll")]
    public static partial HBRUSH GetSysColorBrush(
        int nIndex
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL ShowWindow(
        HWND hWnd,
        int nCmdShow
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-destroywindow
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL DestroyWindow(
        HWND hWnd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerclassexw
    [LibraryImport("user32.dll", EntryPoint = "RegisterClassExW")]
    public static partial ATOM RegisterClassEx(
        in WNDCLASSEX unnamedParam1
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createwindowexw
    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW")]
    public static partial nint CreateWindowEx(
        DWORD dwExStyle,
        LPCWSTR lpClassName,
        LPCWSTR lpWindowName,
        DWORD dwStyle,
        int X,
        int Y,
        int nWidth,
        int nHeight,
        HWND hWndParent,
        HMENU hMenu,
        HINSTANCE hInstance,
        LPVOID lpParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getparent
    [LibraryImport("user32.dll")]
    public static partial HWND GetParent(
        HWND hWnd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL GetWindowRect(
        HWND hWnd,
        out RECT lpRect
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-movewindow
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL MoveWindow(
        HWND hWnd,
        int X,
        int Y,
        int nWidth,
        int nHeight,
        [MarshalAs(UnmanagedType.Bool)] BOOL bRepaint
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowtextlengthw
    [LibraryImport("user32.dll", EntryPoint = "GetWindowTextLengthW")]
    public static partial int GetWindowTextLength(
        HWND hWnd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessagew
    [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
    public static partial LRESULT SendMessage(
        HWND hWnd,
        UINT msg,
        WPARAM wParam,
        LPARAM lParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-postmessagew
    [LibraryImport("user32.dll", EntryPoint = "PostMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL PostMessage(
        HWND hWnd,
        UINT msg,
        WPARAM wParam,
        LPARAM lParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enablewindow
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL EnableWindow(
        HWND hWnd,
        [MarshalAs(UnmanagedType.Bool)] BOOL bEnable
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-iswindowenabled
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL IsWindowEnabled(
        HWND hWnd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-iswindowvisible
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL IsWindowVisible(
        HWND hWnd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-systemparametersinfow
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL SystemParametersInfoForDpi(
        UINT uiAction,
        UINT uiParam,
        PVOID pvParam,
        UINT fWinIni,
        UINT dpi
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowthreadprocessid
    [LibraryImport("user32.dll")]
    public static partial uint GetWindowThreadProcessId(
        HWND hWnd,
        out DWORD lpdwProcessId
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-loadcursorw
    [LibraryImport("user32.dll", EntryPoint = "LoadCursorW")]
    public static partial HCURSOR LoadCursor(
        HINSTANCE hInstance,
        LPCWSTR lpCursorName
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindow
    [LibraryImport("user32.dll")]
    public static partial HWND GetWindow(
        HWND hWndParent,
        UINT uCmd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-iswindow
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL IsWindow(
        HWND hWnd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumthreadwindows
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL EnumThreadWindows(
        DWORD dwThreadId,
        WNDENUMPROC lpfn,
        LPARAM lParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumchildwindows
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL EnumChildWindows(
        HWND hWndParent,
        WNDENUMPROC lpEnumFunc,
        LPARAM lParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-findwindowexw
    [LibraryImport("user32.dll")]
    public static partial HWND FindWindowExW(
        HWND hWndParent,
        HWND hWndChildAfter,
        LPCWSTR lpszClass,
        LPCWSTR lpszWindow
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createactctxw
    [LibraryImport("kernel32.dll", EntryPoint = "CreateActCtxW")]
    public static partial HANDLE CreateActCtx(
        ref ACTCTX pActCtx
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-activateactctx
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL ActivateActCtx(
        HANDLE hActCtx,
        out ULONG_PTR lpCookie
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentthreadid
    [LibraryImport("kernel32.dll")]
    public static partial DWORD GetCurrentThreadId();

    // https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-createfontindirectw
    [LibraryImport("gdi32.dll", EntryPoint = "CreateFontIndirectW")]
    public static partial HFONT CreateFontIndirect(
        in LOGFONT lplf
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/commctrl/nf-commctrl-initcommoncontrolsex
    [LibraryImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL InitCommonControlsEx(
        in INITCOMMONCONTROLSEX picce
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setprocessdpiawarenesscontext
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL SetProcessDpiAwarenessContext(
        HANDLE value
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/commctrl/nf-commctrl-defsubclassproc
    [LibraryImport("comctl32.dll")]   
    public static partial LRESULT DefSubclassProc(
        HWND hWnd,
        UINT uMsg,
        WPARAM wParam,
        LPARAM lParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/commctrl/nf-commctrl-getwindowsubclass
    [LibraryImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL GetWindowSubclass(
        HWND hWnd,
        SUBCLASSPROC pfnSubclass,
        UINT_PTR uIdSubclass,
        out DWORD_PTR plResult
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/commctrl/nf-commctrl-removewindowsubclass
    [LibraryImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL RemoveWindowSubclass(
        HWND hWnd,
        SUBCLASSPROC pfnSubclass,
        UINT_PTR uIdSubclass
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/commctrl/nf-commctrl-setwindowsubclass
    [LibraryImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL SetWindowSubclass(
        HWND hWnd,
        SUBCLASSPROC pfnSubclass,
        UINT_PTR uIdSubclass,
        DWORD_PTR dwRefData
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/shlwapi/nc-shlwapi-dllgetversionproc
    [LibraryImport("comctl32.dll", EntryPoint = "DllGetVersion")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial HRESULT ComCtl32_DllGetVersion(ref DLLVERSIONINFO2 unnamedParam1);

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-peekmessagew
    [LibraryImport("user32.dll", EntryPoint = "PeekMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL PeekMessage(
        out MSG lpMsg,
        HWND hWnd,
        UINT wMsgFilterMin,
        UINT wMsgFilterMax,
        UINT wRemoveMsg
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-dialogboxindirectparamw
    [LibraryImport("user32.dll", EntryPoint = "DialogBoxIndirectParamW", SetLastError = true)]
    public static partial INT_PTR DialogBoxIndirectParam(
        HINSTANCE hInstance,
        LPCDLGTEMPLATE hDialogTemplate,
        HWND hWndParent,
        DLGPROC lpDialogFunc,
        LPARAM dwInitParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createdialogindirectparamw
    [LibraryImport("user32.dll", EntryPoint = "CreateDialogIndirectParamW")]
    public static partial HWND CreateDialogIndirectParam(
        HINSTANCE hInstance,
        LPCDLGTEMPLATE hDialogTemplate,
        HWND hWndParent,
        DLGPROC lpDialogFunc,
        LPARAM dwInitParam
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enddialog
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL EndDialog(
        HWND hDlg,
        INT_PTR nResult
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-muldiv
    [LibraryImport("kernel32.dll")]
    public static partial int MulDiv(
        int nNumber,
        int nNumerator,
        int nDenominator
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdlgitem
    [LibraryImport("user32.dll")]
    public static partial HWND GetDlgItem(
        HWND hDlg,
        int nIDDlgItem
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setfocus
    [LibraryImport("user32.dll")]
    public static partial HWND SetFocus(
        HWND hWnd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcapture
    [LibraryImport("user32.dll")]
    public static partial HWND SetCapture(
        HWND hWnd
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-screentoclient
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL ScreenToClient(
        HWND hWnd,
        ref POINT lpPoint
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL SetWindowPos(
        HWND hWnd,
        HWND hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        UINT uFlags
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-releasecapture
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial BOOL ReleaseCapture();
}
