using System.Collections.ObjectModel;
using System.ComponentModel;

public class ListViewModel : BindableViewModel
{
    public ObservableCollection<ListItem> Items { get; set; } = new(Enumerable.Range(0, 100).Select(i => new ListItem { Id = i, Name = $"Item {i}" }));

    public void OnAddItem() => Items.Insert(0, new ListItem { Id = Items.Count, Name = $"New Item" });
    public void OnRemoveItem() => Items.RemoveAt(10);
    public void OnUpdateItem() => Items[5].Id++;

    public CommonDialogService CommonDialogService { get; set; } = null!;

    public void OnCommand(int index)
    {
        if(index < 0 || index >= Items.Count) return;
        var item = Items[index];
        CommonDialogService.ShowMessageBox($"Command executed for {item.Name} (ID: {item.Id})");
    }
}

public class ListItem : BindableViewModel
{
    public int Id { get; set => SetValue(ref field, value); }
    public string? Name { get; set => SetValue(ref field, value); }
}
