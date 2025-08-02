using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static partial class DialogBoxHelper
{
    public static void EndDialog(nint dialogHandle)
    {
        Win32.EndDialog(dialogHandle, 0).ThrowIfFalse();
    }

    /// <summary>
    /// Subclasses the specified control so that it reflects its own <see cref="WM_KILLFOCUS"/> messages to its parent.
    /// The reflected message is sent as <see cref="WM_KILLFOCUS_REFLECT"/> and stores the reflecting control handle in the lParam.
    /// </summary>
    /// <param name="controlHandle"></param>
    public static unsafe void SubclassToReflectOwnKillFocusToParent(nint controlHandle)
    {
        Win32.SetWindowSubclass(controlHandle, &SubclassProc, WM_KILLFOCUS_REFLECT, 0).ThrowIfFalse();

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static nint SubclassProc(nint hWnd, uint msg, nuint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
        {
            if (msg == WM_KILLFOCUS)
            {
                var parentHandle = Win32.GetParent(hWnd); // TODO: Use GetAncestor
                if (parentHandle != 0)
                {
                    Win32.SendMessage(parentHandle, WM_KILLFOCUS_REFLECT, wParam, hWnd);
                }
            }
            return Win32.DefSubclassProc(hWnd, msg, wParam, lParam);
        }
    }


    private static unsafe Lazy<(short FontSize, string FontName)> defaultFont = new(() =>
    {
        var metrics = new Win32.NONCLIENTMETRICS { cbSize = (uint)sizeof(Win32.NONCLIENTMETRICS) };
        Win32.SystemParametersInfoForDpi(
            SPI_GETNONCLIENTMETRICS,
            (uint)sizeof(Win32.NONCLIENTMETRICS),
            &metrics,
            default,
            96
        ).ThrowIfFalse();
        var fontSize = (short)-Win32.MulDiv(metrics.lfMessageFont.lfHeight, 72, 96);
        var fontName = new string(metrics.lfMessageFont.lfFaceName);
        return (fontSize, fontName);
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    public static short DefaultFontSize => defaultFont.Value.FontSize;
    public static string DefaultFontName => defaultFont.Value.FontName;
}
