using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Modules.Content.Infrastructure.Services;

public class FaqService : IFaqService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FaqService> _logger;

    public FaqService(ApplicationDbContext context, ILogger<FaqService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FaqDto?> GetByIdAsync(long id)
    {
        var faq = await _context.Faqs.FindAsync(id);
        return faq == null ? null : MapToDto(faq);
    }

    public async Task<IEnumerable<FaqDto>> GetAllAsync(bool activeOnly = false)
    {
        var query = _context.Faqs.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(f => f.IsActive);
        }

        var faqs = await query
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.Question)
            .ToListAsync();

        return faqs.Select(MapToDto);
    }

    public async Task<IEnumerable<FaqDto>> GetByCategoryAsync(string category, bool activeOnly = false)
    {
        var query = _context.Faqs
            .Where(f => f.Category == category)
            .AsQueryable();

        if (activeOnly)
        {
            query = query.Where(f => f.IsActive);
        }

        var faqs = await query
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.Question)
            .ToListAsync();

        return faqs.Select(MapToDto);
    }

    public async Task<FaqDto> CreateAsync(CreateFaqDto dto)
    {
        var faq = new Faq
        {
            Question = dto.Question,
            Answer = dto.Answer,
            Category = dto.Category,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Faqs.Add(faq);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(faq.Id))!;
    }

    public async Task<FaqDto> UpdateAsync(long id, UpdateFaqDto dto)
    {
        var faq = await _context.Faqs.FindAsync(id);
        if (faq == null)
        {
            throw new KeyNotFoundException($"FAQ with id {id} not found");
        }

        if (dto.Question != null) faq.Question = dto.Question;
        if (dto.Answer != null) faq.Answer = dto.Answer;
        if (dto.Category != null) faq.Category = dto.Category;
        if (dto.DisplayOrder.HasValue) faq.DisplayOrder = dto.DisplayOrder.Value;
        if (dto.IsActive.HasValue) faq.IsActive = dto.IsActive.Value;

        faq.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var faq = await _context.Faqs.FindAsync(id);
        if (faq == null)
        {
            return false;
        }

        _context.Faqs.Remove(faq);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(long id)
    {
        var faq = await _context.Faqs.FindAsync(id);
        if (faq == null)
        {
            return false;
        }

        faq.IsActive = !faq.IsActive;
        faq.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static FaqDto MapToDto(Faq faq)
    {
        return new FaqDto
        {
            Id = faq.Id,
            Question = faq.Question,
            Answer = faq.Answer,
            Category = faq.Category,
            DisplayOrder = faq.DisplayOrder,
            IsActive = faq.IsActive,
            CreatedAt = faq.CreatedAt,
            UpdatedAt = faq.UpdatedAt
        };
    }
}
