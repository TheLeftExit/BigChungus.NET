﻿using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

public partial class Window
{
    protected static Dictionary<nint, Window> windows = new();
    protected static Stack<Window> createdWindows = new(); // Allows handling of WM_CREATE/WM_NCCREATE in WndProc in forms

    public unsafe Window(ReadOnlySpan<char> className)
    {
        createdWindows.Push(this);
        fixed (char* classNamePtr = className)
        {
            Handle = User32.CreateWindowEx(
                default,
                classNamePtr,
                default,
                default,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                default,
                default,
                default,
                default
            );
        }
        createdWindows.Pop();
        windows.Add(Handle, this);
    }

    public nint Handle { get; }
}

