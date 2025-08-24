// No local WM_APP definitions - all in one place to avoid conflicts.

public static partial class Win32Macros
{
    // WM_VIEWMODEL_PROPERTYCHANGED - sent by the view to itself in response to view model's PropertyChanged.
    // Reduces the number of methods required by IDialogBehavior from 2 to 1.
    // wParam: not used
    // lParam: GCHandle to PropertyChangedEventArgs
    // Return value: not used
    public const UINT WM_VIEWMODEL_PROPERTYCHANGED = WM_APP;

    // WM_KILLFOCUS_REFLECT - sent by a subclassed control to its parent view when the control receives WM_KILLFOCUS.
    // Allows to implement "OnLostFocus" property bindings.
    // wParam: not used
    // lParam: handle of the control that received WM_KILLFOCUS
    // Return value: not used
    public const UINT WM_KILLFOCUS_REFLECT = WM_APP + 1;

    // WM_INVOKE - sent to marshal code to the UI thread.
    // wParam: GCHandle to SendOrPostCallback
    // lParam: GCHandle to state
    // Return value: not used
    public const UINT WM_INVOKE = WM_APP + 2;

    // WM_REFLECT - sent by a subclassed control to its parent view when the control receives any message.
    // wParam: not used
    // lParam: Pointer to `Message` that encodes the original message.
    // Return value: return value for the original message (custom or DefSubclassProc)
    public const UINT WM_REFLECT = WM_APP + 3;
}

