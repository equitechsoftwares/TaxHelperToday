using TaxHelperToday.Modules.Content.Application.DTOs;

namespace TaxHelperToday.Modules.Content.Application.Services;

public interface IBlogService
{
    Task<BlogPostDto?> GetByIdAsync(long id);
    Task<BlogPostDto?> GetBySlugAsync(string slug);
    Task<IEnumerable<BlogPostDto>> GetAllAsync(bool publishedOnly = false);
    Task<IEnumerable<BlogPostDto>> GetByCategoryAsync(string category, bool publishedOnly = false);
    Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto, long userId);
    Task<BlogPostDto> UpdateAsync(long id, UpdateBlogPostDto dto, long userId);
    Task<bool> DeleteAsync(long id);
    Task<bool> PublishAsync(long id, long userId);
    Task<bool> UnpublishAsync(long id);
    Task IncrementViewCountAsync(long id);
}
