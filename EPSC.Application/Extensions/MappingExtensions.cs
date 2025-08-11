using EPSC.Application.Dto.Contribution;
using EPSC.Application.DTOs.Contribution;
using EPSC.Domain.Entities.Contribution;

namespace EPSC.Application.Extensions
{
    public static class MappingExtensions
    {
        public static ContributionViewModel ToViewModel(this TContribution contribution)
        {
            return new ContributionViewModel
            {
                ContributionId = contribution.ContributionId,
                MemberId = contribution.MemberId,
                MemberName = $"{contribution.Member?.FirstName} {contribution.Member?.LastName}",
                EmployerId = contribution.EmployerId,
                EmployerName = contribution.Employer?.CompanyName,
                Amount = contribution.Amount,
                ContributionType = contribution.ContributionType,
                ContributionDate = contribution.ContributionDate,
                IsValidated = contribution.IsValidated,
                ValidationNotes = contribution.ValidationNotes,
                ValidationDate = contribution.ValidationDate,
                CreatedAt = contribution.CreatedAt,
                UpdatedAt = contribution.UpdatedAt ?? default,
                CreatedBy = contribution.CreatedBy,
                UpdatedBy = contribution.UpdatedBy
            };
        }

        public static TContribution ToEntity(this ContributionCreateDto dto)
        {
            return new TContribution
            {
                MemberId = dto.MemberId,
                EmployerId = dto.EmployerId,
                Amount = dto.Amount,
                ContributionType = dto.ContributionType,
                ContributionDate = dto.ContributionDate,
                IsValidated = false,
                ValidationNotes = dto.ValidationNotes
            };
        }

        public static void UpdateEntity(this TContribution entity, ContributionUpdateDto dto)
        {
            if (dto.EmployerId.HasValue)
                entity.EmployerId = dto.EmployerId;

            if (dto.Amount.HasValue)
                entity.Amount = dto.Amount.Value;

            if (dto.ContributionType.HasValue)
                entity.ContributionType = dto.ContributionType.Value;

            if (dto.ContributionDate.HasValue)
                entity.ContributionDate = dto.ContributionDate.Value;

            if (dto.IsValidated.HasValue)
            {
                entity.IsValidated = dto.IsValidated.Value;
                if (dto.IsValidated.Value)
                    entity.ValidationDate = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(dto.ValidationNotes))
                entity.ValidationNotes = dto.ValidationNotes;
        }
    }
}
