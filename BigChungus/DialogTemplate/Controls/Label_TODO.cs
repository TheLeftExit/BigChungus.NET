public class Label : DialogItem
{
    protected override string ClassName => "Static";

    public Label()
    {
        TextHAlignment = HAlignment.Left;
    }

    // Design: WordWrap disabled by default, only works with HAlignment.Left if set, property assignment order doesn't matter.
    public HAlignment TextHAlignment
    {
        get => StyleHelper.GetStyle(style, SS_TYPEMASK, (HAlignment.Left, SS_LEFT), (HAlignment.Left, SS_LEFTNOWORDWRAP), (HAlignment.Right, SS_RIGHT), (HAlignment.Center, SS_CENTER));
        set => StyleHelper.SetStyle(ref style, SS_TYPEMASK, value, (HAlignment.Left, _wordWrap ? SS_LEFT : SS_LEFTNOWORDWRAP), (HAlignment.Right, SS_RIGHT), (HAlignment.Center, SS_CENTER));
    }
    private bool _wordWrap;
    // Also, this only affects automatic word wrap - manual \r\n will always be respected.
    public bool WordWrap
    {
        get
        {
            if(TextHAlignment is HAlignment.Center or HAlignment.Right)
            {
                return false;
            }
            return StyleHelper.GetStyle(style, SS_TYPEMASK, SS_LEFT);
        }
        set
        {
            _wordWrap = value;
            if(TextHAlignment is HAlignment.Left)
            {
                StyleHelper.SetStyle(ref style, SS_TYPEMASK, value ? SS_LEFT : SS_LEFTNOWORDWRAP);
            }
        }
    }

    // There's also ellipsis behavior - it's complicated to understand even with ControlSpy (compat with alignments, etc.), and irrelevant unless Text is bound. I'll revisit it later.
    // That'll probably change the current WordWrap flag implementation, and probably into something even more complex.
}
