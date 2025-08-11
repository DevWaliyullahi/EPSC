using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSC.Domain.Entities.BenefitEligibility
{
    public class TBenefitEligibility : BaseEntity
    {
        public Guid BenefitEligibilityId { get; set; }
        public Guid MemberId { get; set; }
        public bool IsEligible { get; set; }
        public DateTime EligibilityDate { get; set; }
        public int MonthsContributed { get; set; }
        public decimal TotalContributions { get; set; }
        public string? Notes { get; set; } = string.Empty;

        // Navigation property
        public virtual Member.TMember? Member { get; set; }
    }
}
