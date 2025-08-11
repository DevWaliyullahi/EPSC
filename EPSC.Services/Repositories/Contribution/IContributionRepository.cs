using EPSC.Application.DTOs.Contribution;
using EPSC.Domain.Entities.Contribution;
using EPSC.Utility.Pagination;

namespace EPSC.Services.Repositories.Contribution
{
    public interface IContributionRepository
    {
        Task<TContribution?> GetByIdAsync(Guid id);
        Task<IEnumerable<TContribution>> GetByMemberIdAsync(Guid memberId);
        Task<TContribution?> GetMonthlyContributionAsync(Guid memberId, DateTime month);
        Task<PagedResponse<TContribution>> GetPagedAsync(ContributionSearchDto searchDto);
        Task<TContribution> CreateAsync(TContribution contribution);
        Task<TContribution> UpdateAsync(TContribution contribution);
        Task<bool> DeleteAsync(Guid id);
        Task<decimal> GetTotalContributionsAsync(Guid memberId);
        Task<int> GetValidContributionCountAsync(Guid memberId);
        Task<bool> HasMonthlyContributionAsync(Guid memberId, DateTime contributionDate);
    }
}
