using MyCms.Api.DTOs;

namespace MyCms.Api.Services;

public interface IBannerService
{
    Task<PagedResult<BannerDto>> GetBannersAsync(BannerListRequest request);
    Task<List<BannerDto>> GetActiveBannersAsync();
    Task<BannerDto?> GetBannerByIdAsync(int id);
    Task<BannerDto> CreateBannerAsync(CreateBannerRequest request);
    Task<BannerDto?> UpdateBannerAsync(UpdateBannerRequest request);
    Task<bool> DeleteBannerAsync(int id);
    Task<bool> ToggleActiveAsync(int id);
}