using TaxHelperToday.Modules.Content.Application.DTOs;

namespace TaxHelperToday.Modules.Content.Application.Services;

public interface IFaqService
{
    Task<FaqDto?> GetByIdAsync(long id);
    Task<IEnumerable<FaqDto>> GetAllAsync(bool activeOnly = false);
    Task<IEnumerable<FaqDto>> GetByCategoryAsync(string category, bool activeOnly = false);
    Task<FaqDto> CreateAsync(CreateFaqDto dto);
    Task<FaqDto> UpdateAsync(long id, UpdateFaqDto dto);
    Task<bool> DeleteAsync(long id);
    Task<bool> ToggleActiveAsync(long id);
}
