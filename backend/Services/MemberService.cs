using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.Models;
using MyCms.Api.DTOs;

namespace MyCms.Api.Services;

public class MemberService : IMemberService
{
    private readonly CmsDbContext _context;

    public MemberService(CmsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MemberListDto>> GetAllAsync()
    {
        return await _context.Members
            .Where(m => !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MemberListDto
            {
                Id = m.Id,
                MemberCode = m.MemberCode,
                Name = m.Name,
                Phone = m.Phone,
                Email = m.Email,
                MembershipType = m.MembershipType,
                Status = m.Status,
                JoinDate = m.JoinDate,
                ExpiryDate = m.ExpiryDate,
                Balance = m.Balance,
                Points = m.Points
            })
            .ToListAsync();
    }

    public async Task<MemberDto?> GetByIdAsync(int id)
    {
        var member = await _context.Members
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (member == null)
            return null;

        return MapToDto(member);
    }

    public async Task<MemberDto?> GetByMemberCodeAsync(string memberCode)
    {
        var member = await _context.Members
            .Where(m => m.MemberCode == memberCode && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (member == null)
            return null;

        return MapToDto(member);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
    {
        var member = new Member
        {
            MemberCode = dto.MemberCode,
            Name = dto.Name,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            IdNumber = dto.IdNumber,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            MembershipType = dto.MembershipType,
            Status = "Active",
            JoinDate = DateTime.Now,
            ExpiryDate = dto.ExpiryDate,
            Notes = dto.Notes,
            Avatar = dto.Avatar,
            Occupation = dto.Occupation,
            Company = dto.Company,
            EmergencyContact = dto.EmergencyContact,
            EmergencyPhone = dto.EmergencyPhone,
            Balance = dto.Balance ?? 0,
            Points = dto.Points,
            ReferralCode = dto.ReferralCode,
            ReferredBy = dto.ReferredBy
        };

        _context.Members.Add(member);
        await _context.SaveChangesAsync();

        return MapToDto(member);
    }

    public async Task<MemberDto?> UpdateAsync(int id, UpdateMemberDto dto)
    {
        var member = await _context.Members
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (member == null)
            return null;

        member.Name = dto.Name;
        member.Gender = dto.Gender;
        member.DateOfBirth = dto.DateOfBirth;
        member.IdNumber = dto.IdNumber;
        member.Phone = dto.Phone;
        member.Email = dto.Email;
        member.Address = dto.Address;
        member.MembershipType = dto.MembershipType;
        member.Status = dto.Status;
        member.ExpiryDate = dto.ExpiryDate;
        member.Notes = dto.Notes;
        member.Avatar = dto.Avatar;
        member.Occupation = dto.Occupation;
        member.Company = dto.Company;
        member.EmergencyContact = dto.EmergencyContact;
        member.EmergencyPhone = dto.EmergencyPhone;
        member.Balance = dto.Balance ?? member.Balance;
        member.Points = dto.Points;
        member.ReferralCode = dto.ReferralCode;
        member.ReferredBy = dto.ReferredBy;
        member.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return MapToDto(member);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var member = await _context.Members
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (member == null)
            return false;

        member.IsDeleted = true;
        member.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<MemberListDto>> SearchAsync(string? keyword, string? membershipType, string? status)
    {
        var query = _context.Members.Where(m => !m.IsDeleted);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(m =>
                m.Name.Contains(keyword) ||
                m.MemberCode.Contains(keyword) ||
                (m.Phone != null && m.Phone.Contains(keyword)) ||
                (m.Email != null && m.Email.Contains(keyword)));
        }

        if (!string.IsNullOrEmpty(membershipType))
        {
            query = query.Where(m => m.MembershipType == membershipType);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(m => m.Status == status);
        }

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MemberListDto
            {
                Id = m.Id,
                MemberCode = m.MemberCode,
                Name = m.Name,
                Phone = m.Phone,
                Email = m.Email,
                MembershipType = m.MembershipType,
                Status = m.Status,
                JoinDate = m.JoinDate,
                ExpiryDate = m.ExpiryDate,
                Balance = m.Balance,
                Points = m.Points
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string memberCode)
    {
        return await _context.Members
            .Where(m => m.MemberCode == memberCode && !m.IsDeleted)
            .AnyAsync();
    }

    public async Task<string> GenerateMemberCodeAsync()
    {
        string prefix = "M" + DateTime.Now.ToString("yyyyMM");
        int counter = 1;

        var lastMember = await _context.Members
            .Where(m => m.MemberCode.StartsWith(prefix))
            .OrderByDescending(m => m.MemberCode)
            .FirstOrDefaultAsync();

        if (lastMember != null && lastMember.MemberCode.Length > prefix.Length)
        {
            var lastNumber = lastMember.MemberCode.Substring(prefix.Length);
            if (int.TryParse(lastNumber, out int num))
            {
                counter = num + 1;
            }
        }

        return prefix + counter.ToString("D4");
    }

    public async Task<bool> UpdateBalanceAsync(int id, decimal amount)
    {
        var member = await _context.Members
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (member == null)
            return false;

        member.Balance = (member.Balance ?? 0) + amount;
        member.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdatePointsAsync(int id, int points)
    {
        var member = await _context.Members
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (member == null)
            return false;

        member.Points += points;
        member.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateLastVisitAsync(int id)
    {
        var member = await _context.Members
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (member == null)
            return false;

        member.LastVisitDate = DateTime.Now;
        member.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return true;
    }

    private static MemberDto MapToDto(Member member)
    {
        return new MemberDto
        {
            Id = member.Id,
            MemberCode = member.MemberCode,
            Name = member.Name,
            Gender = member.Gender,
            DateOfBirth = member.DateOfBirth,
            IdNumber = member.IdNumber,
            Phone = member.Phone,
            Email = member.Email,
            Address = member.Address,
            MembershipType = member.MembershipType,
            Status = member.Status,
            JoinDate = member.JoinDate,
            ExpiryDate = member.ExpiryDate,
            Notes = member.Notes,
            Avatar = member.Avatar,
            Occupation = member.Occupation,
            Company = member.Company,
            EmergencyContact = member.EmergencyContact,
            EmergencyPhone = member.EmergencyPhone,
            Balance = member.Balance,
            Points = member.Points,
            LastVisitDate = member.LastVisitDate,
            ReferralCode = member.ReferralCode,
            ReferredBy = member.ReferredBy,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt
        };
    }
}