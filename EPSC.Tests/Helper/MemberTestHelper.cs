using EPSC.Application.DTOs.Member;
using EPSC.Domain.Entities.Member;
using EPSC.Utility.Enums;

namespace EPSC.Tests.Helper
{
    public static class MemberTestHelper
    {
        
        // Creates a sample valid MemberUpdateDto
        public static MemberUpdateDto GetValidUpdateDto(Guid id)
            => new MemberUpdateDto
            {
                Id = id,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                PhoneNumber = "+1987654321",
                DateOfBirth = DateTime.Today.AddYears(-28)
            };

        // Creates a sample TMember entity matching the DTO data
        public static TMember GetSampleMember(Guid? id = null)
        {
            return new TMember
            {
                MemberId = id ?? Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = DateTime.Today.AddYears(-30),
                IsDeleted = false,
                Status = MemberStatus.Active  
            };
        }

        

    }
}
