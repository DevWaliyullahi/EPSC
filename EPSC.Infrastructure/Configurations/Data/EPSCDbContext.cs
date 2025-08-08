using EPSC.Infrastructure.Identity.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EPSC.Domain.Entities.Member;


namespace EPSC.Infrastructure.Configurations.Data
{
    public class EPSCDbContext : IdentityDbContext<EPSAuthUser, EPSAuthRole, string>
    {
        public EPSCDbContext(DbContextOptions<EPSCDbContext> options) : base(options) { }

        //  DbSets
        public DbSet<TMember> TMembers { get; set; }
       // public DbSet<TContribution> TContributions { get; set; }
        // ...
    }
}
