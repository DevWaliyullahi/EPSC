using EPSC.Utility.Enums;

namespace EPSC.Application.DTOs.Contribution
{
    public class ContributionViewModel
    {
        public Guid ContributionId { get; set; }
        public Guid MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public Guid? EmployerId { get; set; }
        public string? EmployerName { get; set; }
        public decimal Amount { get; set; }
        public ContributionType ContributionType { get; set; }
        public string TypeDisplay => ContributionType.ToString();
        public DateTime ContributionDate { get; set; }
        public bool IsValidated { get; set; }
        public string? ValidationNotes { get; set; }
        public DateTime? ValidationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
