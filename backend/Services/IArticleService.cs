using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Services;

public interface IArticleService
{
    Task<PagedResult<ArticleDto>> GetArticlesAsync(ArticleListRequest request);
    Task<ArticleDto?> GetArticleByIdAsync(int id);
    Task<ArticleDto> CreateArticleAsync(CreateArticleRequest request, int authorId);
    Task<ArticleDto?> UpdateArticleAsync(UpdateArticleRequest request);
    Task<bool> DeleteArticleAsync(int id);
    Task<bool> PublishArticleAsync(int id);
    Task<bool> UnpublishArticleAsync(int id);
    Task IncrementViewCountAsync(int id);
}