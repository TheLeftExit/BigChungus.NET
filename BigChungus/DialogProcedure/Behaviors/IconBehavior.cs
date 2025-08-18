using System.Runtime.InteropServices;

public class IconBehavior<TViewModel> : IDialogBehavior<TViewModel>
    where TViewModel : class
{
    nint? IDialogBehavior<TViewModel>.OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if (message.msg is WM_INITDIALOG)
        {
            var iconHandle = Win32.LoadIcon(NativeLibrary.GetMainProgramHandle(), new IntResource(32512)); // Same as <ApplicationIcon>
            Win32.SendMessage(context.DialogBoxHandle, WM_SETICON, ICON_BIG, iconHandle);
            Win32.SendMessage(context.DialogBoxHandle, WM_SETICON, ICON_SMALL, iconHandle);
            return null;
        }
        return null;
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void UseApplicationIcon<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder
    )
        where TViewModel : class
    {
        var behavior = new IconBehavior<TViewModel>();
        builder.AddBehavior(behavior);
    }
}
