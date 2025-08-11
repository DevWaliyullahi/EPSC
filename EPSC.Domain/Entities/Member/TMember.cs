using EPSC.Domain.Entities.Contribution;
using EPSC.Utility.Enums;
using System.ComponentModel.DataAnnotations;


namespace EPSC.Domain.Entities.Member
{
    public class TMember : BaseEntity
    {
        [Key]
        public Guid MemberId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public MemberStatus Status { get; set; } = MemberStatus.Active;
        public Guid? EmployerId { get; set; }
        public virtual ICollection<TContribution>? Contributions { get; set; }

        // Business logic methods
        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        public bool IsEligibleForBenefits(int minimumMonths = 6)
        {
            // Check if member has at least 6 months of contributions
            return Contributions?.Count(c => c.IsValidated) >= minimumMonths;
        }
    }
}
