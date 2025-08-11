using EPSC.Domain.Entities.Employer;
using EPSC.Domain.Entities.Member;
using EPSC.Utility.Enums;

namespace EPSC.Domain.Entities.Contribution
{
    public class TContribution : BaseEntity
    {
        public Guid ContributionId { get; set; }
        public Guid MemberId { get; set; }
        public Guid? EmployerId { get; set; }
        public decimal Amount { get; set; }
        public ContributionType ContributionType { get; set; }
        public DateTime ContributionDate { get; set; }
        public bool IsValidated { get; set; }
        public string? ValidationNotes { get; set; }
        public DateTime? ValidationDate { get; set; }

        // Navigation Properties
        public virtual TMember Member { get; set; }
        public virtual TEmployer? Employer { get; set; }

        // Business logic methods
        public bool IsMonthlyContribution => ContributionType == ContributionType.Monthly;
        public bool IsVoluntaryContribution => ContributionType == ContributionType.Voluntary;

        public void Validate(string? notes = null)
        {
            IsValidated = true;
            ValidationDate = DateTime.UtcNow;
            ValidationNotes = notes;
        }

        public bool IsForCurrentMonth()
        {
            var now = DateTime.Now;
            return ContributionDate.Year == now.Year &&
                   ContributionDate.Month == now.Month;
        }
    }
}
