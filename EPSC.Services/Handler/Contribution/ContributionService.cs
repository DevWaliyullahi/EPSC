using EPSC.Application.Dto.Contribution;
using EPSC.Application.DTOs.Contribution;
using EPSC.Application.Extensions;
using EPSC.Application.Interfaces.IContributionService;
using EPSC.Services.Repositories.Contribution;
using EPSC.Services.Repositories.Member;
using EPSC.Utility.Enums;
using EPSC.Utility.Pagination;
using Microsoft.Extensions.Logging;
using RPNL.Net.Utilities.ResponseUtil; 

namespace EPSC.Services.Handler.Contribution
{
    public class ContributionService : IContributionService
    {
        private readonly IContributionRepository _contributionRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ILogger<ContributionService> _logger;

        public ContributionService(
            IContributionRepository contributionRepository,
            IMemberRepository memberRepository,
            ILogger<ContributionService> logger)
        {
            _contributionRepository = contributionRepository;
            _memberRepository = memberRepository;
            _logger = logger;
        }

        public async Task<ResponseModel<ContributionViewModel?>> GetContributionAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving contribution with ID: {ContributionId}", id);

                var contribution = await _contributionRepository.GetByIdAsync(id);
                if (contribution == null)
                {
                    _logger.LogWarning("Contribution not found with ID: {ContributionId}", id);
                    return new ResponseModel<ContributionViewModel?>
                    {
                        Success = false,
                        Code = ErrorCodes.DataNotFound,
                        Message = "Contribution not found",
                        Data = null
                    };
                }

                return new ResponseModel<ContributionViewModel?>
                {
                    Success = true,
                    Data = contribution.ToViewModel()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contribution with ID: {ContributionId}", id);
                return new ResponseModel<ContributionViewModel?>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while retrieving contribution",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<PagedResponse<ContributionViewModel>>> GetAllContributionsAsync(ContributionSearchDto searchDto)
        {
            try
            {
                _logger.LogInformation("Retrieving paged contributions with search criteria");

                var pagedContributions = await _contributionRepository.GetPagedAsync(searchDto);

                var viewModels = pagedContributions.Data.Select(c => c.ToViewModel()).ToList();

                return new ResponseModel<PagedResponse<ContributionViewModel>>
                {
                    Success = true,
                    Data = new PagedResponse<ContributionViewModel>(
                        viewModels,
                        pagedContributions.MetaData.TotalItems,
                        pagedContributions.MetaData.PageNumber,
                        pagedContributions.MetaData.PageSize
                    )
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged contributions");
                return new ResponseModel<PagedResponse<ContributionViewModel>>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while retrieving contributions"
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<ContributionViewModel>>> GetMemberContributionsAsync(Guid memberId)
        {
            try
            {
                _logger.LogInformation("Retrieving contributions for member: {MemberId}", memberId);

                var member = await _memberRepository.GetByIdAsync(memberId);
                if (member == null)
                {
                    return new ResponseModel<IEnumerable<ContributionViewModel>>
                    {
                        Success = false,
                        Code = ErrorCodes.DataNotFound,
                        Message = "Member not found"
                    };
                }

                var contributions = await _contributionRepository.GetByMemberIdAsync(memberId);
                var viewModels = contributions.Select(c => c.ToViewModel());

                return new ResponseModel<IEnumerable<ContributionViewModel>>
                {
                    Success = true,
                    Data = viewModels
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contributions for member: {MemberId}", memberId);
                return new ResponseModel<IEnumerable<ContributionViewModel>>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while retrieving member contributions"
                };
            }
        }

        public async Task<ResponseModel<ContributionViewModel>> CreateContributionAsync(ContributionCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new contribution for member: {MemberId}", dto.MemberId);

                var member = await _memberRepository.GetByIdAsync(dto.MemberId);
                if (member == null)
                {
                    return new ResponseModel<ContributionViewModel>
                    {
                        Success = false,
                        Code = ErrorCodes.DataNotFound,
                        Message = "Member not found"
                    };
                }

                //var employer = await _employerRepository.GetByIdAsync(dto.EmployerId);
                //if (employer == null)
                //    return new ResponseModel<ContributionViewModel>
                //    {
                //        Success = false,
                //        Code = ErrorCodes.DataNotFound,
                //        Message = "Employer not found"
                //    };


                await ValidateContributionRulesAsync(dto);

                var contribution = dto.ToEntity();
                var createdContribution = await _contributionRepository.CreateAsync(contribution);

                var result = await _contributionRepository.GetByIdAsync(createdContribution.ContributionId);

                return new ResponseModel<ContributionViewModel>
                {
                    Success = true,
                    Data = result!.ToViewModel()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contribution for member: {MemberId}", dto.MemberId);
                return new ResponseModel<ContributionViewModel>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while creating contribution"
                };
            }
        }

        public async Task<ResponseModel<ContributionViewModel?>> UpdateContributionAsync(Guid id, ContributionUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Updating contribution with ID: {ContributionId}", id);

                var contribution = await _contributionRepository.GetByIdAsync(id);
                if (contribution == null)
                {
                    return new ResponseModel<ContributionViewModel?>
                    {
                        Success = false,
                        Code = ErrorCodes.DataNotFound,
                        Message = "Contribution not found"
                    };
                }

                contribution.UpdateEntity(dto);

                var updatedContribution = await _contributionRepository.UpdateAsync(contribution);

                return new ResponseModel<ContributionViewModel?>
                {
                    Success = true,
                    Data = updatedContribution.ToViewModel()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contribution with ID: {ContributionId}", id);
                return new ResponseModel<ContributionViewModel?>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while updating contribution"
                };
            }
        }

        public async Task<ResponseModel<bool>> SoftDeleteContributionAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting contribution with ID: {ContributionId}", id);

                var result = await _contributionRepository.DeleteAsync(id);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted contribution with ID: {ContributionId}", id);
                }
                else
                {
                    _logger.LogWarning("Contribution not found for deletion with ID: {ContributionId}", id);
                }

                return new ResponseModel<bool>
                {
                    Success = result,
                    Data = result,
                    Code = result ? ErrorCodes.Successful : ErrorCodes.DataNotFound,
                    Message = result ? "Deleted successfully" : "Contribution not found"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contribution with ID: {ContributionId}", id);
                return new ResponseModel<bool>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while deleting contribution",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel<ContributionSummaryDto>> GetMemberSummaryAsync(Guid memberId)
        {
            try
            {
                _logger.LogInformation("Generating contribution summary for member: {MemberId}", memberId);

                var member = await _memberRepository.GetByIdAsync(memberId);
                if (member == null)
                {
                    return new ResponseModel<ContributionSummaryDto>
                    {
                        Success = false,
                        Code = ErrorCodes.DataNotFound,
                        Message = "Member not found"
                    };
                }

                var contributions = await _contributionRepository.GetByMemberIdAsync(memberId);
                var contributionsList = contributions.ToList();

                var validatedContributions = contributionsList.Where(c => c.IsValidated).ToList();
                var monthlyContributions = contributionsList.Where(c => c.ContributionType == ContributionType.Monthly).ToList();
                var voluntaryContributions = contributionsList.Where(c => c.ContributionType == ContributionType.Voluntary).ToList();

                var summary = new ContributionSummaryDto
                {
                    MemberId = memberId,
                    MemberName = $"{member.FirstName} {member.LastName}",
                    TotalContributions = contributionsList.Sum(c => c.Amount),
                    TotalValidatedContributions = validatedContributions.Sum(c => c.Amount),
                    TotalContributionCount = contributionsList.Count,
                    ValidatedContributionCount = validatedContributions.Count,
                    MonthlyContributions = monthlyContributions.Count,
                    VoluntaryContributions = voluntaryContributions.Count,
                    LastContributionDate = contributionsList.Any() ? contributionsList.Max(c => c.ContributionDate) : null,
                    FirstContributionDate = contributionsList.Any() ? contributionsList.Min(c => c.ContributionDate) : null,
                    IsEligibleForBenefits = validatedContributions.Count >= 6,
                    MonthsContributed = validatedContributions.Count
                };

                return new ResponseModel<ContributionSummaryDto>
                {
                    Success = true,
                    Data = summary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating contribution summary for member: {MemberId}", memberId);
                return new ResponseModel<ContributionSummaryDto>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while generating contribution summary"
                };
            }
        }

        public async Task<ResponseModel<bool>> ValidateContributionAsync(Guid contributionId, string? notes = null)
        {
            try
            {
                _logger.LogInformation("Validating contribution with ID: {ContributionId}", contributionId);

                var contribution = await _contributionRepository.GetByIdAsync(contributionId);
                if (contribution == null)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Code = ErrorCodes.DataNotFound,
                        Message = "Contribution not found",
                        Data = false
                    };
                }

                contribution.Validate(notes);
                await _contributionRepository.UpdateAsync(contribution);

                return new ResponseModel<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Contribution validated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating contribution with ID: {ContributionId}", contributionId);
                return new ResponseModel<bool>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while validating contribution",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel<bool>> CanMakeMonthlyContributionAsync(Guid memberId, DateTime contributionDate)
        {
            try
            {
                _logger.LogInformation("Checking monthly contribution eligibility for member: {MemberId}", memberId);

                var hasExistingMonthly = await _contributionRepository.HasMonthlyContributionAsync(memberId, contributionDate);

                return new ResponseModel<bool>
                {
                    Success = true,
                    Data = !hasExistingMonthly
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking monthly contribution eligibility for member: {MemberId}", memberId);
                return new ResponseModel<bool>
                {
                    Success = false,
                    Code = ErrorCodes.Failed,
                    Message = "An error occurred while checking contribution eligibility",
                    Data = false
                };
            }
        }

        private async Task ValidateContributionRulesAsync(ContributionCreateDto dto)
        {
            if (dto.Amount <= 0)
                throw new ArgumentException("Contribution amount must be greater than zero");

            if (dto.ContributionDate > DateTime.Now)
                throw new ArgumentException("Contribution date cannot be in the future");

            if (dto.ContributionType == ContributionType.Monthly)
            {
                var canMakeMonthly = await CanMakeMonthlyContributionAsync(dto.MemberId, dto.ContributionDate);
                if (!canMakeMonthly.Data)
                    throw new InvalidOperationException("Only one monthly contribution is allowed per month");
            }

            if (dto.ContributionDate < DateTime.Now.AddYears(-2))
                throw new ArgumentException("Contribution date is too old. Maximum allowed is 2 years back");

            _logger.LogInformation("Contribution validation passed for member: {MemberId}", dto.MemberId);
        }
    }
}
