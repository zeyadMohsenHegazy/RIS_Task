using POS.Application.Interfaces;

namespace POS.Infrastructure.Data;

public sealed class DatabaseInfo : IDatabaseInfo
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public DatabaseInfo(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public bool RequiresSync => _connectionFactory.RequiresSync;
}
