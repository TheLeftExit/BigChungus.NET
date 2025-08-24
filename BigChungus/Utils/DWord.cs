public readonly record struct DWord(ushort Low, ushort High)
{
    public DWord(nuint value) : this((ushort)(value & 0xFFFF), (ushort)((value >> 16) & 0xFFFF)) { }
    public readonly nint Value => (High << 16) | Low;
    public int XParam => unchecked((short)Low);
    public int YParam => unchecked((short)High);

    public static implicit operator nint(DWord dWord) => dWord.Value;
    public static implicit operator DWord(nuint value) => new(value);
    public static implicit operator (ushort, ushort)(DWord dWord) => (dWord.Low, dWord.High);
}
