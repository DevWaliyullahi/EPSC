using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSC.Application.DTOs.Contribution
{
    public class ContributionSummaryDto
    {
        public Guid MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public decimal TotalContributions { get; set; }
        public decimal TotalValidatedContributions { get; set; }
        public int TotalContributionCount { get; set; }
        public int ValidatedContributionCount { get; set; }
        public int MonthlyContributions { get; set; }
        public int VoluntaryContributions { get; set; }
        public DateTime? LastContributionDate { get; set; }
        public DateTime? FirstContributionDate { get; set; }
        public bool IsEligibleForBenefits { get; set; }
        public int MonthsContributed { get; set; }
    }
}
