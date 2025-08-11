using EPSC.Application.Dto.Contribution;
using EPSC.Application.DTOs.Contribution;
using EPSC.Services.Repositories.Contribution;
using EPSC.Services.Repositories.Member;
using EPSC.Utility.Enums;
using FluentValidation;

namespace EPSC.Services.Validations.Contribution
{
    public class CreateContributionDtoValidator : AbstractValidator<ContributionCreateDto>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IContributionRepository _contributionRepository;

        public CreateContributionDtoValidator(IMemberRepository memberRepository, IContributionRepository contributionRepository)
        {
            _memberRepository = memberRepository;
            _contributionRepository = contributionRepository;

            RuleFor(x => x.MemberId)
                .NotEmpty()
                .WithMessage("Member ID is required")
                .MustAsync(MemberExists)
                .WithMessage("Member does not exist");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero")
                .LessThanOrEqualTo(1000000)
                .WithMessage("Amount cannot exceed 1,000,000");

            RuleFor(x => x.ContributionType)
                .IsInEnum()
                .WithMessage("Invalid contribution type");

            RuleFor(x => x.ContributionDate)
                .NotEmpty()
                .WithMessage("Contribution date is required")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("Contribution date cannot be in the future")
                .GreaterThan(DateTime.Now.AddYears(-2))
                .WithMessage("Contribution date cannot be more than 2 years ago");

            // Monthly contribution validation
            RuleFor(x => x)
                .MustAsync(ValidateMonthlyContribution)
                .WithMessage("Only one monthly contribution is allowed per month")
                .When(x => x.ContributionType == ContributionType.Monthly);
        }

        private async Task<bool> MemberExists(Guid memberId, CancellationToken cancellationToken)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            return member != null && !member.IsDeleted;
        }

        private async Task<bool> ValidateMonthlyContribution(ContributionCreateDto dto, CancellationToken cancellationToken)
        {
            if (dto.ContributionType != ContributionType.Monthly)
                return true;

            var hasExisting = await _contributionRepository.HasMonthlyContributionAsync(dto.MemberId, dto.ContributionDate);
            return !hasExisting;
        }
    }

    public class ContributionUpdateDtoValidator : AbstractValidator<ContributionUpdateDto>
    {
        public ContributionUpdateDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero")
                .LessThanOrEqualTo(1000000)
                .WithMessage("Amount cannot exceed 1,000,000")
                .When(x => x.Amount.HasValue);

            RuleFor(x => x.ContributionType)
                .IsInEnum()
                .WithMessage("Invalid contribution type")
                .When(x => x.ContributionType.HasValue);

            RuleFor(x => x.ContributionDate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("Contribution date cannot be in the future")
                .GreaterThan(DateTime.Now.AddYears(-2))
                .WithMessage("Contribution date cannot be more than 2 years ago")
                .When(x => x.ContributionDate.HasValue);

            RuleFor(x => x.ValidationNotes)
                .MaximumLength(500)
                .WithMessage("Validation notes cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.ValidationNotes));
        }
    }

    public class ContributionSearchDtoValidator : AbstractValidator<ContributionSearchDto>
    {
        public ContributionSearchDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than zero");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than zero")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size cannot exceed 100");

            RuleFor(x => x.ContributionType)
                .IsInEnum()
                .WithMessage("Invalid contribution type")
                .When(x => x.ContributionType.HasValue);

            RuleFor(x => x.MinAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum amount cannot be negative")
                .When(x => x.MinAmount.HasValue);

            RuleFor(x => x.MaxAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maximum amount cannot be negative")
                .GreaterThanOrEqualTo(x => x.MinAmount)
                .WithMessage("Maximum amount must be greater than or equal to minimum amount")
                .When(x => x.MaxAmount.HasValue && x.MinAmount.HasValue);

            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate)
                .WithMessage("To date must be greater than or equal to from date")
                .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
        }
    }
}
