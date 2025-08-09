using EPSC.Utility.Enums;
using System.ComponentModel.DataAnnotations;


namespace EPSC.Domain.Entities.Member
{
    public class TMember : BaseEntity
    {
        [Key]
        public Guid MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public MemberStatus Status { get; set; }
    }
}
