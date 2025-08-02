public record struct RectangleDLU(short X, short Y, short Width, short Height)
{
    public PointDLU Location
    {
        get => new(X, Y);
        set => (X, Y) = value;
    }

    public SizeDLU Size
    {
        get => new(Width, Height);
        set => (Width, Height) = value;
    }
}

public record struct PointDLU(short X, short Y);

public record struct SizeDLU(short Width, short Height);
