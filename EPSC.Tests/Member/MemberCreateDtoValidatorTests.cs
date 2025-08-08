using EPSC.Application.DTOs.Member;
using EPSC.Services.Validations.Member;
using FluentValidation.TestHelper;
using Xunit;

namespace EPSC.Tests.Member
{

    public class MemberValidatorTests
    {
        private readonly MemberCreateDtoValidator _createValidator;
        private readonly MemberUpdateDtoValidator _updateValidator;

        public MemberValidatorTests()
        {
            _createValidator = new MemberCreateDtoValidator();
            _updateValidator = new MemberUpdateDtoValidator();
        }

        [Fact]
        public void CreateValidator_Should_Have_Error_When_FirstName_Is_Empty()
        {
            var model = new MemberCreateDto { FirstName = "" };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(m => m.FirstName);
        }

        [Fact]
        public void CreateValidator_Should_Have_Error_When_LastName_Is_Empty()
        {
            var model = new MemberCreateDto { LastName = "" };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(m => m.LastName);
        }

        [Fact]
        public void CreateValidator_Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new MemberCreateDto { Email = "notanemail" };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(m => m.Email);
        }

        [Fact]
        public void CreateValidator_Should_Have_Error_When_Phone_Is_Invalid()
        {
            var model = new MemberCreateDto { PhoneNumber = "12345" };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(m => m.PhoneNumber);
        }

        [Fact]
        public void CreateValidator_Should_Have_Error_When_DateOfBirth_Is_In_Future()
        {
            var model = new MemberCreateDto { DateOfBirth = DateTime.Today.AddDays(1) };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(m => m.DateOfBirth);
        }

        [Fact]
        public void CreateValidator_Should_Have_Error_When_Age_Is_Less_Than_18()
        {
            var model = new MemberCreateDto { DateOfBirth = DateTime.Today.AddYears(-17) };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(m => m.DateOfBirth);
        }

        [Fact]
        public void CreateValidator_Should_Have_Error_When_Age_Is_Greater_Than_70()
        {
            var model = new MemberCreateDto { DateOfBirth = DateTime.Today.AddYears(-71) };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(m => m.DateOfBirth);
        }

        [Fact]
        public void CreateValidator_Should_Not_Have_Error_When_Valid()
        {
            var model = new MemberCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = DateTime.Today.AddYears(-30)
            };
            var result = _createValidator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // UpdateValidator tests

        [Fact]
        public void UpdateValidator_Should_Have_Error_When_Id_Is_Empty()
        {
            var model = new MemberUpdateDto { Id = Guid.Empty };
            var result = _updateValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(m => m.Id);
        }

        [Fact]
        public void UpdateValidator_Should_Pass_When_Valid()
        {
            var model = new MemberUpdateDto
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = DateTime.Today.AddYears(-30)
            };
            var result = _updateValidator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

