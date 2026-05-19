namespace POS.Application.DTOs;

public sealed class SystemStatusDto
{
    public string DatabaseProvider { get; init; } = string.Empty;

    public bool SyncEnabled { get; init; }

    public bool RequiresSync { get; init; }

    public string StatusText { get; init; } = string.Empty;
}
