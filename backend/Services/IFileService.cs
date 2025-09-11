using MyCms.Api.DTOs;

namespace MyCms.Api.Services;

public interface IFileService
{
    Task<UploadResponse> UploadFileAsync(IFormFile file, int? userId = null);
    Task<PagedResult<UploadedFileDto>> GetFilesAsync(FileListRequest request);
    Task<UploadedFileDto?> GetFileByIdAsync(int id);
    Task<bool> DeleteFileAsync(int id);
    Task<List<UploadedFileDto>> GetRecentFilesAsync(int userId, int count = 10);
}