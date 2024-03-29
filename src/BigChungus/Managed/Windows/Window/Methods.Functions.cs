﻿using BigChungus.Unmanaged;
using System.Drawing;

namespace BigChungus.Managed;

public readonly partial record struct Window
{
	public RECT GetBounds()
	{
		User32.GetWindowRect(Handle, out var result).ThrowIfFalse();
		return result;
	}

	public void SetBounds(Rectangle newBounds, bool repaint = true)
	{
		User32.MoveWindow(Handle, newBounds.X, newBounds.Y, newBounds.Width, newBounds.Height, repaint).ThrowIfFalse();
	}

	public nint GetParent()
	{
		return User32.GetParent(Handle);
	}

	public void SetParent(nint newParent)
	{
		User32.SetParent(Handle, newParent);
	}

	public void Update()
	{
		User32.UpdateWindow(Handle).ThrowIfFalse();
	}

	public void Destroy()
	{
		User32.DestroyWindow(Handle).ThrowIfFalse();
	}

	public void SetEnabled(bool enabled)
	{
		User32.EnableWindow(Handle, enabled);
	}

	public bool GetEnabled()
	{
		return User32.IsWindowEnabled(Handle);
	}

	public void SetVisible(bool visible)
	{
		User32.ShowWindow(Handle, visible ? SW.SHOW : SW.HIDE);
	}

	public bool GetVisible()
	{
		return User32.IsWindowVisible(Handle);
	}
}