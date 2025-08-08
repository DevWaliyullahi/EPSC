using EPSC.Utility;
using EPSC.Utility.Enums;

namespace EPSC.Application.DTOs.Member
{
    public class MemberSearchDto : BaseSearchDto
    {
        public string? Name { get; set; }
        public MemberStatus? Status { get; set; }
       
    }
}
