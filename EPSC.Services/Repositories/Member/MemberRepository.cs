using EPSC.Application.DTOs.Member;
using EPSC.Domain.Entities.Member;
using EPSC.Infrastructure.Configurations.Data;
using EPSC.Utility.Pagination;
using Microsoft.EntityFrameworkCore;

namespace EPSC.Services.Repositories.Member
{
    public class MemberRepository : IMemberRepository
    {
        private readonly EPSCDbContext _context;

        public MemberRepository(EPSCDbContext context)
        {
            _context = context;
        }

        public async Task<TMember?> GetByIdAsync(Guid id) =>
            await _context.TMembers.FindAsync(id);

        public async Task AddAsync(TMember member) =>
            await _context.TMembers.AddAsync(member);

        public void Update(TMember member) =>
            _context.TMembers.Update(member);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public IQueryable<TMember> Query() =>
            _context.TMembers.AsNoTracking();

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeMemberId = null)
        {
            var query = _context.TMembers.AsNoTracking()
                .Where(m => m.Email == email && !m.IsDeleted);

            if (excludeMemberId.HasValue)
                query = query.Where(m => m.MemberId != excludeMemberId.Value);

            return await query.AnyAsync();
        }

        public async Task<PagedResponse<MemberViewModel>> GetMembersPagedAsync(MemberSearchDto searchDto)
        {
            var query = _context.TMembers.Where(m => !m.IsDeleted);

            // Apply name filter
            if (!string.IsNullOrWhiteSpace(searchDto.Name))
            {
                var nameFilter = searchDto.Name.Trim().ToLower();
                query = query.Where(m =>
                    (m.FirstName + " " + m.LastName).ToLower().Contains(nameFilter) ||
                    m.FirstName.ToLower().Contains(nameFilter) ||
                    m.LastName.ToLower().Contains(nameFilter));
            }

            // Apply status filter
            if (searchDto.Status.HasValue)
            {
                query = query.Where(m => m.Status == searchDto.Status.Value);
            }

            // Project to ViewModel
            var projectedQuery = query.Select(m => new MemberViewModel
            {
                MemberId = m.MemberId,
                FullName = m.FirstName + " " + m.LastName,
                Email = m.Email,
                Status = m.Status,
                PhoneNumber = m.PhoneNumber,
                DateOfBirth = m.DateOfBirth,
                CreatedAt = m.CreatedAt
            });

            // Apply pagination
            return await projectedQuery.ToPagedResponseAsync(searchDto.PageNumber, searchDto.PageSize);
        }
    }
}
