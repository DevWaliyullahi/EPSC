using EPSC.API.Controllers.MembersController;
using EPSC.Application.DTOs.Member;
using EPSC.Infrastructure.Configurations.Data;
using EPSC.Services.Handler.Member;
using EPSC.Services.Repositories;
using EPSC.Tests.Helper;
using EPSC.Utility.Enums;
using EPSC.Utility.Pagination;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace EPSC.Tests
{
    public class MembersControllerIntegrationTests
    {
        private EPSCDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EPSCDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new EPSCDbContext(options);
            return context;
        }

        [Fact]
        public async Task CreateMember_Then_GetMember_Returns_Created_Member()
        {
            var context = GetInMemoryDbContext();
            var repo = new MemberRepository(context);
            var logger = new NullLogger<MemberService>();
            var service = new MemberService(repo, logger);
            var controller = new MemberController(service);

            var createDto = new MemberCreateDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = DateTime.Today.AddYears(-25)
            };

            var createResult = await controller.CreateMember(createDto) as CreatedAtActionResult;
            createResult.Should().NotBeNull();
            var createdMember = createResult!.Value as MemberViewModel;
            createdMember.Should().NotBeNull();
            createdMember!.FullName.Should().Be("Test User");

            var getResult = await controller.GetMemberById(createdMember.MemberId) as OkObjectResult;
            getResult.Should().NotBeNull();
            var fetchedMember = getResult!.Value as MemberViewModel;
            fetchedMember.Should().NotBeNull();
            fetchedMember!.Email.Should().Be("testuser@example.com");
        }

        [Fact]
        public async Task DeleteMember_Should_SoftDelete_And_Not_Found_After()
        {
            var context = GetInMemoryDbContext();
            var repo = new MemberRepository(context);
            var logger = new NullLogger<MemberService>();
            var service = new MemberService(repo, logger);
            var controller = new MemberController(service);

            var member = new Domain.Entities.Member.TMember
            {
                MemberId = Guid.NewGuid(),
                FirstName = "Del",
                LastName = "User",
                Email = "deluser@example.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = DateTime.Today.AddYears(-40),
                IsDeleted = false
            };
            await context.TMembers.AddAsync(member);
            await context.SaveChangesAsync();

            var deleteResult = await controller.SoftDeleteMember(member.MemberId);
            deleteResult.Should().BeOfType<NoContentResult>();

            var getResult = await controller.GetMemberById(member.MemberId);
            getResult.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateMember_Should_Update_And_Return_Ok()
        {
            var context = GetInMemoryDbContext();
            var repo = new MemberRepository(context);
            var logger = new NullLogger<MemberService>();
            var service = new MemberService(repo, logger);
            var controller = new MemberController(service);

            var member = MemberTestHelper.GetSampleMember();
            await context.TMembers.AddAsync(member);
            await context.SaveChangesAsync();

            var updateDto = MemberTestHelper.GetValidUpdateDto(member.MemberId);

            var updateResult = await controller.UpdateMember(member.MemberId, updateDto) as OkObjectResult;

            updateResult.Should().NotBeNull();
            var updatedMember = updateResult!.Value as MemberViewModel;
            updatedMember.Should().NotBeNull();
            updatedMember!.FullName.Should().Be($"{updateDto.FirstName} {updateDto.LastName}");
        }

        [Fact]
        public async Task GetAllMembers_Should_Return_Filtered_Results()
        {
            var context = GetInMemoryDbContext();
            var repo = new MemberRepository(context);
            var logger = new NullLogger<MemberService>();
            var service = new MemberService(repo, logger);
            var controller = new MemberController(service);

            var member1 = MemberTestHelper.GetSampleMember();
            var member2 = new Domain.Entities.Member.TMember
            {
                MemberId = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = DateTime.Today.AddYears(-30),
                IsDeleted = false,
                Status = (MemberStatus)1
            };
            await context.TMembers.AddRangeAsync(member1, member2);
            await context.SaveChangesAsync();

            var searchDto = new MemberSearchDto
            {
                Name = "Alice",
                PageNumber = 1,
                PageSize = 10
            };

            var result = await controller.GetAllMembers(searchDto) as OkObjectResult;

            result.Should().NotBeNull();
            var pagedResponse = result!.Value as PagedResponse<MemberViewModel>;
            pagedResponse.Should().NotBeNull();
            pagedResponse!.Data.Should().HaveCount(1);
            pagedResponse.Data.First().FullName.Should().Contain("Alice");
        }

        [Fact]
        public async Task UpdateMember_Should_Return_NotFound_If_Member_Not_Exist()
        {
            var context = GetInMemoryDbContext();
            var repo = new MemberRepository(context);
            var logger = new NullLogger<MemberService>();
            var service = new MemberService(repo, logger);
            var controller = new MemberController(service);

            var nonExistentId = Guid.NewGuid();
            var updateDto = MemberTestHelper.GetValidUpdateDto(nonExistentId);

            var result = await controller.UpdateMember(nonExistentId, updateDto);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

    }
}
