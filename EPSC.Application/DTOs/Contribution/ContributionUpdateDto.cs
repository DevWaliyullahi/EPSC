using EPSC.Utility.Enums;
using System.ComponentModel.DataAnnotations;

namespace EPSC.Application.DTOs.Contribution
{
    public class ContributionUpdateDto
    {
        public Guid? EmployerId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? Amount { get; set; }

        public ContributionType? ContributionType { get; set; }
        public DateTime? ContributionDate { get; set; }
        public bool? IsValidated { get; set; }
        public string? ValidationNotes { get; set; }
    }
}
