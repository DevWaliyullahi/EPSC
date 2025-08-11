using EPSC.Application.DTOs.Contribution;
using EPSC.Domain.Entities.Contribution;
using EPSC.Infrastructure.Configurations.Data;
using EPSC.Utility.Enums;
using EPSC.Utility.Pagination;
using Microsoft.EntityFrameworkCore;

namespace EPSC.Services.Repositories.Contribution
{
    public class ContributionRepository : IContributionRepository
    {
        private readonly EPSCDbContext _context;

        public ContributionRepository(EPSCDbContext context)
        {
            _context = context;
        }

        public async Task<TContribution?> GetByIdAsync(Guid id)
        {
            return await _context.TContributions
                .Include(c => c.Member)
                .Include(c => c.Employer)
                .FirstOrDefaultAsync(c => c.ContributionId == id);
        }

        public async Task<IEnumerable<TContribution>> GetByMemberIdAsync(Guid memberId)
        {
            return await _context.TContributions
                .Where(c => c.MemberId == memberId)
                .OrderByDescending(c => c.ContributionDate)
                .ToListAsync();
        }

        public async Task<TContribution?> GetMonthlyContributionAsync(Guid memberId, DateTime month)
        {
            return await _context.TContributions
                .FirstOrDefaultAsync(c => c.MemberId == memberId
                    && c.ContributionType == ContributionType.Monthly
                    && c.ContributionDate.Year == month.Year
                    && c.ContributionDate.Month == month.Month);
        }

        public async Task<PagedResponse<TContribution>> GetPagedAsync(ContributionSearchDto searchDto)
        {
            var query = _context.TContributions.AsQueryable();

            // Apply filters
            if (searchDto.MemberId.HasValue)
                query = query.Where(c => c.MemberId == searchDto.MemberId.Value);

            if (searchDto.EmployerId.HasValue)
                query = query.Where(c => c.EmployerId == searchDto.EmployerId.Value);

            if (searchDto.ContributionType.HasValue)
                query = query.Where(c => c.ContributionType == searchDto.ContributionType.Value);

            if (searchDto.IsValidated.HasValue)
                query = query.Where(c => c.IsValidated == searchDto.IsValidated.Value);

            if (searchDto.FromDate.HasValue)
                query = query.Where(c => c.ContributionDate >= searchDto.FromDate.Value);

            if (searchDto.ToDate.HasValue)
                query = query.Where(c => c.ContributionDate <= searchDto.ToDate.Value);

            if (searchDto.MinAmount.HasValue)
                query = query.Where(c => c.Amount >= searchDto.MinAmount.Value);

            if (searchDto.MaxAmount.HasValue)
                query = query.Where(c => c.Amount <= searchDto.MaxAmount.Value);

            query = query.OrderByDescending(c => c.ContributionDate);

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var contributions = await query
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Include(c => c.Member)
                .Include(c => c.Employer)
                .ToListAsync();

            return new PagedResponse<TContribution>(
                contributions,
                totalCount,
                searchDto.PageNumber,
                searchDto.PageSize
            );
        }

        public async Task<TContribution> CreateAsync(TContribution contribution)
        {
            _context.TContributions.Add(contribution);
            await _context.SaveChangesAsync();
            return contribution;
        }

        public async Task<TContribution> UpdateAsync(TContribution contribution)
        {
            _context.TContributions.Update(contribution);
            await _context.SaveChangesAsync();
            return contribution;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var contribution = await GetByIdAsync(id);
            if (contribution == null) return false;

            contribution.IsDeleted = true;
            contribution.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalContributionsAsync(Guid memberId)
        {
            return await _context.TContributions
                .Where(c => c.MemberId == memberId && c.IsValidated)
                .SumAsync(c => c.Amount);
        }

        public async Task<int> GetValidContributionCountAsync(Guid memberId)
        {
            return await _context.TContributions
                .Where(c => c.MemberId == memberId && c.IsValidated)
                .CountAsync();
        }

        public async Task<bool> HasMonthlyContributionAsync(Guid memberId, DateTime contributionDate)
        {
            return await _context.TContributions
                .AnyAsync(c => c.MemberId == memberId
                    && c.ContributionType == ContributionType.Monthly
                    && c.ContributionDate.Year == contributionDate.Year
                    && c.ContributionDate.Month == contributionDate.Month);
        }
    }

}
