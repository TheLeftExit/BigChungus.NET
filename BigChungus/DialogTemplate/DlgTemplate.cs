using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public sealed class DlgTemplate : BinaryTemplate
{
    public uint Style = WS_SYSMENU | WS_MINIMIZEBOX | WS_CAPTION | DS_MODALFRAME | DS_SETFONT | DS_CENTER;
    public uint ExStyle = WS_EX_APPWINDOW;
    public List<DlgItemTemplate> Items { get; } = new();
    public short X = 0;
    public short Y = 0;
    public short Width = 200;
    public short Height = 200;
    public string? WindowText = "Dialog Box";
    public short FontSize = DialogBoxHelper.DefaultFontSize;
    public string? FontName = DialogBoxHelper.DefaultFontName;

    protected override void Write(ref SpanWriter writer)
    {
        writer.Write(Style);
        writer.Write(ExStyle);
        writer.Write((ushort)Items.Count);
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Width);
        writer.Write(Height);

        writer.Write('\0'); // not supported: menu resource identifier
        writer.Write('\0'); // not supported: custom dialog class
        writer.Write(WindowText); writer.Write('\0');

        if (StyleHelper.GetFlag(Style, DS_SETFONT))
        {
            writer.Write(FontSize);
            writer.Write(FontName); writer.Write('\0');
        }

        foreach (var item in Items)
        {
            Write(item, ref writer);
        }
    }
}

public sealed class DlgItemTemplate : BinaryTemplate
{
    public uint Style;
    public uint ExStyle;
    public short X;
    public short Y;
    public short Width;
    public short Height;
    public ushort Id;
    public string? WindowText;
    public string? WindowClass;

    protected override void Write(ref SpanWriter writer)
    {
        while (writer.Position % 4 != 0)
        {
            writer.Write<byte>(0);
        }

        writer.Write(Style);
        writer.Write(ExStyle);
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(Id);

        ArgumentException.ThrowIfNullOrWhiteSpace(WindowClass);
        writer.Write(WindowClass); writer.Write('\0');
        writer.Write(WindowText); writer.Write('\0');
        writer.Write('\0'); // not implemented: creation data
    }
}

public abstract class BinaryTemplate
{
    public int GetTemplateLength()
    {
        var measurer = new SpanWriter();
        Write(ref measurer);
        return measurer.Position;

    }

    public int GetTemplate(Span<byte> buffer)
    {
        var writer = new SpanWriter(buffer);
        Write(ref writer);
        return writer.Position;
    }

    protected abstract void Write(ref SpanWriter writer);
    protected static void Write(BinaryTemplate sourceTemplate, ref SpanWriter writer)
    {
        sourceTemplate.Write(ref writer);
    }
    public ReadOnlyMemory<byte> ToMemory()
    {
        var buffer = new byte[GetTemplateLength()];
        GetTemplate(buffer);
        return buffer;
    }

    protected unsafe ref struct SpanWriter
    {
        public int Position { get; private set; }
        private readonly Span<byte> buffer;
        private readonly bool IsSpanBacked() => !buffer.IsEmpty;

        public SpanWriter(Span<byte> targetSpan) => buffer = targetSpan;

        public void Write<T>(T value) where T : unmanaged
        {
            if (IsSpanBacked())
            {
                Unsafe.As<byte, T>(ref buffer[Position]) = value;
            }
            Position += sizeof(T);
        }
        public void Write(ReadOnlySpan<char> value)
        {
            if (value.Contains('\0')) throw new InvalidOperationException("String contains null terminator.");

            var byteSpan = MemoryMarshal.AsBytes(value);
            if (IsSpanBacked()) byteSpan.CopyTo(buffer.Slice(Position));
            Position += byteSpan.Length;
        }
    }
}
