namespace POS.UI.Navigation;

public sealed class DashboardNavigator
{
    private readonly Panel _contentHost;
    private UserControl? _activeView;

    public DashboardNavigator(Panel contentHost)
    {
        _contentHost = contentHost ?? throw new ArgumentNullException(nameof(contentHost));
    }

    public DashboardSection? ActiveSection { get; private set; }

    public void ShowView(DashboardSection section, UserControl view)
    {
        ArgumentNullException.ThrowIfNull(view);

        _activeView?.Dispose();
        _contentHost.Controls.Clear();

        view.Dock = DockStyle.Fill;
        _contentHost.Controls.Add(view);
        _activeView = view;
        ActiveSection = section;
    }

    public void Clear()
    {
        _activeView?.Dispose();
        _contentHost.Controls.Clear();
        _activeView = null;
        ActiveSection = null;
    }
}
