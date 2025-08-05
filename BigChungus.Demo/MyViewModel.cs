using System.Diagnostics.CodeAnalysis;

public class MyViewModel : BindableViewModel
{
    public CommonDialogService CommonDialogService { get; set; } = null!;

    public required string Text { get; set => SetValue(ref field, value); }
    private int clickCount = 0;

    public void OnClick()
    {
        Text = $"Clicked {++clickCount} times!";
        CommonDialogService.ShowMessageBox("Good job!");
    }

    public string Caption { get; set; } = "Initial Caption";

    public bool Checked
    {
        get;
        set
        {
            Caption = value ? "Checked" : "Unchecked";
            SetValue(ref field, value, "Caption");
        }
    }
}


