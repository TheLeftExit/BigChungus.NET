using System.Drawing;

public class DialogEditorView : DialogViewBase<DialogEditorViewModel>
{
    protected override void Configure(DialogBuilder<DialogEditorViewModel> builder)
    {
        builder.Properties.Text = "Dialog Editor (right-click for options)";
        builder.Properties.Size = new(200, 200);

        builder.AddBehavior(new WmInitDialogHandler());
        builder.AddBehavior(new WmSizingHandler());
        builder.AddBehavior(new WmEraseBkgndHandler());
        builder.AddBehavior(new WmNcHitTestHandler());
        builder.AddBehavior(new WmRButtonUpHandler());
        builder.AddBehavior(new WmReflectHandler());
    }
}

public class DialogEditorViewModel : DialogViewModelBase<DialogEditorView, DialogEditorViewModel>
{
    public SizeDLU Size { get; set; }
    public SizeDLU DLUGridSize { get; set; } = new(4, 4);
    public SizeDLU SizingMargin { get; set; } = new(4, 4);

    // === Internal stuff (will probably be moved to a separate "internal view model") ===

    public SizeDLU MaxDialogSize { get; } = new(2000, 2000);
    public Win32.POINT SizingLoopStartingCursorPositionRelativeToControl { get; set; }
    public SizeDLU SizingLoopStartingSize { get; set; }
    public (nint HBITMAP, nint HDC, Size Size) DLUGridBitmap { get; set; }
}
