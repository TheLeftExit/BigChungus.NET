using System.Buffers;

public interface IControl<TSelf>
    where TSelf : IControl<TSelf>
{
    static abstract TSelf Create(nint handle);
    nint Handle { get; }
}

public interface IControl<TSelf, TCommand> : IControl<TSelf>
    where TSelf : IControl<TSelf, TCommand>
    where TCommand : struct, Enum
{
    static abstract bool IsCommandMessage(Message message, out TCommand command);
    bool IsCommandSender(Message message, TCommand command);
}

public readonly struct Control : IControl<Control, NoCommand>
{
    private readonly nint _handle;
    public Control(nint hWnd) => _handle = hWnd;
    static Control IControl<Control>.Create(nint handle) => new(handle);
    nint IControl<Control>.Handle => _handle;

    public static bool IsCommandMessage(Message message, out NoCommand command)
    {
        command = default;
        return false;
    }

    public bool IsCommandSender(Message message, NoCommand command)
    {
        return false;
    }

    public unsafe string? Text
    {
        get
        {
            var bufferSize = (nuint)Win32.SendMessage(_handle, WM_GETTEXTLENGTH, 0, 0) + 1;
            using var memoryOwner = MemoryPool<char>.Shared.Rent((int)bufferSize);
            using var memoryPointer = memoryOwner.Memory.Pin();
            Win32.SendMessage(_handle, WM_GETTEXT, bufferSize, (nint)memoryPointer.Pointer);
            return new string((char*)memoryPointer.Pointer);
        }
        set
        {
            fixed (char* buffer = value)
            {
                Win32.SendMessage(_handle, WM_SETTEXT, 0, (nint)buffer);
            }
        }
    }
}
