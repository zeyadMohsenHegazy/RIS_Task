namespace POS.UI.Helpers;

public sealed class SearchDebouncer : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer;
    private readonly Func<Task> _searchAction;

    public SearchDebouncer(Func<Task> searchAction, int intervalMilliseconds = 350)
    {
        _searchAction = searchAction ?? throw new ArgumentNullException(nameof(searchAction));
        _timer = new System.Windows.Forms.Timer { Interval = intervalMilliseconds };
        _timer.Tick += async (_, _) =>
        {
            _timer.Stop();
            await _searchAction();
        };
    }

    public void Schedule()
    {
        _timer.Stop();
        _timer.Start();
    }

    public void Dispose() => _timer.Dispose();
}
