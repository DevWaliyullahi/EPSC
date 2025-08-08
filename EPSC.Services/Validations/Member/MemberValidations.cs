using EPSC.Application.DTOs.Member;
using FluentValidation;
using System.Reflection;

namespace EPSC.Services.Validations.Member
{
    public abstract class MemberValidations<T> : AbstractValidator<T> where T : class
    {
        public void ValidateFirstName()
        {
            RuleFor(x => x).Custom((instance, context) =>
            {
                var value = GetStringProperty(instance, "FirstName", context);
                if (value == null) return;

                if (string.IsNullOrWhiteSpace(value))
                    context.AddFailure("FirstName", "First name is required.");
                else if (value.Length > 50)
                    context.AddFailure("FirstName", "First name must not exceed 50 characters.");
            });
        }

        public void ValidateLastName()
        {
            RuleFor(x => x).Custom((instance, context) =>
            {
                var value = GetStringProperty(instance, "LastName", context);
                if (value == null) return;

                if (string.IsNullOrWhiteSpace(value))
                    context.AddFailure("LastName", "Last name is required.");
                else if (value.Length > 50)
                    context.AddFailure("LastName", "Last name must not exceed 50 characters.");
            });
        }

        public void ValidateEmail()
        {
            RuleFor(x => x).Custom((instance, context) =>
            {
                var value = GetStringProperty(instance, "Email", context);
                if (value == null) return;

                if (string.IsNullOrWhiteSpace(value))
                    context.AddFailure("Email", "Email is required.");
                else if (!System.Net.Mail.MailAddress.TryCreate(value, out _))
                    context.AddFailure("Email", "Email must be valid.");
            });
        }

        public void ValidatePhone()
        {
            RuleFor(x => x).Custom((instance, context) =>
            {
                var value = GetStringProperty(instance, "PhoneNumber", context);
                if (value == null) 
                    return;

                if (string.IsNullOrWhiteSpace(value))
                    context.AddFailure("PhoneNumber", "Phone number is required.");
                else if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\+?[1-9]\d{1,14}$"))
                    context.AddFailure("PhoneNumber", "Phone number is invalid.");
            });
        }

        public void ValidateDateOfBirth()
        {
            RuleFor(x => x).Custom((instance, context) =>
            {
                var property = typeof(T).GetProperty("DateOfBirth", BindingFlags.Public | BindingFlags.Instance);
                if (property == null) 
                    return;

                var value = property.GetValue(instance) as DateTime?;
                if (value == null) 
                    return;

                if (value >= DateTime.Today)
                    context.AddFailure("DateOfBirth", "Date of birth must be in the past.");
                else
                {
                    var age = DateTime.Today.Year - value.Value.Year;
                    if (value.Value.Date > DateTime.Today.AddYears(-age)) age--;

                    if (age < 18 || age > 70)
                        context.AddFailure("DateOfBirth", "Member must be between 18 and 70 years old.");
                }
            });
        }


        private string? GetStringProperty(object instance, string propertyName, ValidationContext<T> context)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                context.AddFailure(propertyName, $"Property '{propertyName}' not found.");
                return null;
            }
            return property.GetValue(instance) as string;
        }
    }

    public class MemberCreateDtoValidator : MemberValidations<MemberCreateDto>
    {
        public MemberCreateDtoValidator()
        {
            ValidateFirstName();
            ValidateLastName();
            ValidateEmail();
            ValidateDateOfBirth();
            ValidatePhone();
        }
    }

    public class MemberUpdateDtoValidator : MemberValidations<MemberUpdateDto>
    {
        public MemberUpdateDtoValidator()
        {
            ValidateFirstName();
            ValidateLastName();
            ValidatePhone();
            ValidateEmail();
            ValidateDateOfBirth();

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Member Id is required for updates.");
        }
    }
}
