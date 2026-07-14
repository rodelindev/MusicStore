using MusicStore.Entities;

namespace MusicStore.Repositories;

public interface IRepositoryBase<TEntity, TKey> where TEntity : EntityBase<TKey>
{
    IQueryable<TEntity> Query();
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey id);
}
