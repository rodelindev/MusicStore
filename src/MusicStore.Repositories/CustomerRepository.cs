using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Persistence;

namespace MusicStore.Repositories;

public class CustomerRepository : RepositoryBase<Customer, int>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Set<Customer>().FirstOrDefaultAsync(x => x.Email == email);
    }
}
