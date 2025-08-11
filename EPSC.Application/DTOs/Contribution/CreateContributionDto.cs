using EPSC.Utility.Enums;
using System.ComponentModel.DataAnnotations;

namespace EPSC.Application.Dto.Contribution
{
    public class ContributionCreateDto
    {
        [Required]
        public Guid MemberId { get; set; }

        public Guid? EmployerId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public ContributionType ContributionType { get; set; }

        [Required]
        public DateTime ContributionDate { get; set; }

        public string? ValidationNotes { get; set; }
    }
}
