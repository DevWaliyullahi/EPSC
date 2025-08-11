using EPSC.Application.Dto.Contribution;
using EPSC.Application.DTOs.Contribution;
using EPSC.Application.Interfaces.IContributionService;
using Microsoft.AspNetCore.Mvc;

namespace EPSC.API.Controllers.ContributionsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionsController : BaseController
    {
        private readonly IContributionService _contributionService;
        private readonly ILogger<ContributionsController> _logger;

        public ContributionsController(
            IContributionService contributionService,
            ILogger<ContributionsController> logger)
        {
            _contributionService = contributionService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new contribution in the system.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateContribution([FromBody] ContributionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return NotifyModelStateError();

            var result = await _contributionService.CreateContributionAsync(dto);
            return Response(result);
        }

        /// <summary>
        /// Gets a contribution by its unique identifier.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContribution(Guid id)
        {
            var result = await _contributionService.GetContributionAsync(id);
            return Response(result);
        }

        /// <summary>
        /// Retrieves all contributions with optional filtering and pagination.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllContributions([FromQuery] ContributionSearchDto payload)
        {
            var result = await _contributionService.GetAllContributionsAsync(payload);
            return Response(result);
        }

        /// <summary>
        /// Gets contributions for a specific member.
        /// </summary>
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetMemberContributions(Guid memberId)
        {
            var result = await _contributionService.GetMemberContributionsAsync(memberId);
            return Response(result);
        }

        /// <summary>
        /// Gets contribution summary for a member.
        /// </summary>
        [HttpGet("member/{memberId}/summary")]
        public async Task<IActionResult> GetMemberContributionSummary(Guid memberId)
        {
            var result = await _contributionService.GetMemberSummaryAsync(memberId);
            return Response(result);
        }

        /// <summary>
        /// Updates an existing contribution's information.
        /// </summary>
        [HttpPut("{contributionId}")]
        public async Task<IActionResult> UpdateContribution(Guid contributionId, [FromBody] ContributionUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return NotifyModelStateError();

            var result = await _contributionService.UpdateContributionAsync(contributionId, dto);
            return Response(result);
        }

        /// <summary>
        /// Soft deletes a contribution.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteContribution(Guid id)
        {
            var result = await _contributionService.SoftDeleteContributionAsync(id);
            return Response(result);
        }

        /// <summary>
        /// Validates a contribution.
        /// </summary>
        [HttpPost("{id}/validate")]
        public async Task<IActionResult> ValidateContribution(Guid id, [FromBody] string? notes = null)
        {
            var result = await _contributionService.ValidateContributionAsync(id, notes);
            return Response(result);
        }

        /// <summary>
        /// Checks if member can make monthly contribution.
        /// </summary>
        [HttpGet("member/{memberId}/can-contribute-monthly")]
        public async Task<IActionResult> CanMakeMonthlyContribution(Guid memberId, [FromQuery] DateTime contributionDate)
        {
            var result = await _contributionService.CanMakeMonthlyContributionAsync(memberId, contributionDate);
            return Response(result);
        }
    }
}
