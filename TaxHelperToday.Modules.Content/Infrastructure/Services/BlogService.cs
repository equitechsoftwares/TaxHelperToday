using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Modules.Content.Infrastructure.Services;

public class BlogService : IBlogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BlogService> _logger;

    public BlogService(ApplicationDbContext context, ILogger<BlogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BlogPostDto?> GetByIdAsync(long id)
    {
        var blogPost = await _context.BlogPosts
            .Include(bp => bp.BlogPostTags)
                .ThenInclude(bpt => bpt.Tag)
            .FirstOrDefaultAsync(bp => bp.Id == id);

        return blogPost == null ? null : MapToDto(blogPost);
    }

    public async Task<BlogPostDto?> GetBySlugAsync(string slug)
    {
        var blogPost = await _context.BlogPosts
            .Include(bp => bp.BlogPostTags)
                .ThenInclude(bpt => bpt.Tag)
            .FirstOrDefaultAsync(bp => bp.Slug == slug);

        return blogPost == null ? null : MapToDto(blogPost);
    }

    public async Task<IEnumerable<BlogPostDto>> GetAllAsync(bool publishedOnly = false)
    {
        var query = _context.BlogPosts
            .Include(bp => bp.BlogPostTags)
                .ThenInclude(bpt => bpt.Tag)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(bp => bp.IsPublished && bp.PublishedAt <= DateTime.UtcNow);
        }

        var blogPosts = await query
            .OrderByDescending(bp => bp.CreatedAt)
            .ToListAsync();

        return blogPosts.Select(MapToDto);
    }

    public async Task<IEnumerable<BlogPostDto>> GetByCategoryAsync(string category, bool publishedOnly = false)
    {
        var query = _context.BlogPosts
            .Include(bp => bp.BlogPostTags)
                .ThenInclude(bpt => bpt.Tag)
            .Where(bp => bp.Category == category)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(bp => bp.IsPublished && bp.PublishedAt <= DateTime.UtcNow);
        }

        var blogPosts = await query
            .OrderByDescending(bp => bp.CreatedAt)
            .ToListAsync();

        return blogPosts.Select(MapToDto);
    }

    public async Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto, long userId)
    {
        var blogPost = new BlogPost
        {
            Slug = dto.Slug,
            Title = dto.Title,
            Excerpt = dto.Excerpt,
            Content = dto.Content,
            Category = dto.Category,
            ReadTime = dto.ReadTime,
            FeaturedImageUrl = dto.FeaturedImageUrl,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            IsPublished = dto.IsPublished,
            PublishedAt = dto.IsPublished ? DateTime.UtcNow : null,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync();

        // Handle tags
        if (dto.Tags.Any())
        {
            await AddTagsToBlogPostAsync(blogPost.Id, dto.Tags);
        }

        return (await GetByIdAsync(blogPost.Id))!;
    }

    public async Task<BlogPostDto> UpdateAsync(long id, UpdateBlogPostDto dto, long userId)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            throw new KeyNotFoundException($"Blog post with id {id} not found");
        }

        if (dto.Slug != null) blogPost.Slug = dto.Slug;
        if (dto.Title != null) blogPost.Title = dto.Title;
        if (dto.Excerpt != null) blogPost.Excerpt = dto.Excerpt;
        if (dto.Content != null) blogPost.Content = dto.Content;
        if (dto.Category != null) blogPost.Category = dto.Category;
        if (dto.ReadTime != null) blogPost.ReadTime = dto.ReadTime;
        if (dto.FeaturedImageUrl != null) blogPost.FeaturedImageUrl = dto.FeaturedImageUrl;
        if (dto.MetaDescription != null) blogPost.MetaDescription = dto.MetaDescription;
        if (dto.MetaKeywords != null) blogPost.MetaKeywords = dto.MetaKeywords;
        if (dto.IsPublished.HasValue)
        {
            blogPost.IsPublished = dto.IsPublished.Value;
            if (dto.IsPublished.Value && blogPost.PublishedAt == null)
            {
                blogPost.PublishedAt = DateTime.UtcNow;
            }
        }

        blogPost.UpdatedBy = userId;
        blogPost.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Handle tags
        if (dto.Tags != null)
        {
            // Remove existing tags
            var existingTags = await _context.BlogPostTags
                .Where(bpt => bpt.BlogPostId == id)
                .ToListAsync();
            _context.BlogPostTags.RemoveRange(existingTags);
            await _context.SaveChangesAsync();

            // Add new tags
            await AddTagsToBlogPostAsync(id, dto.Tags);
        }

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            return false;
        }

        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishAsync(long id, long userId)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            return false;
        }

        blogPost.IsPublished = true;
        blogPost.PublishedAt = DateTime.UtcNow;
        blogPost.UpdatedBy = userId;
        blogPost.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnpublishAsync(long id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            return false;
        }

        blogPost.IsPublished = false;
        blogPost.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task IncrementViewCountAsync(long id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost != null)
        {
            blogPost.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }

    private async Task AddTagsToBlogPostAsync(long blogPostId, List<string> tagNames)
    {
        foreach (var tagName in tagNames)
        {
            var slug = GenerateSlug(tagName);
            var tag = await _context.BlogTags
                .FirstOrDefaultAsync(t => t.Slug == slug);

            if (tag == null)
            {
                tag = new BlogTag
                {
                    Name = tagName,
                    Slug = slug,
                    CreatedAt = DateTime.UtcNow
                };
                _context.BlogTags.Add(tag);
                await _context.SaveChangesAsync();
            }

            // Check if relationship already exists
            var exists = await _context.BlogPostTags
                .AnyAsync(bpt => bpt.BlogPostId == blogPostId && bpt.TagId == tag.Id);

            if (!exists)
            {
                _context.BlogPostTags.Add(new BlogPostTag
                {
                    BlogPostId = blogPostId,
                    TagId = tag.Id
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    private static string GenerateSlug(string text)
    {
        return text.ToLower()
            .Replace(" ", "-")
            .Replace("&", "and");
    }

    private static BlogPostDto MapToDto(BlogPost blogPost)
    {
        return new BlogPostDto
        {
            Id = blogPost.Id,
            Slug = blogPost.Slug,
            Title = blogPost.Title,
            Excerpt = blogPost.Excerpt,
            Content = blogPost.Content,
            Category = blogPost.Category,
            ReadTime = blogPost.ReadTime,
            FeaturedImageUrl = blogPost.FeaturedImageUrl,
            MetaDescription = blogPost.MetaDescription,
            MetaKeywords = blogPost.MetaKeywords,
            IsPublished = blogPost.IsPublished,
            PublishedAt = blogPost.PublishedAt,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt,
            ViewCount = blogPost.ViewCount,
            Tags = blogPost.BlogPostTags.Select(bpt => bpt.Tag.Name).ToList()
        };
    }
}
