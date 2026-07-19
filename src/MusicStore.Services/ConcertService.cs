using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Common;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Exceptions;

namespace MusicStore.Services;

public class ConcertService(
    IConcertRepository repository,
    IGenreRepository genreRepository,
    IUnitOfWork unitOfWork,
    ILogger<ConcertService> logger,
    IMapper mapper,
    IFileStorage fileStorage
) : IConcertService
{
    private readonly string container = "concerts";

    public async Task<PaginatedResult<ConcertResponseDto>> GetAsync(string? title, int page, int pageSize)
    {
        var data = await repository.GetDetailAsync(title);
        var total = data.Count;
        var items = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedResult<ConcertResponseDto>
        {
            Data = mapper.Map<IReadOnlyList<ConcertResponseDto>>(items),
            Meta = new PaginationMeta
            {
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            }
        };
    }

    public async Task<IReadOnlyList<ConcertResponseDto>> GetUpcomingAsync()
    {
        var now = DateTime.Now;
        var data = await repository.GetDetailAsync(null);
        var filtered = data.Where(x => x.DateEvent >= now).ToList();

        return mapper.Map<IReadOnlyList<ConcertResponseDto>>(filtered);
    }

    public async Task<ConcertResponseDto> GetAsync(int id)
    {
        var entity = await repository
            .Query()
            .Include(x => x.Genre)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null || !entity.Status)
        {
            logger.LogWarning("Concert {Id} no encontrado", id);
            throw new NotFoundException($"No se encontró el concierto con id {id}");
        }

        return mapper.Map<ConcertResponseDto>(entity);
    }

    public async Task<ConcertResponseDto> AddAsync(ConcertRequestDto request)
    {
        var response = new ConcertResponseDto();
        Concert? entity = null;

        var genre = await genreRepository.GetByIdAsync(request.GenreId);

        if (genre is null)
        {
            logger.LogWarning("Intento de creación de Concert con Genre inexistente {GenreId}", request.GenreId);
            throw new NotFoundException($"No se encontró el género con id {request.GenreId}");
        }

        try
        {
            entity = mapper.Map<Concert>(request);

            if (request.Image is not null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await request.Image.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(request.Image.FileName);
                    entity.ImageUrl = await fileStorage.SaveFile(content,
                        extension, container, request.Image.ContentType);
                }
            }

            entity.DateEvent = request.DateEvent.ToDateTime(request.TimeEvent);
            entity.Finalized = false;
            entity.Status = true;
            entity.Genre = genre;
            await repository.AddAsync(entity);
            await unitOfWork.CommitAsync();

            response = mapper.Map<ConcertResponseDto>(entity);
        }
        catch (Exception ex)
        {
            await fileStorage.DeleteFile(entity?.ImageUrl ?? string.Empty,
                container); //si ocurre algún error borrar la imagen (si existe)
            logger.LogError(ex, "Error al guardar la imagen");
        }

        return response;
    }

    public async Task<ConcertResponseDto> UpdateAsync(int id, ConcertRequestDto request)
    {
        var genre = await genreRepository.GetByIdAsync(request.GenreId);

        if (genre is null)
        {
            logger.LogWarning("Intento de creación de Concert con Genre inexistente {GenreId}", request.GenreId);
            throw new NotFoundException($"No se encontró el género con id {request.GenreId}");
        }

        var entity = await repository.GetByIdAsync(id);

        if (entity is null || !entity.Status)
        {
            logger.LogWarning("Intento de actualización de Concert inexistente {Id}", id);
            throw new NotFoundException($"No se encontró el concierto con id {id}");
        }

        var previousImageUrl = entity.ImageUrl;
        var newImageUrl = previousImageUrl;

        try
        {
            mapper.Map(request, entity);

            if (request.Image is not null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await request.Image.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(request.Image.FileName);
                    newImageUrl = await fileStorage.SaveFile(content, extension, container, request.Image.ContentType);
                    entity.ImageUrl = newImageUrl;
                }
            }
            else
            {
                entity.ImageUrl = string.Empty;
            }

            entity.DateEvent = request.DateEvent.ToDateTime(request.TimeEvent);
            entity.Genre = genre;

            await repository.UpdateAsync(entity);
            await unitOfWork.CommitAsync();

            if (request.Image is not null && previousImageUrl is not null)
            {
                await fileStorage.DeleteFile(previousImageUrl, container);
            }
        }
        catch (Exception ex)
        {
            if (newImageUrl != previousImageUrl)
            {
                await fileStorage.DeleteFile(newImageUrl ?? string.Empty, container);
            }

            logger.LogError(ex, "Error al actualizar el concert");
            throw;
        }

        return mapper.Map<ConcertResponseDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);

        if (entity is null || !entity.Status)
        {
            logger.LogWarning("Intento de eliminación de Concert inexistente {Id}", id);
            throw new NotFoundException($"No se encontró el concierto con id {id}");
        }

        await fileStorage.DeleteFile(entity.ImageUrl ?? string.Empty, container);
        await repository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }

    public async Task FinalizeAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);

        if (entity is null || !entity.Status)
        {
            logger.LogWarning("Intento de finalización de Concert inexistente {Id}", id);
            throw new NotFoundException($"No se encontró el concierto con id {id}");
        }

        if (entity.Finalized)
        {
            logger.LogWarning("Concert {Id} ya estaba finalizado", id);
            return;
        }

        entity.Finalized = true;

        await repository.UpdateAsync(entity);
        await unitOfWork.CommitAsync();
    }
}