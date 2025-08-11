using EPSC.Application.DTOs.Member;
using EPSC.Domain.Entities.Member;
using EPSC.Infrastructure.Configurations.Data;
using EPSC.Utility.Pagination;
using Microsoft.EntityFrameworkCore;

namespace EPSC.Services.Repositories.Member
{
    public interface IMemberRepository
    {
        Task<TMember?> GetByIdAsync(Guid id);
        Task AddAsync(TMember member);
        void Update(TMember member);
        Task SaveChangesAsync();
        IQueryable<TMember> Query();
        Task<bool> EmailExistsAsync(string email, Guid? excludeMemberId = null);
        Task<PagedResponse<MemberViewModel>> GetMembersPagedAsync(MemberSearchDto searchDto);

    }


}
