using EPSC.API.Controllers.MembersController;
using EPSC.Application.DTOs.Member;
using EPSC.Domain.Entities.Member;
using EPSC.Infrastructure.Configurations.Data;
using EPSC.Services.Handler.Member;
using EPSC.Services.Repositories;
using EPSC.Tests.Helper;
using EPSC.Utility.Enums;
using EPSC.Utility.Pagination;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RPNL.Net.Utilities.ResponseUtil;

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
        // Arrange
        var context = GetInMemoryDbContext();
        var repo = new MemberRepository(context);
        var logger = new NullLogger<MemberService>();
        var service = new MemberService(repo, logger);
        var controller = new MemberController(service);

        // Setup controller context for POST request
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.Request.Method = "POST";

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("http://localhost/fake-url");
        controller.Url = mockUrlHelper.Object;

        var createDto = new MemberCreateDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@example.com",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Today.AddYears(-25)
        };

        // Act - Create member
        var createResult = await controller.CreateMember(createDto) as CreatedAtActionResult;

        // Assert create result
        createResult.Should().NotBeNull();
        createResult!.StatusCode.Should().Be(201);

        // Extract the ResponseModel<MemberViewModel> from the CreatedAtActionResult
        var createResponseModel = createResult.Value as ResponseModel<MemberViewModel>;
        createResponseModel.Should().NotBeNull();
        createResponseModel!.Success.Should().BeTrue();
        createResponseModel.Data.Should().NotBeNull();
        createResponseModel.Data!.FullName.Should().Be("Test User");

        var createdMemberId = createResponseModel.Data.MemberId;

        // Act - Get the created member
        // Setup controller context for GET request
        controller.ControllerContext.HttpContext.Request.Method = "GET";

        var getResult = await controller.GetMemberById(createdMemberId) as OkObjectResult;

        // Assert get result
        getResult.Should().NotBeNull();

        // Extract the ResponseModel<MemberViewModel> from the OkObjectResult
        var getResponseModel = getResult!.Value as ResponseModel<MemberViewModel>;
        getResponseModel.Should().NotBeNull();
        getResponseModel!.Success.Should().BeTrue();
        getResponseModel.Data.Should().NotBeNull();
        getResponseModel.Data!.Email.Should().Be("testuser@example.com");
        getResponseModel.Data.MemberId.Should().Be(createdMemberId);
    }

    [Fact]
    public async Task DeleteMember_Should_SoftDelete_And_Not_Found_After()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repo = new MemberRepository(context);
        var logger = new NullLogger<MemberService>();
        var service = new MemberService(repo, logger);
        var controller = new MemberController(service);

        // Setup controller context
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var member = new TMember
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

        // Act - Delete member
        controller.ControllerContext.HttpContext.Request.Method = "DELETE";
        var deleteResult = await controller.SoftDeleteMember(member.MemberId);

        // Assert delete result
        deleteResult.Should().BeOfType<NoContentResult>();

        // Act - Try to get deleted member
        controller.ControllerContext.HttpContext.Request.Method = "GET";
        var getResult = await controller.GetMemberById(member.MemberId);

        // Assert get result - should be NotFound
        getResult.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResult = getResult as NotFoundObjectResult;
        var errorResponseModel = notFoundResult!.Value as ResponseModel<MemberViewModel>;
        errorResponseModel.Should().NotBeNull();
        errorResponseModel!.Success.Should().BeFalse();
        errorResponseModel.Code.Should().Be(ErrorCodes.DataNotFound);
    }

    [Fact]
    public async Task UpdateMember_Should_Update_And_Return_Ok()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repo = new MemberRepository(context);
        var logger = new NullLogger<MemberService>();
        var service = new MemberService(repo, logger);
        var controller = new MemberController(service);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var member = MemberTestHelper.GetSampleMember();
        await context.TMembers.AddAsync(member);
        await context.SaveChangesAsync();

        var updateDto = MemberTestHelper.GetValidUpdateDto(member.MemberId);

        // Act - Update member
        controller.ControllerContext.HttpContext.Request.Method = "PUT";
        var updateResult = await controller.UpdateMember(member.MemberId, updateDto) as OkObjectResult;

        // Assert
        updateResult.Should().NotBeNull();

        // Extract the ResponseModel<MemberViewModel> from the OkObjectResult
        var updateResponseModel = updateResult!.Value as ResponseModel<MemberViewModel>;
        updateResponseModel.Should().NotBeNull();
        updateResponseModel!.Success.Should().BeTrue();
        updateResponseModel.Data.Should().NotBeNull();
        updateResponseModel.Data!.FullName.Should().Be($"{updateDto.FirstName} {updateDto.LastName}");
    }

    [Fact]
    public async Task GetAllMembers_Should_Return_Filtered_Results()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repo = new MemberRepository(context);
        var logger = new NullLogger<MemberService>();
        var service = new MemberService(repo, logger);
        var controller = new MemberController(service);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var member1 = MemberTestHelper.GetSampleMember();
        var member2 = new TMember
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

        // Act
        controller.ControllerContext.HttpContext.Request.Method = "GET";
        var result = await controller.GetAllMembers(searchDto) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();

        // Extract the ResponseModel<PagedResponse<MemberViewModel>> from the OkObjectResult
        var responseModel = result!.Value as ResponseModel<PagedResponse<MemberViewModel>>;
        responseModel.Should().NotBeNull();
        responseModel!.Success.Should().BeTrue();
        responseModel.Data.Should().NotBeNull();
        responseModel.Data!.Data.Should().HaveCount(1);
        responseModel.Data.Data.First().FullName.Should().Contain("Alice");
    }

    [Fact]
    public async Task UpdateMember_Should_Return_NotFound_If_Member_Not_Exist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repo = new MemberRepository(context);
        var logger = new NullLogger<MemberService>();
        var service = new MemberService(repo, logger);
        var controller = new MemberController(service);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var nonExistentId = Guid.NewGuid();
        var updateDto = MemberTestHelper.GetValidUpdateDto(nonExistentId);

        // Act
        controller.ControllerContext.HttpContext.Request.Method = "PUT";
        var result = await controller.UpdateMember(nonExistentId, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResult = result as NotFoundObjectResult;
        var errorResponseModel = notFoundResult!.Value as ResponseModel<MemberViewModel>;
        errorResponseModel.Should().NotBeNull();
        errorResponseModel!.Success.Should().BeFalse();
        errorResponseModel.Code.Should().Be(ErrorCodes.DataNotFound);
        errorResponseModel.Message.Should().Be("Member not found.");
    }
}
}
