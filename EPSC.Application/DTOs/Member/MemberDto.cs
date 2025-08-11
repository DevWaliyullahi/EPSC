namespace EPSC.Application.DTOs.Member
{
    public class MemberCreateDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class MemberUpdateDto : MemberCreateDto
    {
        public Guid MemberId { get; set; }
    }
    
}
