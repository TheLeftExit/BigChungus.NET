public class MyViewModel : BindableViewModel
{
    public required string Text { get; set => SetValue(ref field, value); }
    private int clickCount = 0;

    public void OnClick()
    {
        Text = $"Clicked {++clickCount} times!";
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


