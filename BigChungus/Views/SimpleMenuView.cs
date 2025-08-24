// This is a very basic implementation. Ideally:
// - There should be a separate menu item class for each item type (button, sub-menu, separators)
// - The menu should be displayed by a behavior, where it will be positioned according to WM_CONTEXTMENU parameters.
// But what we have here should help with Dialog Editor functionalities and other simple scenarios.
// The "normal" menu will probably have its own builder API and per-item bindings for "enabled", "visible", "checked", etc.

using System.Runtime.CompilerServices;

public sealed class SimpleMenuView : IDialogRunner<SimpleMenuViewModel>
{
    private const int StartingItemId = 2000;

    public DialogResult ShowDialog(SimpleMenuViewModel viewModel, nint parentHandle)
    {
        if (parentHandle == 0) throw new ArgumentException("Attempt to show a menu without an owner window.", nameof(parentHandle));
        
        var handle = Win32.CreatePopupMenu();
        for(int i = 0; i < viewModel.Items.Length; i++)
        {
            var item = viewModel.Items[i];
            Win32.AppendMenu(handle, 0, (nuint)(StartingItemId + i), item);
        }
        Win32.GetCursorPos(out var cursorPosition);
        var result = Win32.TrackPopupMenuEx(handle, TPM_NONOTIFY | TPM_RETURNCMD, cursorPosition.x, cursorPosition.y, parentHandle, in Unsafe.NullRef<Win32.TPMPARAMS>());
        Win32.DestroyMenu(handle);
        if(result is 0)
        {
            viewModel.SelectedIndex = -1;
            return DialogResult.Cancel;
        }
        viewModel.SelectedIndex = result - StartingItemId;
        return DialogResult.OK;
    }
}

public class SimpleMenuViewModel : DialogViewModelBase<SimpleMenuView, SimpleMenuViewModel>
{
    public string[] Items { get; set; } = Array.Empty<string>();
    public int SelectedIndex { get; set; } = -1;
}
