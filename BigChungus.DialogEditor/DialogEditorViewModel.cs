using System.Drawing;

public class DialogEditorView : DialogViewBase<DialogEditorViewModel>
{
    protected override void Configure(DialogBuilder<DialogEditorViewModel> builder)
    {
        builder.Properties.Size = new(200, 200);

        builder.AddBehavior(new ViewModelInitializerBehavior());
        builder.AddBehavior(new WindowSizeBehavior());
        builder.AddBehavior(new WindowBackgroundBehavior());
    }
}

public class DialogEditorViewModel : DialogViewModelBase<DialogEditorView, DialogEditorViewModel>
{
    public SizeDLU Size { get; set; }

    // === Internal stuff (will probably be moved to a separate "internal view model") ===

    public SizeDLU MaxDialogSize { get; } = new(2000, 2000);
    public int[] DLUXPoints { get; set; } = null!;
    public int[] DLUYPoints { get; set; } = null!;

    public SizeDLU DLUGridSize { get; } = new(4, 4);
    public int[] DLUXGridPoints => field ??= DLUXPoints.Where((_, index) => index % DLUGridSize.Width == 0).ToArray();
    public int[] DLUYGridPoints => field ??= DLUYPoints.Where((_, index) => index % DLUGridSize.Height == 0).ToArray();
    public (nint HBITMAP, nint HDC, Size Size) DLUGridBitmap { get; set; }

    public (int Start, int End) RoundToDLUGrid(int start, int end, VariablePoint variablePoint, DLUDirection direction, out short lengthIndex)
    {
        var points = direction == DLUDirection.X ? DLUXGridPoints : DLUYGridPoints;
        var index = Array.BinarySearch(points, end - start);
        if (index < 0) index = ~index;
        lengthIndex = (short)index;
        return variablePoint switch
        {
            VariablePoint.Start => (end - points[index], end),
            VariablePoint.End => (start, start + points[index]),
            _ => throw new ArgumentException(nameof(variablePoint))
        };
    }
}
