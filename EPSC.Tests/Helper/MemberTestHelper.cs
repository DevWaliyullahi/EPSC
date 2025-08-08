using EPSC.Application.DTOs.Member;
using EPSC.Domain.Entities.Member;
using EPSC.Services.Repositories;
using EPSC.Utility.Enums;
using Moq;

namespace EPSC.Tests.Helper
{
    public static class MemberTestHelper
    {
        // Creates a sample valid MemberCreateDto
        public static MemberCreateDto GetValidCreateDto()
            => new MemberCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = DateTime.Today.AddYears(-30)
            };

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

        // Setup mock repository with default GetByIdAsync to return a sample member or null
        public static void SetupGetById(this Mock<IMemberRepository> mock, Guid id, TMember? member = null)
        {
            mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(member ?? GetSampleMember(id));
        }

        // Setup mock repository AddAsync to capture the added entity
        public static void SetupAddAsync(this Mock<IMemberRepository> mock)
        {
            mock.Setup(x => x.AddAsync(It.IsAny<TMember>())).Returns(Task.CompletedTask);
        }

        // Setup SaveChangesAsync to return completed task
        public static void SetupSaveChanges(this Mock<IMemberRepository> mock)
        {
            mock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        }

        // Setup mock repository Query to return a specified IQueryable of TMember
        public static void SetupQuery(this Mock<IMemberRepository> mock, IQueryable<TMember> members)
        {
            mock.Setup(x => x.Query()).Returns(members);
        }

    }
}
