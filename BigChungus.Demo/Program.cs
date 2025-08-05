var builder = new DialogBuilder<MyViewModel>();

builder.Properties.Text = "My Dialog";

var button = builder.AddItem<Button>(new(4, 4, 80, 12), "Click Me");
var checkBox = builder.AddItem<CheckBox>(new(4, 20, 80, 12), "Check Me");
var groupBox = builder.AddItem<GroupBox>(new(4, 36, 80, 40), "Group Box");

builder.SetCommand(button, x => x.OnClick());
builder.SetBinding(button, x => x.Text, x => x.Text);
builder.SetProperty(button, x => x.ShowShield, true);

builder.SetBinding(x => x.Text, x => x.Caption);
builder.SetBinding(checkBox, x => x.IsChecked, x => x.Checked); 

var viewModel = new MyViewModel { Text = "Hello, World!" };
var dialogRunner = builder.Build(viewModel);
dialogRunner.Run();
