using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

public readonly struct ControlProperties
{
    private readonly nint _handle;
    public ControlProperties(nint handle) => _handle = handle;
    public static ControlProperties FromControl<T>(T control)
        where T : IControl<T>
    {
        return new(control.Handle);
    }

    public T? GetProperty<T>(ReadOnlySpan<char> propertyName)
        where T : class
    {
        var propertyHandle = Win32.GetProp(_handle, propertyName);
        if (propertyHandle is 0) return null;
        return (T?)GCHandle.FromIntPtr(propertyHandle).Target;
    }

    public void SetProperty<T>(ReadOnlySpan<char> propertyName, T value)
        where T : class
    {
        var oldPropertyHandle = Win32.GetProp(_handle, propertyName);
        if (oldPropertyHandle is not 0) GCHandle.FromIntPtr(oldPropertyHandle).Free();
        var gcHandle = GCHandle.Alloc(value);
        var handle = GCHandle.ToIntPtr(gcHandle);
        Win32.SetProp(_handle, propertyName, handle);
    }

    public T? RemoveProperty<T>(ReadOnlySpan<char> propertyName)
        where T : class
    {
        var oldHandle = Win32.RemoveProp(_handle, propertyName);
        if (oldHandle is 0) return null;
        var gcHandle = GCHandle.FromIntPtr(oldHandle);
        var value = (T?)gcHandle.Target;
        gcHandle.Free();
        return value;
    }
}
