using EPSC.Infrastructure.Identity.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EPSC.Domain.Entities.Member;
using EPSC.Domain.Entities.Contribution;
using EPSC.Domain.Entities;
using EPSC.Domain.Entities.Employer;
using EPSC.Domain.Entities.BenefitEligibility;
using EPSC.Domain.Entities.TransactionLog;


namespace EPSC.Infrastructure.Configurations.Data
{
    public class EPSCDbContext : IdentityDbContext<EPSAuthUser, EPSAuthRole, string>
    {
        public EPSCDbContext(DbContextOptions<EPSCDbContext> options) : base(options) { }

        //  DbSets
        public DbSet<TMember> TMembers { get; set; }
        public DbSet<TContribution> TContributions { get; set; }
        public DbSet<TEmployer> TEmployers { get; set; } = null!;
        public DbSet<TBenefitEligibility> TBenefitEligibilities { get; set; } = null!;
        public DbSet<TTransactionLog> TTransactionLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TMember
            modelBuilder.Entity<TMember>(entity =>
            {
                entity.HasKey(e => e.MemberId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(55);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(55);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.Gender).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Status).HasConversion<string>();

                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure TContribution
            modelBuilder.Entity<TContribution>(entity =>
            {
                entity.HasKey(e => e.ContributionId);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ContributionDate).IsRequired();
                entity.Property(e => e.ContributionType).HasConversion<string>();
                entity.Property(e => e.IsValidated).HasDefaultValue(false);

                // Foreign key relationships
                entity.HasOne<TMember>()
                      .WithMany()
                      .HasForeignKey(e => e.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<TEmployer>()
                      .WithMany()
                      .HasForeignKey(e => e.EmployerId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);

                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure TEmployer
            modelBuilder.Entity<TEmployer>(entity =>
            {
                entity.HasKey(e => e.EmployerId);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.RCNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(500);

                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure TBenefitEligibility
            modelBuilder.Entity<TBenefitEligibility>(entity =>
            {
                entity.HasKey(e => e.BenefitEligibilityId);
                entity.Property(e => e.IsEligible).IsRequired();
                entity.Property(e => e.EligibilityDate).IsRequired();

                entity.HasOne<TMember>()
                      .WithMany()
                      .HasForeignKey(e => e.MemberId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TTransactionLog
            modelBuilder.Entity<TTransactionLog>(entity =>
            {
                entity.HasKey(e => e.TransactionLogId);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(255);
                entity.Property(e => e.EntityId).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.UserId).HasMaxLength(255);
            });

            // Add unique constraints
            modelBuilder.Entity<TMember>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<TEmployer>()
                .HasIndex(e => e.RCNumber)
                .IsUnique();

            // Add composite index for monthly contribution validation
            modelBuilder.Entity<TContribution>()
                .HasIndex(e => new { e.MemberId, e.ContributionDate, e.ContributionType })
                .HasDatabaseName("IX_MonthlyContribution_Validation");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                var currentUser = "System"; 

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.CreatedBy = currentUser;
                    entity.UpdatedAt = DateTime.UtcNow;
                    entity.UpdatedBy = currentUser;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                    entity.UpdatedBy = currentUser;
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                }
            }
        }



    }
}
