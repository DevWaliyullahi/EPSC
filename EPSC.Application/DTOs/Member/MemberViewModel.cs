using EPSC.Utility.Enums;

namespace EPSC.Application.DTOs.Member
{
    public class MemberViewModel
    {
        public Guid MemberId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public MemberStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; } = null;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
