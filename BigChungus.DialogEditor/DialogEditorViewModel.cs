using System.Collections.ObjectModel;
using System.ComponentModel;

interface IDialogEditorControl
{
    RectangleDLU Bounds { get; set; }
    string? Text { get; set; }
    string WindowClass { get; set; }
    uint Style { get; set; }
    uint ExStyle { get; set; }

    // General idea; will probably be changed
    (string Name, object Value)[] Properties { get; }
    object GetProperty(string key);
    void SetProperty(string key, object value);
}

class DialogEditorViewModel
{
    public SizeDLU Size { get; set; }
    public ObservableCollection<IDialogEditorControl> Controls { get; } = new();
}

