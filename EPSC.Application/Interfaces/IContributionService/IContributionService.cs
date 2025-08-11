using EPSC.Application.Dto.Contribution;
using EPSC.Application.DTOs.Contribution;
using EPSC.Utility.Pagination;
using RPNL.Net.Utilities.ResponseUtil;

namespace EPSC.Application.Interfaces.IContributionService
{
    public interface IContributionService
    {
        Task<ResponseModel<ContributionViewModel?>> GetContributionAsync(Guid id);
        Task<ResponseModel<PagedResponse<ContributionViewModel>>> GetAllContributionsAsync(ContributionSearchDto searchDto);
        Task<ResponseModel<IEnumerable<ContributionViewModel>>> GetMemberContributionsAsync(Guid memberId);
        Task<ResponseModel<ContributionViewModel>> CreateContributionAsync(ContributionCreateDto dto);
        Task<ResponseModel<ContributionViewModel?>> UpdateContributionAsync(Guid id, ContributionUpdateDto dto);
        Task<ResponseModel<bool>> SoftDeleteContributionAsync(Guid id);
        Task<ResponseModel<ContributionSummaryDto>> GetMemberSummaryAsync(Guid memberId);
        Task<ResponseModel<bool>> ValidateContributionAsync(Guid contributionId, string? notes = null);
        Task<ResponseModel<bool>> CanMakeMonthlyContributionAsync(Guid memberId, DateTime contributionDate);
    }
}
