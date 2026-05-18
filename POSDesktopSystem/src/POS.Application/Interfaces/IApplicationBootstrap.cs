namespace POS.Application.Interfaces;

public interface IApplicationBootstrap
{
    bool IsInitialized { get; }

    Task InitializeAsync(CancellationToken cancellationToken = default);

    void StartBackgroundServices();
}
