DialogEditor.Show(new(200, 100));
return;

var builder = new DialogBuilder<MyViewModel>();

builder.Properties.Text = "My Dialog";
builder.Properties.Size = new(200, 200);

var button = builder.AddItem<Button>(new(4, 4, 80, 12), "Click Me");
var checkBox = builder.AddItem<CheckBox>(new(4, 20, 80, 12), "&Check Me");
var groupBox = builder.AddItem<GroupBox>(new(4, 36, 80, 40), "Group Box");
var textBox = builder.AddItem<TextBox>(new(4, 80, 80, 12), "Type Here");

builder.BeginRadioGroup();
var radio1 = builder.AddItem<RadioButton>(new(4, 96, 80, 12), "English");
var radio2 = builder.AddItem<RadioButton>(new(4, 108, 80, 12), "Greek");
var radio3 = builder.AddItem<RadioButton>(new(4, 120, 80, 12), "Montenegrin");
var radio4 = builder.AddItem<RadioButton>(new(4, 132, 80, 12), "Chinese");
builder.EndRadioGroup();

var label = builder.AddItem<Label>(new(4, 144, 80, 12), "Word Word Word Word Word \r\nWord Word Word Word Word Word");

builder.SetCommand(button, x => x.OnClick());
builder.SetBinding(button, x => x.Text, x => x.Text);
builder.SetProperty(button, x => x.ShowShield, true);

builder.SetBinding(x => x.Text, x => x.Caption);
builder.SetBinding(checkBox, x => x.IsChecked, x => x.Checked);

builder.SetProperty(textBox, x => x.MaxLength, 30);

builder.RegisterService(x => x.CommonDialogService);

builder.SetRadioGroupBinding(x => x.RadioValue,
    (radio1, RadioButtonValue.English),
    (radio2, RadioButtonValue.Greek),
    (radio3, RadioButtonValue.Montenegrin),
    (radio4, RadioButtonValue.Chinese)
);

var viewModel = new MyViewModel { Text = "Hello, World!" };
var dialogRunner = builder.Build(viewModel);
dialogRunner.Run();
