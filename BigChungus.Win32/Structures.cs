using BOOL = int;

public static partial class Win32
{
    public const int TRUE = 255;

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msg"/>
    /// </summary>
    public struct MSG
    {
        public HWND hwnd;
        public UINT message;
        public WPARAM wParam;
        public LPARAM lParam;
        public DWORD time;
        public POINT pt;
        public DWORD lPrivate;
    }
    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-wndclassexw"/>
    /// </summary>
    public unsafe struct WNDCLASSEX
    {
        public UINT cbSize;
        public UINT style;
        public WNDPROC lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public HINSTANCE hInstance;
        public HICON hIcon;
        public HCURSOR hCursor;
        public HBRUSH hbrBackground;
        public LPCWSTR lpszMenuName;
        public LPCWSTR lpszClassName;
        public HICON hIconSm;
    }

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-nmhdr"/>
    /// </summary>
    public struct NMHDR
    {
        public HWND hwndFrom;
        public UINT_PTR idFrom;
        public UINT code;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-point
    public struct POINT
    {
        public LONG x;
        public LONG y;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect
    public struct RECT
    {
        public LONG left;
        public LONG top;
        public LONG right;
        public LONG bottom;

        public static implicit operator System.Drawing.Rectangle(RECT rect) => new(
            x: rect.left,
            y: rect.top,
            width: rect.right - rect.left,
            height: rect.bottom - rect.top
        );

        public static implicit operator RECT(System.Drawing.Rectangle rect) => new()
        {
            left = rect.X,
            top = rect.Y,
            right = rect.X + rect.Width,
            bottom = rect.Y + rect.Height,
        };
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winbase/ns-winbase-actctxw
    public unsafe struct ACTCTX
    {
        public ULONG cbSize;
        public DWORD dwFlags;
        public LPCWSTR lpSource;
        public USHORT wProcessorArchitecture;
        public LANGID wLangId;
        public LPCWSTR lpAssemblyDirectory;
        public LPCWSTR lpResourceName;
        public LPCWSTR lpApplicationName;
        public HMODULE hModule;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-logfontw
    public unsafe struct LOGFONT
    {
        public LONG lfHeight;
        public LONG lfWidth;
        public LONG lfEscapement;
        public LONG lfOrientation;
        public LONG lfWeight;
        public BYTE lfItalic;
        public BYTE lfUnderline;
        public BYTE lfStrikeOut;
        public BYTE lfCharSet;
        public BYTE lfOutPrecision;
        public BYTE lfClipPrecision;
        public BYTE lfQuality;
        public BYTE lfPitchAndFamily;
        public fixed WCHAR lfFaceName[32];
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/commctrl/ns-commctrl-initcommoncontrolsex
    public struct INITCOMMONCONTROLSEX
    {
        public DWORD dwSize;
        public DWORD dwICC;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-nonclientmetricsw
    public struct NONCLIENTMETRICS
    {
        public UINT cbSize;
        public int iBorderWidth;
        public int iScrollWidth;
        public int iScrollHeight;
        public int iCaptionWidth;
        public int iCaptionHeight;
        public LOGFONT lfCaptionFont;
        public int iSmCaptionWidth;
        public int iSmCaptionHeight;
        public LOGFONT lfSmCaptionFont;
        public int iMenuWidth;
        public int iMenuHeight;
        public LOGFONT lfMenuFont;
        public LOGFONT lfStatusFont;
        public LOGFONT lfMessageFont;
        public int iPaddedBorderWidth;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/shlwapi/ns-shlwapi-dllversioninfo2
    public struct DLLVERSIONINFO2
    {
        public DLLVERSIONINFO info1;
        public DWORD dwFlags;
        public ULONGLONG ullVersion;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/shlwapi/ns-shlwapi-dllversioninfo
    public struct DLLVERSIONINFO
    {
        public DWORD cbSize;
        public DWORD dwMajorVersion;
        public DWORD dwMinorVersion;
        public DWORD dwBuildNumber;
        public DWORD dwPlatformID;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-createstructw
    public unsafe struct CREATESTRUCT
    {
        public LPVOID lpCreateParams;
        public HINSTANCE hInstance;
        public HMENU hMenu;
        public HWND hwndParent;
        public int cy;
        public int cx;
        public int y;
        public int x;
        public LONG style;
        public LPCWSTR lpszName;
        public LPCWSTR lpszClass;
        public DWORD dwExStyle;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-dlgtemplate
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DLGTEMPLATE
    {
        public DWORD style;
        public DWORD dwExtendedStyle;
        public WORD cdit;
        public short x;
        public short y;
        public short cx;
        public short cy;
        // Menu (null-terminated), '\0' for us
        // Class (null-terminated)
        // Title (null-terminated)
        // Font size (ushort, if DS_SETFONT)
        // Font name (null-terminated, if DS_SETFONT)
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-dlgitemtemplate
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DLGITEMTEMPLATE
    {
        public DWORD style;
        public DWORD dwExtendedStyle;
        public short x;
        public short y;
        public short cx;
        public short cy;
        public WORD id;
        // Class (null-terminated)
        // Title (null-terminated)
        // lParam data (length-prefixed)
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-paintstruct
    public unsafe struct PAINTSTRUCT
    {
        public HDC hdc;
        public BOOL fErase;
        public RECT rcPaint;
        public BOOL fRestore;
        public BOOL fIncUpdate;
        public fixed BYTE rgbReserved[32];
    }
}
