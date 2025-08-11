using EPSC.Application.DTOs.Member;
using FluentValidation;

namespace EPSC.Services.Validations.Member
{
    public abstract class MemberValidationBase<T> : AbstractValidator<T> where T : class
    {
        protected void SetupNameValidation(
            IRuleBuilder<T, string> ruleBuilder,
            string fieldName,
            bool isRequired = true)
        {
            if (isRequired)
            {
                ruleBuilder.NotEmpty()
                    .WithMessage($"{fieldName} is required.");
            }

            ruleBuilder.MaximumLength(50)
                .WithMessage($"{fieldName} must not exceed 50 characters.");
        }

        protected void SetupEmailValidation(
            IRuleBuilder<T, string> ruleBuilder,
            bool isRequired = true)
        {
            if (isRequired)
            {
                ruleBuilder.NotEmpty()
                    .WithMessage("Email is required.");
            }

            ruleBuilder.EmailAddress()
                .WithMessage("Email must be valid.");
        }

        protected void SetupPhoneValidation(
            IRuleBuilder<T, string> ruleBuilder,
            bool isRequired = true)
        {
            if (isRequired)
            {
                ruleBuilder.NotEmpty()
                    .WithMessage("Phone number is required.");
            }

            ruleBuilder.Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Phone number is invalid.");
        }

        protected bool IsValidAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) return true;

            var age = DateTime.Today.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value.Date > DateTime.Today.AddYears(-age))
                age--;

            return age >= 18 && age <= 70;
        }
    }

    public class MemberCreateDtoValidator : MemberValidationBase<MemberCreateDto>
    {
        public MemberCreateDtoValidator()
        {
            SetupNameValidation(RuleFor(x => x.FirstName), "First name");
            SetupNameValidation(RuleFor(x => x.LastName), "Last name");
            SetupEmailValidation(RuleFor(x => x.Email));
            SetupPhoneValidation(RuleFor(x => x.PhoneNumber));

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
                .Must(dateOfBirth => dateOfBirth.HasValue && IsValidAge(dateOfBirth))
                .WithMessage("Member must be between 18 and 70 years old.");
        }
    }

    public class MemberUpdateDtoValidator : MemberValidationBase<MemberUpdateDto>
    {
        public MemberUpdateDtoValidator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty()
                .WithMessage("Member Id is required for updates.");

            // Validate only when values are not null or empty
            When(x => !string.IsNullOrEmpty(x.FirstName), () =>
            {
                SetupNameValidation(RuleFor(x => x.FirstName), "First name");
            });

            When(x => !string.IsNullOrEmpty(x.LastName), () =>
            {
                SetupNameValidation(RuleFor(x => x.LastName), "Last name");
            });

            When(x => !string.IsNullOrEmpty(x.Email), () =>
            {
                SetupEmailValidation(RuleFor(x => x.Email));
            });

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                SetupPhoneValidation(RuleFor(x => x.PhoneNumber));
            });

            When(x => x.DateOfBirth.HasValue, () =>
            {
                RuleFor(x => x.DateOfBirth)
                    .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
                    .Must(dateOfBirth => !dateOfBirth.HasValue || IsValidAge(dateOfBirth))
                    .WithMessage("Member must be between 18 and 70 years old.");
            });
        }
    }
}
