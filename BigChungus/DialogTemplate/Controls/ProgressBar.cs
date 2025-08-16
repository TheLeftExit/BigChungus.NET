public class ProgressBar : DialogItem
{
    protected override string ClassName => "msctls_progress32";

    public ProgressBar()
    {
        Visible = true;
    }

    public bool SmoothBackwardProgress
    {
        get => StyleHelper.GetFlag(style, PBS_SMOOTHREVERSE);
        set => StyleHelper.SetFlag(ref style, PBS_SMOOTHREVERSE, value);
    }

    public bool Vertical
    {
        get => StyleHelper.GetFlag(style, PBS_VERTICAL);
        set => StyleHelper.SetFlag(ref style, PBS_VERTICAL, value);
    }
}
