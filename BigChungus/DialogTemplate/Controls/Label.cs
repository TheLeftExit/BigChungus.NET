public class Label : DialogItem
{
    protected override string ClassName => "Static";

    public Label()
    {
        
    }

    public HAlignment TextAlignment
    {
        get => StyleHelper.GetFlag(style, (HAlignment.Left, SS_LEFT), (HAlignment.Right, SS_RIGHT), (HAlignment.Center, SS_CENTER));
        set => StyleHelper.SetFlag(ref style, value, (HAlignment.Left, SS_LEFT), (HAlignment.Right, SS_RIGHT), (HAlignment.Center, SS_CENTER));
    }

    public TextOverflowBehavior TextOverflowBehavior
    {
        get => StyleHelper.GetFlag(style, (TextOverflowBehavior.EllipsisOnAnyCharacter, SS_ENDELLIPSIS), (TextOverflowBehavior.EllipsisOnWordBoundary, SS_WORDELLIPSIS), (TextOverflowBehavior.EllipsisInsideFilePath, SS_PATHELLIPSIS), (TextOverflowBehavior.WordWrap, 0u));
        set => StyleHelper.SetFlag(ref style, value, (TextOverflowBehavior.EllipsisOnAnyCharacter, SS_ENDELLIPSIS), (TextOverflowBehavior.EllipsisOnWordBoundary, SS_WORDELLIPSIS), (TextOverflowBehavior.EllipsisInsideFilePath, SS_PATHELLIPSIS), (TextOverflowBehavior.WordWrap, 0u));
    }
}

public enum TextOverflowBehavior
{
    EllipsisOnAnyCharacter,
    EllipsisOnWordBoundary,
    EllipsisInsideFilePath,
    WordWrap
}
