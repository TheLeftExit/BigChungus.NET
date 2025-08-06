using System.Diagnostics.CodeAnalysis;

public class MyViewModel : BindableViewModel
{
    public CommonDialogService CommonDialogService { get; set; } = null!;

    public required string Text { get; set => SetValue(ref field, value); }
    private int clickCount = 0;

    public void OnClick()
    {
        Text = $"Clicked {++clickCount} times!";
        RadioValue = (RadioButtonValue)((int)(RadioValue + 1) % 4);
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

    public RadioButtonValue RadioValue
    {
        get;
        set
        {
            Text = value.ToString();
            SetValue(ref field, value);
        }
    } = RadioButtonValue.English;
}

public enum RadioButtonValue
{
    English,
    Greek,
    Montenegrin,
    Chinese
}
