using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSC.Domain.Entities.Employer
{
    public class TEmployer : BaseEntity
    {
        public Guid EmployerId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string RCNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Member.TMember>? Members { get; set; }
        public virtual ICollection<Contribution.TContribution>? Contributions { get; set; }
    }
}
