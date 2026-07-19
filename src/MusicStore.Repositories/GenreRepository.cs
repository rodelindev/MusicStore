using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Persistance;

namespace MusicStore.Repositories;

public class GenreRepository : RepositoryBase<Genre, int>, IGenreRepository
{
    public GenreRepository(ApplicationDbContext context) : base(context)
    {
    }
}