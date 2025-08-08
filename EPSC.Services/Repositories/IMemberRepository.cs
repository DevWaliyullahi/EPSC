using EPSC.Domain.Entities.Member;
using EPSC.Infrastructure.Configurations.Data;
using Microsoft.EntityFrameworkCore;

namespace EPSC.Services.Repositories
{
    public interface IMemberRepository
    {
        Task<TMember?> GetByIdAsync(Guid id);
        Task AddAsync(TMember member);
        void Update(TMember member);
        Task SaveChangesAsync();
        IQueryable<TMember> Query();
    }

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
    }
}
