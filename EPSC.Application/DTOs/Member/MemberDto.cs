namespace EPSC.Application.DTOs.Member
{
    public class MemberCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class MemberUpdateDto : MemberCreateDto
    {
        public Guid Id { get; set; }
    }

}
