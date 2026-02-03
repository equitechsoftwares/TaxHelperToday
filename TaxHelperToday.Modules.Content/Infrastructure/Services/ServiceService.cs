using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Modules.Content.Infrastructure.Services;

public class ServiceService : IServiceService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ServiceService> _logger;

    public ServiceService(ApplicationDbContext context, ILogger<ServiceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceDto?> GetByIdAsync(long id)
    {
        var service = await _context.Services.FindAsync(id);
        return service == null ? null : MapToDto(service);
    }

    public async Task<ServiceDto?> GetBySlugAsync(string slug)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Slug == slug);
        return service == null ? null : MapToDto(service);
    }

    public async Task<IEnumerable<ServiceDto>> GetAllAsync(bool activeOnly = false)
    {
        var query = _context.Services.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(s => s.IsActive);
        }

        var services = await query
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();

        return services.Select(MapToDto);
    }

    public async Task<IEnumerable<ServiceDto>> GetByTypeAsync(string type, bool activeOnly = false)
    {
        var query = _context.Services
            .Where(s => s.Type == type)
            .AsQueryable();

        if (activeOnly)
        {
            query = query.Where(s => s.IsActive);
        }

        var services = await query
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();

        return services.Select(MapToDto);
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto)
    {
        var service = new Service
        {
            Slug = dto.Slug,
            Name = dto.Name,
            Description = dto.Description,
            Content = dto.Content,
            Type = dto.Type,
            Level = dto.Level,
            Highlight = dto.Highlight,
            IconUrl = dto.IconUrl,
            EnquiryTitle = dto.EnquiryTitle,
            EnquirySubtitle = dto.EnquirySubtitle,
            EnquiryButtonText = dto.EnquiryButtonText,
            EnquiryNote = dto.EnquiryNote,
            IsActive = dto.IsActive,
            DisplayOrder = dto.DisplayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(service.Id))!;
    }

    public async Task<ServiceDto> UpdateAsync(long id, UpdateServiceDto dto)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with id {id} not found");
        }

        if (dto.Slug != null) service.Slug = dto.Slug;
        if (dto.Name != null) service.Name = dto.Name;
        if (dto.Description != null) service.Description = dto.Description;
        if (dto.Content != null) service.Content = dto.Content;
        if (dto.Type != null) service.Type = dto.Type;
        if (dto.Level != null) service.Level = dto.Level;
        if (dto.Highlight != null) service.Highlight = dto.Highlight;
        if (dto.IconUrl != null) service.IconUrl = dto.IconUrl;
        if (dto.EnquiryTitle != null) service.EnquiryTitle = dto.EnquiryTitle;
        if (dto.EnquirySubtitle != null) service.EnquirySubtitle = dto.EnquirySubtitle;
        if (dto.EnquiryButtonText != null) service.EnquiryButtonText = dto.EnquiryButtonText;
        if (dto.EnquiryNote != null) service.EnquiryNote = dto.EnquiryNote;
        if (dto.IsActive.HasValue) service.IsActive = dto.IsActive.Value;
        if (dto.DisplayOrder.HasValue) service.DisplayOrder = dto.DisplayOrder.Value;

        service.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
        {
            return false;
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(long id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
        {
            return false;
        }

        service.IsActive = !service.IsActive;
        service.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static ServiceDto MapToDto(Service service)
    {
        return new ServiceDto
        {
            Id = service.Id,
            Slug = service.Slug,
            Name = service.Name,
            Description = service.Description,
            Content = service.Content,
            Type = service.Type,
            Level = service.Level,
            Highlight = service.Highlight,
            IconUrl = service.IconUrl,
            EnquiryTitle = service.EnquiryTitle,
            EnquirySubtitle = service.EnquirySubtitle,
            EnquiryButtonText = service.EnquiryButtonText,
            EnquiryNote = service.EnquiryNote,
            IsActive = service.IsActive,
            DisplayOrder = service.DisplayOrder,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }
}
