public class MyViewModel : BindableViewModel
{
    public required string Text { get; set => SetValue(ref field, value); }
    private int clickCount = 0;

    public void OnClick()
    {
        Text = $"Clicked {++clickCount} times!";
    }
}


