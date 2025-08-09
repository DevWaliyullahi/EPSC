using EPSC.Application.DTOs.Member;
using EPSC.Application.Interfaces.IMemberService;
using Microsoft.AspNetCore.Mvc;

namespace EPSC.API.Controllers.MembersController
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : BaseController
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService) => _memberService = memberService;

        /// <summary>
        /// Creates a new member in the system.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] MemberCreateDto dto)
        {
            if (!ModelState.IsValid)
                return NotifyModelStateError();

            var result = await _memberService.CreateMemberAsync(dto);
            return Response(result); // Use BaseController.Response() for consistent handling
        }

        /// <summary>
        /// Gets a member by their unique identifier.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var result = await _memberService.GetMemberAsync(id);
            return Response(result);
        }

        /// <summary>
        /// Retrieves all members in the system with optional filtering and pagination.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMembers([FromQuery] MemberSearchDto payload)
        {
            var result = await _memberService.GetAllMembersAsync(payload);
            return Response(result);
        }

        /// <summary>
        /// Updates an existing member's information.
        /// </summary>
        [HttpPut("{memberId}")]
        public async Task<IActionResult> UpdateMember(Guid memberId, [FromBody] MemberUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return NotifyModelStateError();

            var result = await _memberService.UpdateMemberAsync(memberId, dto);
            return Response(result); // Use BaseController.Response() for consistent handling
        }

        /// <summary>
        /// Soft deletes a member.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteMember(Guid id)
        {
            var result = await _memberService.SoftDeleteMemberAsync(id);
            return Response(result); // Use BaseController.Response() for consistent handling
        }
    }
}