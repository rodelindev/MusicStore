using MusicStore.Entities;

namespace MusicStore.Repositories;

public interface ICustomerRepository : IRepositoryBase<Customer, int>
{
    Task<Customer?> GetByEmailAsync(string email);
}
