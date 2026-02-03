using TaxHelperToday.Modules.Content.Application.DTOs;

namespace TaxHelperToday.Modules.Content.Application.Services;

public interface IServiceService
{
    Task<ServiceDto?> GetByIdAsync(long id);
    Task<ServiceDto?> GetBySlugAsync(string slug);
    Task<IEnumerable<ServiceDto>> GetAllAsync(bool activeOnly = false);
    Task<IEnumerable<ServiceDto>> GetByTypeAsync(string type, bool activeOnly = false);
    Task<ServiceDto> CreateAsync(CreateServiceDto dto);
    Task<ServiceDto> UpdateAsync(long id, UpdateServiceDto dto);
    Task<bool> DeleteAsync(long id);
    Task<bool> ToggleActiveAsync(long id);
}
