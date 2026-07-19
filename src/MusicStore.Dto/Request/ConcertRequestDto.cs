using Microsoft.AspNetCore.Http;

namespace MusicStore.Dto.Request;

public class ConcertRequestDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ExtendedDescription { get; set; } = default!;
    public string Place { get; set; } = default!;
    public double UnitPrice { get; set; }
    public int GenreId { get; set; }
    public DateOnly DateEvent { get; set; } = default!;
    public TimeOnly TimeEvent { get; set; } = default!;
    /*[FileSizeValidation(MaxSizeInMegabytes: 1)]
    [FileTypeValidation(fileTypeGroup:FileTypeGroup.Image)]*/
    public IFormFile? Image { get; set; } = default!;
    public int Capacity { get; set; }
}