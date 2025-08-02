var builder = new DialogBuilder<MyViewModel>();

var button = builder.AddItem<Button>(new(4, 4, 80, 12), "Click Me");
builder.SetCommand(button, x => x.OnClick());
builder.SetBinding(button, x => x.Text, x => x.Text);

var viewModel = new MyViewModel { Text = "Hello, World!" };
var dialogRunner = builder.Build(viewModel);
dialogRunner.Run();
