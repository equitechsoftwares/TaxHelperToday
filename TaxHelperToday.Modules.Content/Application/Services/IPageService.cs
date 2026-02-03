using TaxHelperToday.Modules.Content.Application.DTOs;

namespace TaxHelperToday.Modules.Content.Application.Services;

public interface IPageService
{
    Task<PageDto?> GetByIdAsync(long id);
    Task<PageDto?> GetBySlugAsync(string slug, bool activeOnly = true);
    Task<IEnumerable<PageDto>> GetAllAsync(bool activeOnly = false);
    Task<PageDto> CreateAsync(CreatePageDto dto);
    Task<PageDto> UpdateAsync(long id, UpdatePageDto dto);
    Task<bool> DeleteAsync(long id);
    Task<bool> ToggleActiveAsync(long id);
}
