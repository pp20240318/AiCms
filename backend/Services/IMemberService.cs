using MyCms.Api.Models;
using MyCms.Api.DTOs;

namespace MyCms.Api.Services;

public interface IMemberService
{
    Task<IEnumerable<MemberListDto>> GetAllAsync();
    Task<object> GetPagedAsync(int page, int pageSize, string? keyword, string? membershipType, string? status);
    Task<MemberDto?> GetByIdAsync(int id);
    Task<MemberDto?> GetByMemberCodeAsync(string memberCode);
    Task<MemberDto> CreateAsync(CreateMemberDto dto);
    Task<MemberDto?> UpdateAsync(int id, UpdateMemberDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<MemberListDto>> SearchAsync(string? keyword, string? membershipType, string? status);
    Task<bool> ExistsAsync(string memberCode);
    Task<string> GenerateMemberCodeAsync();
    Task<bool> UpdateBalanceAsync(int id, decimal amount);
    Task<bool> UpdatePointsAsync(int id, int points);
    Task<bool> UpdateLastVisitAsync(int id);
}