using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;
using System.Security.Cryptography;
using SixLabors.ImageSharp;

namespace MyCms.Api.Services;

public class FileService : IFileService
{
    private readonly CmsDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public FileService(CmsDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _context = context;
        _environment = environment;
        _configuration = configuration;
    }

    public async Task<UploadResponse> UploadFileAsync(IFormFile file, int? userId = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided");

        // Validate file size (default 10MB)
        var maxFileSize = _configuration.GetValue<long>("FileUpload:MaxFileSizeBytes", 10485760);
        if (file.Length > maxFileSize)
            throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / 1024 / 1024}MB");

        // Validate file type
        var allowedTypes = _configuration.GetSection("FileUpload:AllowedTypes").Get<string[]>() 
            ?? new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "application/pdf", "text/plain" };
        
        if (!allowedTypes.Contains(file.ContentType))
            throw new ArgumentException($"File type {file.ContentType} is not allowed");

        // Generate unique filename
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        
        // Create upload directory if it doesn't exist
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
        var yearMonth = DateTime.UtcNow.ToString("yyyy/MM");
        var fullUploadPath = Path.Combine(uploadPath, yearMonth);
        
        if (!Directory.Exists(fullUploadPath))
        {
            Directory.CreateDirectory(fullUploadPath);
        }

        var filePath = Path.Combine(fullUploadPath, uniqueFileName);
        var relativePath = $"uploads/{yearMonth}/{uniqueFileName}".Replace("\\", "/");

        // Calculate file hash
        string fileHash;
        using (var stream = file.OpenReadStream())
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = await sha256.ComputeHashAsync(stream);
                fileHash = Convert.ToHexString(hashBytes).ToLowerInvariant();
            }
        }

        // Check for duplicate files
        var existingFile = await _context.UploadedFiles
            .FirstOrDefaultAsync(f => f.FileHash == fileHash);
        
        if (existingFile != null)
        {
            // Return existing file info
            var existingDto = new UploadedFileDto
            {
                Id = existingFile.Id,
                OriginalName = existingFile.OriginalName,
                FileName = existingFile.FileName,
                FilePath = existingFile.FilePath,
                FileUrl = $"/{existingFile.FilePath}",
                ContentType = existingFile.ContentType,
                FileSize = existingFile.FileSize,
                FileHash = existingFile.FileHash,
                Width = existingFile.Width,
                Height = existingFile.Height,
                UserId = existingFile.UserId,
                CreatedAt = existingFile.CreatedAt,
                UpdatedAt = existingFile.UpdatedAt ?? existingFile.CreatedAt
            };

            return new UploadResponse
            {
                Url = existingDto.FileUrl,
                FileName = existingFile.FileName,
                FileSize = existingFile.FileSize,
                Width = existingFile.Width,
                Height = existingFile.Height,
                FileInfo = existingDto
            };
        }

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Get image dimensions if it's an image
        int? width = null, height = null;
        if (file.ContentType.StartsWith("image/"))
        {
            try
            {
                using (var image = Image.Load(filePath))
                {
                    width = image.Width;
                    height = image.Height;
                }
            }
            catch
            {
                // Ignore errors getting image dimensions
            }
        }

        // Save file info to database
        var uploadedFile = new UploadedFile
        {
            OriginalName = file.FileName,
            FileName = uniqueFileName,
            FilePath = relativePath,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileHash = fileHash,
            Width = width,
            Height = height,
            UserId = userId
        };

        _context.UploadedFiles.Add(uploadedFile);
        await _context.SaveChangesAsync();

        var fileDto = new UploadedFileDto
        {
            Id = uploadedFile.Id,
            OriginalName = uploadedFile.OriginalName,
            FileName = uploadedFile.FileName,
            FilePath = uploadedFile.FilePath,
            FileUrl = $"/{uploadedFile.FilePath}",
            ContentType = uploadedFile.ContentType,
            FileSize = uploadedFile.FileSize,
            FileHash = uploadedFile.FileHash,
            Width = uploadedFile.Width,
            Height = uploadedFile.Height,
            UserId = uploadedFile.UserId,
            CreatedAt = uploadedFile.CreatedAt,
            UpdatedAt = uploadedFile.UpdatedAt ?? uploadedFile.CreatedAt
        };

        return new UploadResponse
        {
            Url = fileDto.FileUrl,
            FileName = uploadedFile.FileName,
            FileSize = uploadedFile.FileSize,
            Width = uploadedFile.Width,
            Height = uploadedFile.Height,
            FileInfo = fileDto
        };
    }

    public async Task<PagedResult<UploadedFileDto>> GetFilesAsync(FileListRequest request)
    {
        var query = _context.UploadedFiles
            .Include(f => f.User)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(f => f.OriginalName.Contains(request.Search) ||
                                   f.FileName.Contains(request.Search));
        }

        // Content type filter
        if (!string.IsNullOrEmpty(request.ContentType))
        {
            if (request.ContentType == "image")
                query = query.Where(f => f.ContentType.StartsWith("image/"));
            else if (request.ContentType == "document")
                query = query.Where(f => !f.ContentType.StartsWith("image/"));
            else
                query = query.Where(f => f.ContentType == request.ContentType);
        }

        // User filter
        if (request.UserId.HasValue)
        {
            query = query.Where(f => f.UserId == request.UserId);
        }

        var totalCount = await query.CountAsync();

        var files = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new UploadedFileDto
            {
                Id = f.Id,
                OriginalName = f.OriginalName,
                FileName = f.FileName,
                FilePath = f.FilePath,
                FileUrl = $"/{f.FilePath}",
                ContentType = f.ContentType,
                FileSize = f.FileSize,
                FileHash = f.FileHash,
                Width = f.Width,
                Height = f.Height,
                UserId = f.UserId,
                UserName = f.User != null ? f.User.Username : null,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt ?? f.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<UploadedFileDto>
        {
            Items = files,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<UploadedFileDto?> GetFileByIdAsync(int id)
    {
        var file = await _context.UploadedFiles
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (file == null) return null;

        return new UploadedFileDto
        {
            Id = file.Id,
            OriginalName = file.OriginalName,
            FileName = file.FileName,
            FilePath = file.FilePath,
            FileUrl = $"/{file.FilePath}",
            ContentType = file.ContentType,
            FileSize = file.FileSize,
            FileHash = file.FileHash,
            Width = file.Width,
            Height = file.Height,
            UserId = file.UserId,
            UserName = file.User?.Username,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt ?? file.CreatedAt
        };
    }

    public async Task<bool> DeleteFileAsync(int id)
    {
        var file = await _context.UploadedFiles.FindAsync(id);
        if (file == null) return false;

        // Delete physical file
        var physicalPath = Path.Combine(_environment.WebRootPath, file.FilePath);
        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }

        _context.UploadedFiles.Remove(file);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<UploadedFileDto>> GetRecentFilesAsync(int userId, int count = 10)
    {
        return await _context.UploadedFiles
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Take(count)
            .Select(f => new UploadedFileDto
            {
                Id = f.Id,
                OriginalName = f.OriginalName,
                FileName = f.FileName,
                FilePath = f.FilePath,
                FileUrl = $"/{f.FilePath}",
                ContentType = f.ContentType,
                FileSize = f.FileSize,
                Width = f.Width,
                Height = f.Height,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync();
    }
}