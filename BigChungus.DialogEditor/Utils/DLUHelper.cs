using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public readonly struct DLUHelper
{
    public const int MaxDLU = 2000;

    private readonly nint _dialogHandle;
    public DLUHelper(nint dialogHandle) => _dialogHandle = dialogHandle;

    public int DLUToPixelsHorz(int value) => DLUToPixels(value, DLUDirection.X);
    public int DLUToPixelsVert(int value) => DLUToPixels(value, DLUDirection.Y);
    public double PixelsToDLUHorz(int value) => PixelsToDLU(value, DLUDirection.X);
    public double PixelsToDLUVert(int value) => PixelsToDLU(value, DLUDirection.Y);

    private int DLUToPixels(int value, DLUDirection direction)
    {
        var rect = direction switch
        {
            DLUDirection.X => new Win32.RECT { right = value },
            DLUDirection.Y => new Win32.RECT { bottom = value },
            _ => throw new ArgumentException(nameof(direction))
        };
        Win32.MapDialogRect(_dialogHandle, ref rect);
        return direction switch
        {
            DLUDirection.X => rect.right,
            DLUDirection.Y => rect.bottom,
            _ => throw new UnreachableException()
        };
    }

    private double PixelsToDLU(int value, DLUDirection direction)
    {
        if (value < DLUToPixels(0, direction) || value > DLUToPixels(MaxDLU, direction)) throw new InvalidOperationException();

        var low = 0;
        var high = MaxDLU;

        while (true)
        {
            var midLow = (low + high) >> 1;
            var midLowValue = DLUToPixels(midLow, direction);

            var midHigh = midLow + ((low + high) & 1);
            var midHighValue = midLow != midHigh ? DLUToPixels(midHigh, direction) : midLowValue;

            // Check the [midLow, midHigh] range (either single-value, or 1-DLU-wide)

            if (value < midLowValue) // Out of range, search lower
            {
                high = midLow;
            }
            else if (value > midHighValue) // Out of range, search higher
            {
                low = midHigh;
            }
            else if (midLow == midHigh) // Within a single-value range
            {
                return midLow;
            }
            else // Within a 1-DLU-wide range
            {
                return midLow + (double)(value - midLowValue) / (midHighValue - midLowValue);
            }
        }
    }
}
