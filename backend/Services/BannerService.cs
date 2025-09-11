using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Services;

public class BannerService : IBannerService
{
    private readonly CmsDbContext _context;

    public BannerService(CmsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BannerDto>> GetBannersAsync(BannerListRequest request)
    {
        var query = _context.Banners.AsQueryable();

        // Search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(b => b.Title.Contains(request.Search) ||
                                   b.Description!.Contains(request.Search));
        }

        // Active filter
        if (request.IsActive.HasValue)
        {
            query = query.Where(b => b.IsActive == request.IsActive);
        }

        var totalCount = await query.CountAsync();

        var banners = await query
            .OrderBy(b => b.SortOrder)
            .ThenByDescending(b => b.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BannerDto
            {
                Id = b.Id,
                Title = b.Title,
                ImageUrl = b.ImageUrl,
                LinkUrl = b.LinkUrl,
                LinkTarget = b.LinkTarget,
                Description = b.Description,
                SortOrder = b.SortOrder,
                IsActive = b.IsActive,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt ?? b.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<BannerDto>
        {
            Items = banners,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<List<BannerDto>> GetActiveBannersAsync()
    {
        var now = DateTime.UtcNow;
        
        return await _context.Banners
            .Where(b => b.IsActive && 
                       (b.StartTime == null || b.StartTime <= now) &&
                       (b.EndTime == null || b.EndTime >= now))
            .OrderBy(b => b.SortOrder)
            .Select(b => new BannerDto
            {
                Id = b.Id,
                Title = b.Title,
                ImageUrl = b.ImageUrl,
                LinkUrl = b.LinkUrl,
                LinkTarget = b.LinkTarget,
                Description = b.Description,
                SortOrder = b.SortOrder,
                IsActive = b.IsActive,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt ?? b.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<BannerDto?> GetBannerByIdAsync(int id)
    {
        var banner = await _context.Banners.FirstOrDefaultAsync(b => b.Id == id);
        if (banner == null) return null;

        return new BannerDto
        {
            Id = banner.Id,
            Title = banner.Title,
            ImageUrl = banner.ImageUrl,
            LinkUrl = banner.LinkUrl,
            LinkTarget = banner.LinkTarget,
            Description = banner.Description,
            SortOrder = banner.SortOrder,
            IsActive = banner.IsActive,
            StartTime = banner.StartTime,
            EndTime = banner.EndTime,
            CreatedAt = banner.CreatedAt,
            UpdatedAt = banner.UpdatedAt ?? banner.CreatedAt
        };
    }

    public async Task<BannerDto> CreateBannerAsync(CreateBannerRequest request)
    {
        var banner = new Banner
        {
            Title = request.Title,
            ImageUrl = request.ImageUrl,
            LinkUrl = request.LinkUrl,
            LinkTarget = request.LinkTarget,
            Description = request.Description,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        _context.Banners.Add(banner);
        await _context.SaveChangesAsync();

        return await GetBannerByIdAsync(banner.Id) ?? throw new Exception("创建横幅失败");
    }

    public async Task<BannerDto?> UpdateBannerAsync(UpdateBannerRequest request)
    {
        var banner = await _context.Banners.FindAsync(request.Id);
        if (banner == null) return null;

        banner.Title = request.Title;
        banner.ImageUrl = request.ImageUrl;
        banner.LinkUrl = request.LinkUrl;
        banner.LinkTarget = request.LinkTarget;
        banner.Description = request.Description;
        banner.SortOrder = request.SortOrder;
        banner.IsActive = request.IsActive;
        banner.StartTime = request.StartTime;
        banner.EndTime = request.EndTime;
        banner.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetBannerByIdAsync(banner.Id);
    }

    public async Task<bool> DeleteBannerAsync(int id)
    {
        var banner = await _context.Banners.FindAsync(id);
        if (banner == null) return false;

        _context.Banners.Remove(banner);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id)
    {
        var banner = await _context.Banners.FindAsync(id);
        if (banner == null) return false;

        banner.IsActive = !banner.IsActive;
        banner.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}