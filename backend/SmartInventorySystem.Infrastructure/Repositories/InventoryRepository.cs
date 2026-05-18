using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Repositories;

public class InventoryRepository : GenericRepository<InventoryTransaction>, IInventoryRepository
{
    public InventoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
