global using static Win32Macros;

TestCases.ShowListView();

public static class TestCases
{
    public static void ShowDialogEditor()
    {
        DialogEditor.Show(new(200, 100));
    }

    public static void ShowListView()
    {
        var builder = new DialogBuilder<ListViewModel>();
        builder.Properties.Text = "List View Example";
        builder.Properties.Size = new(300, 200);

        builder.RegisterService(x => x.CommonDialogService);

        var listView = builder.AddItem<ListView>(new(4, 4, 280, 150));
        builder.SetListViewVirtualBinding(
            listView,
            x => x.Items,
            [
                new ListViewValueColumn<ListItem, int>("Id", x => x.Id, 100),
                new ListViewStringColumn<ListItem>("Name", x => x.Name ?? "None", 100)
            ],
            ListViewOptions.FullRowSelect | ListViewOptions.GridLines | ListViewOptions.HeaderDragDrop
        );
        builder.SetListViewCommand(listView, x => x.OnCommand);

        var addButton = builder.AddItem<Button>(new(4, 160, 80, 12), "Add");
        var removeButton = builder.AddItem<Button>(new(90, 160, 80, 12), "Remove");
        var updateButton = builder.AddItem<Button>(new(176, 160, 80, 12), "Update");

        builder.SetCommand(addButton, x => x.OnAddItem());
        builder.SetCommand(removeButton, x => x.OnRemoveItem());
        builder.SetCommand(updateButton, x => x.OnUpdateItem());

        var dialogRunner = builder.Build();
        var viewModel = new ListViewModel();
        dialogRunner.Run(viewModel);
    }

    public static void ShowSomeControls()
    {
        var builder = new DialogBuilder<SimpleViewModel>();

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

        
        var dialogRunner = builder.Build();
        var viewModel = new SimpleViewModel { Text = "Hello, World!" };
        dialogRunner.Run(viewModel);
    }
}
