using EPSC.Application.DTOs.Member;
using EPSC.Domain.Entities.Member;
using EPSC.Services.Handler.Member;
using EPSC.Services.Repositories;
using EPSC.Tests.Helper;
using EPSC.Utility.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPSC.Tests
{
    public class MemberServiceTests
    {
        private readonly Mock<IMemberRepository> _repositoryMock;
        private readonly Mock<ILogger<MemberService>> _loggerMock;
        private readonly MemberService _service;

        public MemberServiceTests()
        {
            _repositoryMock = new Mock<IMemberRepository>();
            _loggerMock = new Mock<ILogger<MemberService>>();
            _service = new MemberService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateMemberAsync_Should_Create_Member()
        {
            TMember savedMember = null!;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TMember>()))
                .Callback<TMember>(m => savedMember = m)
                .Returns(Task.CompletedTask);

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var dto = new MemberCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = DateTime.Today.AddYears(-30)
            };

            var result = await _service.CreateMemberAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(savedMember);
            Assert.Equal(dto.Email, savedMember.Email);
        }

        [Fact]
        public async Task GetMemberAsync_Should_Return_NotFound_If_Member_Not_Exist()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((TMember?)null);

            var result = await _service.GetMemberAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Member not found.", result.Message);
        }

        [Fact]
        public async Task UpdateMemberAsync_Should_Update_Member()
        {
            var existingMember = new TMember
            {
                MemberId = Guid.NewGuid(),
                FirstName = "Old",
                LastName = "Name",
                Email = "old@example.com",
                PhoneNumber = "+1111111111",
                DateOfBirth = DateTime.Today.AddYears(-40),
                IsDeleted = false
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(existingMember.MemberId))
                .ReturnsAsync(existingMember);
            _repositoryMock.Setup(r => r.Update(It.IsAny<TMember>()));
            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var dto = new MemberUpdateDto
            {
                Id = existingMember.MemberId,
                FirstName = "New",
                LastName = "Name",
                Email = "new@example.com",
                PhoneNumber = "+2222222222",
                DateOfBirth = DateTime.Today.AddYears(-30)
            };

            var result = await _service.UpdateMemberAsync(existingMember.MemberId, dto);

            Assert.True(result.Success);
            Assert.Equal("New", existingMember.FirstName);
            Assert.Equal("new@example.com", existingMember.Email);
        }

        [Fact]
        public async Task SoftDeleteMemberAsync_Should_Mark_IsDeleted()
        {
            var existingMember = new TMember { MemberId = Guid.NewGuid(), IsDeleted = false };
            _repositoryMock.Setup(r => r.GetByIdAsync(existingMember.MemberId))
                .ReturnsAsync(existingMember);
            _repositoryMock.Setup(r => r.Update(It.IsAny<TMember>()));
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.SoftDeleteMemberAsync(existingMember.MemberId);

            Assert.True(result.Success);
            Assert.True(existingMember.IsDeleted);
        }

        [Fact]
        public async Task GetAllMembersAsync_Should_Return_Filtered_Paged_Result()
        {
            var members = new List<TMember>
           {
               MemberTestHelper.GetSampleMember(),
               MemberTestHelper.GetSampleMember(),
               new TMember
               {
                   MemberId = Guid.NewGuid(),
                   FirstName = "Alice",
                   LastName = "Smith",
                   Email = "alice@example.com",
                   DateOfBirth = DateTime.Today.AddYears(-25),
                   Status = MemberStatus.Active,  
                   IsDeleted = false
               }
           }.AsQueryable();

            _repositoryMock.SetupQuery(members);

            var searchDto = new MemberSearchDto
            {
                Name = "Alice",
                PageNumber = 1,
                PageSize = 10
            };

            var result = await _service.GetAllMembersAsync(searchDto);

            Assert.True(result.Success);
            Assert.Single(result.Data.Data);
            Assert.Contains("Alice", result.Data.Data.First().FullName);
        }

        [Fact]
        public async Task UpdateMemberAsync_Should_Return_NotFound_When_Member_Does_Not_Exist()
        {
            var nonExistingId = Guid.NewGuid();
            _repositoryMock.SetupGetById(nonExistingId, null);

            var dto = MemberTestHelper.GetValidUpdateDto(nonExistingId);

            var result = await _service.UpdateMemberAsync(nonExistingId, dto);

            Assert.False(result.Success);
            Assert.Equal("Member not found.", result.Message);
        }

        [Fact]
        public async Task SoftDeleteMemberAsync_Should_Return_NotFound_If_Already_Deleted()
        {
            var existingMember = MemberTestHelper.GetSampleMember();
            existingMember.IsDeleted = true;

            _repositoryMock.SetupGetById(existingMember.MemberId, existingMember);

            var result = await _service.SoftDeleteMemberAsync(existingMember.MemberId);

            Assert.False(result.Success);
            Assert.Equal("Member not found.", result.Message);
        }

    }
}
