using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Services;

public class ArticleService : IArticleService
{
    private readonly CmsDbContext _context;

    public ArticleService(CmsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ArticleDto>> GetArticlesAsync(ArticleListRequest request)
    {
        var query = _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .AsQueryable();

        // 搜索过滤
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(a => a.Title.Contains(request.Search) ||
                                   a.Summary!.Contains(request.Search));
        }

        // 状态过滤
        if (!string.IsNullOrEmpty(request.Status))
        {
            if (request.Status.ToLower() == "published")
                query = query.Where(a => a.IsPublished);
            else if (request.Status.ToLower() == "draft")
                query = query.Where(a => !a.IsPublished);
        }

        // 分类过滤
        if (request.CategoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == request.CategoryId);
        }

        var totalCount = await query.CountAsync();

        var articles = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                Content = a.Content,
                CoverImage = a.CoverImage,
                CategoryId = a.CategoryId,
                CategoryName = a.Category.Name,
                AuthorId = a.AuthorId,
                AuthorName = a.Author.Username,
                IsPublished = a.IsPublished,
                PublishedAt = a.PublishedAt,
                ViewCount = a.ViewCount,
                SortOrder = a.SortOrder,
                SeoTitle = a.SeoTitle,
                SeoDescription = a.SeoDescription,
                SeoKeywords = a.SeoKeywords,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt ?? a.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<ArticleDto>
        {
            Items = articles,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<ArticleDto?> GetArticleByIdAsync(int id)
    {
        var article = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null) return null;

        return new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Summary = article.Summary,
            Content = article.Content,
            CoverImage = article.CoverImage,
            CategoryId = article.CategoryId,
            CategoryName = article.Category.Name,
            AuthorId = article.AuthorId,
            AuthorName = article.Author.Username,
            IsPublished = article.IsPublished,
            PublishedAt = article.PublishedAt,
            ViewCount = article.ViewCount,
            SortOrder = article.SortOrder,
            SeoTitle = article.SeoTitle,
            SeoDescription = article.SeoDescription,
            SeoKeywords = article.SeoKeywords,
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt ?? article.CreatedAt
        };
    }

    public async Task<ArticleDto> CreateArticleAsync(CreateArticleRequest request, int authorId)
    {
        var article = new Article
        {
            Title = request.Title,
            Summary = request.Summary,
            Content = request.Content,
            CoverImage = request.CoverImage,
            CategoryId = request.CategoryId,
            AuthorId = authorId,
            IsPublished = request.IsPublished,
            PublishedAt = request.PublishedAt,
            SortOrder = request.SortOrder,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            SeoKeywords = request.SeoKeywords
        };

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        return await GetArticleByIdAsync(article.Id) ?? throw new Exception("创建文章失败");
    }

    public async Task<ArticleDto?> UpdateArticleAsync(UpdateArticleRequest request)
    {
        var article = await _context.Articles.FindAsync(request.Id);
        if (article == null) return null;

        article.Title = request.Title;
        article.Summary = request.Summary;
        article.Content = request.Content;
        article.CoverImage = request.CoverImage;
        article.CategoryId = request.CategoryId;
        article.IsPublished = request.IsPublished;
        article.PublishedAt = request.PublishedAt;
        article.SortOrder = request.SortOrder;
        article.SeoTitle = request.SeoTitle;
        article.SeoDescription = request.SeoDescription;
        article.SeoKeywords = request.SeoKeywords;
        article.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetArticleByIdAsync(article.Id);
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null) return false;

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishArticleAsync(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null) return false;

        article.IsPublished = true;
        article.PublishedAt = DateTime.UtcNow;
        article.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnpublishArticleAsync(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null) return false;

        article.IsPublished = false;
        article.PublishedAt = null;
        article.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var article = await _context.Articles
            .Where(a => a.Id == id && !a.IsDeleted)
            .FirstOrDefaultAsync();

        if (article != null)
        {
            article.ViewCount++;
            article.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}