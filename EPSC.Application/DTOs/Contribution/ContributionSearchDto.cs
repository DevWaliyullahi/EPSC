using EPSC.Utility;
using EPSC.Utility.Enums;

namespace EPSC.Application.DTOs.Contribution
{
    public class ContributionSearchDto : BaseSearchDto
    {
        public Guid? MemberId { get; set; }
        public Guid? EmployerId { get; set; }
        public ContributionType? ContributionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsValidated { get; set; }
        public decimal? MinAmount { get; set; }  
        public decimal? MaxAmount { get; set; }
    }
}
