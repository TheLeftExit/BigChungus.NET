public interface IDialogItemProperties
{
    uint Style { get; init; }
    uint ExStyle { get; init; }
    void SetDefault();
    string ClassName { get; }
}

public static class StyleHelper
{
    public static bool GetTabStop(uint style) => FlagHelper.GetFlag(style, WS_TABSTOP);
    public static void SetTabStop(ref uint style, bool set) => FlagHelper.SetFlag(ref style, WS_TABSTOP, set);

    public static bool GetEnabled(uint style) => !FlagHelper.GetFlag(style, WS_DISABLED);
    public static void SetEnabled(ref uint style, bool set) => FlagHelper.SetFlag(ref style, WS_DISABLED, !set);

    public static bool GetVisible(uint style) => FlagHelper.GetFlag(style, WS_VISIBLE);
    public static void SetVisible(ref uint style, bool set) => FlagHelper.SetFlag(ref style, WS_VISIBLE, set);

    public static bool GetStartGroup(uint style) => FlagHelper.GetFlag(style, WS_GROUP);
    public static void SetStartGroup(ref uint style, bool set) => FlagHelper.SetFlag(ref style, WS_GROUP, set);

    public static bool GetIsChild(uint style) => FlagHelper.GetFlag(style, WS_CHILD);
    public static void SetIsChild(ref uint style, bool set) => FlagHelper.SetFlag(ref style, WS_CHILD, set);
}

public enum HAlignment { Left, Center, Right }

public enum VAlignment { Top, Center, Bottom }
