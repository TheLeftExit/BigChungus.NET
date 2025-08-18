var viewModel = new SimpleViewModel();
var view = new SimpleView();
view.ShowDialog(viewModel);

public class SimpleView : DialogViewBase<SimpleViewModel>
{
    protected override void Configure(DialogBuilder<SimpleViewModel> builder)
    {
        var label = builder.AddItem<Label>(new RectangleDLU(4, 4, 100, 10), "Label");
        var textBox = builder.AddItem<TextBox>(new RectangleDLU(4, 16, 100, 12), "TextBox");
        var button = builder.AddItem<Button>(new RectangleDLU(4, 32, 50, 12), "Get freaky");
        var checkBox = builder.AddItem<CheckBox>(new RectangleDLU(56, 32, 50, 12), "CheckBox");
        var progressBar = builder.AddItem<ProgressBar>(new RectangleDLU(4, 48, 100, 12), "ProgressBar", x =>
        {
            x.SmoothBackwardProgress = true;
        });

        var groupBox = builder.AddItem<GroupBox>(new RectangleDLU(4, 64, 100, 60), "ProgressBar settings");
        builder.BeginRadioGroup();
        var radioActive = builder.AddItem<RadioButton>(new RectangleDLU(8, 72, 80, 12), "Active");
        var radioError = builder.AddItem<RadioButton>(new RectangleDLU(8, 84, 80, 12), "Error");
        var radioPaused = builder.AddItem<RadioButton>(new RectangleDLU(8, 96, 80, 12), "Paused");
        builder.EndRadioGroup();
        var textBoxProgress = builder.AddItem<TextBox>(new RectangleDLU(8, 108, 80, 12));
        var buttonShowDialog = builder.AddItem<Button>(new RectangleDLU(4, 128, 92, 12), "(broken button)");

        builder.SetBinding(progressBar, x => x.State, x => x.State);
        builder.SetBinding(progressBar, x => x.Value, x => x.Value);

        builder.SetBinding(textBoxProgress, x => x.Text, x => x.ValueAsText);
        builder.SetRadioGroupBinding(x => x.State,
            (radioActive, ProgressBarState.Normal),
            (radioError, ProgressBarState.Error),
            (radioPaused, ProgressBarState.Paused)
        );

        builder.SetCommand(button, x => x.OnClick());

        builder.UseDispatcher();
        builder.UseApplicationIcon();
    }
}

public class SimpleViewModel : DialogViewModelBase<SimpleView, SimpleViewModel>
{
    public ProgressBarState State { get; set => SetValue(ref field, value); }
    public double Value { get; set => SetValue(ref field, value); } = 0.4;
    public string ValueAsText
    {
        get => Value.ToString();
        set => Value = double.Parse(value);
    }

    private bool _gettingFreaky;

    public async void OnClick()
    {
        if(!_gettingFreaky)
        {
            _gettingFreaky = true;
            var up = true;
            while (_gettingFreaky)
            {
                Value += up ? 0.2 : -0.2;
                up = !up;
                await Task.Delay(500);
            }
            Value = 0.5;

        }
        else
        {
            _gettingFreaky = false;
        }
        
    }
}
