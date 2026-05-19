namespace POS.UI.Helpers;

public static class UiTheme
{
    public static readonly Color Primary = Color.FromArgb(30, 64, 124);
    public static readonly Color PrimaryDark = Color.FromArgb(24, 52, 96);
    public static readonly Color PrimaryLight = Color.FromArgb(42, 88, 156);
    public static readonly Color PrimaryHover = Color.FromArgb(34, 72, 128);
    public static readonly Color Success = Color.FromArgb(46, 125, 50);
    public static readonly Color Error = Color.FromArgb(198, 40, 40);
    public static readonly Color Surface = Color.FromArgb(245, 247, 250);
    public static readonly Color Card = Color.White;
    public static readonly Font TitleFont = new("Segoe UI", 14F, FontStyle.Bold);
    public static readonly Font SectionFont = new("Segoe UI", 10F, FontStyle.Bold);
    public static readonly Font BodyFont = new("Segoe UI", 9F, FontStyle.Regular);

    public static void StylePrimaryButton(Button button)
    {
        button.BackColor = Primary;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = SectionFont;
        button.Cursor = Cursors.Hand;
    }

    public static void StyleSuccessButton(Button button)
    {
        button.BackColor = Success;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = SectionFont;
        button.Cursor = Cursors.Hand;
    }
}
