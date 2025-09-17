using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyCms.Api.Services;
using MyCms.Api.DTOs;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberListDto>>> GetMembers()
    {
        var members = await _memberService.GetAllAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetMember(int id)
    {
        var member = await _memberService.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound(new { message = "会员不存在" });
        }

        return Ok(member);
    }

    [HttpGet("code/{memberCode}")]
    public async Task<ActionResult<MemberDto>> GetMemberByCode(string memberCode)
    {
        var member = await _memberService.GetByMemberCodeAsync(memberCode);

        if (member == null)
        {
            return NotFound(new { message = "会员不存在" });
        }

        return Ok(member);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MemberListDto>>> SearchMembers(
        [FromQuery] string? keyword,
        [FromQuery] string? membershipType,
        [FromQuery] string? status)
    {
        var members = await _memberService.SearchAsync(keyword, membershipType, status);
        return Ok(members);
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> CreateMember(CreateMemberDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if member code already exists
        if (await _memberService.ExistsAsync(dto.MemberCode))
        {
            return BadRequest(new { message = "会员编号已存在" });
        }

        var member = await _memberService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetMember),
            new { id = member.Id },
            member);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MemberDto>> UpdateMember(int id, UpdateMemberDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var member = await _memberService.UpdateAsync(id, dto);

        if (member == null)
        {
            return NotFound(new { message = "会员不存在" });
        }

        return Ok(member);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMember(int id)
    {
        var result = await _memberService.DeleteAsync(id);

        if (!result)
        {
            return NotFound(new { message = "会员不存在" });
        }

        return Ok(new { message = "会员已删除" });
    }

    [HttpGet("generate-code")]
    public async Task<ActionResult<string>> GenerateMemberCode()
    {
        var memberCode = await _memberService.GenerateMemberCodeAsync();
        return Ok(new { memberCode });
    }

    [HttpPost("{id}/balance")]
    public async Task<ActionResult> UpdateBalance(int id, [FromBody] decimal amount)
    {
        var result = await _memberService.UpdateBalanceAsync(id, amount);

        if (!result)
        {
            return NotFound(new { message = "会员不存在" });
        }

        return Ok(new { message = "余额更新成功" });
    }

    [HttpPost("{id}/points")]
    public async Task<ActionResult> UpdatePoints(int id, [FromBody] int points)
    {
        var result = await _memberService.UpdatePointsAsync(id, points);

        if (!result)
        {
            return NotFound(new { message = "会员不存在" });
        }

        return Ok(new { message = "积分更新成功" });
    }

    [HttpPost("{id}/visit")]
    public async Task<ActionResult> UpdateLastVisit(int id)
    {
        var result = await _memberService.UpdateLastVisitAsync(id);

        if (!result)
        {
            return NotFound(new { message = "会员不存在" });
        }

        return Ok(new { message = "访问记录更新成功" });
    }
}