using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Interfaces;

public interface IInventoryRepository : IGenericRepository<InventoryTransaction>
{
}
