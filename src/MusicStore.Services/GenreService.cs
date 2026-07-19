using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Exceptions;

namespace MusicStore.Services;

public class GenreService(
    IGenreRepository _repository,
    ILogger<GenreService> _logger,
    IMapper _mapper,
    IUnitOfWork _unitOfWork
) : IGenreService {
    
    public async Task<IReadOnlyList<GenreResponseDto>> GetAsync()
    {
        var entities = await _repository
            .Query()
            //.Where(x => x.Status)
            .ToListAsync();

        return _mapper.Map<IReadOnlyList<GenreResponseDto>>(entities);
    }

    public async Task<GenreResponseDto> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity is null || !entity.Status)
        {
            _logger.LogWarning("Genre {Id} no encontrado", id);
            throw new NotFoundException($"No se encontró el género con id {id}");
        }

        return _mapper.Map<GenreResponseDto>(entity);
    }

    public async Task<GenreResponseDto> AddAsync(GenreRequestDto request)
    {
        var entity = _mapper.Map<Genre>(request);

        var created = await _repository.AddAsync(entity);
        //await _unitOfWork.CommitAsync();
        return _mapper.Map<GenreResponseDto>(created);
    }

    public async Task<GenreResponseDto> UpdateAsync(int id, GenreRequestDto request)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity is null || !entity.Status)
        {
            _logger.LogWarning("Intento de actualización de Genre inexistente {Id}", id);
            throw new NotFoundException($"No se encontró el género con id {id}");
        }

        _mapper.Map(request, entity);

        var updated = await _repository.UpdateAsync(entity);
        //await _unitOfWork.CommitAsync();

        return _mapper.Map<GenreResponseDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity is null || !entity.Status)
        {
            _logger.LogWarning("Intento de eliminación de Genre inexistente {Id}", id);
            throw new NotFoundException($"No se encontró el género con id {id}");
        }

        await _repository.DeleteAsync(id);
        //await _unitOfWork.CommitAsync();
    }
}