using EPSC.Application.DTOs.Member;
using EPSC.Application.Interfaces.IMemberService;
using EPSC.Domain.Entities.Member;
using EPSC.Services.Repositories.Member;
using EPSC.Utility.Pagination;
using Microsoft.Extensions.Logging;
using RPNL.Net.Utilities.ResponseUtil;

namespace EPSC.Services.Handler.Member
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _repository;
        private readonly ILogger<MemberService> _logger;

        public MemberService(IMemberRepository repository, ILogger<MemberService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new member with the provided details.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<MemberViewModel>> CreateMemberAsync(MemberCreateDto dto)
        {
            var response = new ResponseModel<MemberViewModel>();

            try
            {
                if (await _repository.EmailExistsAsync(dto.Email))
                {
                    response.Code = ErrorCodes.Failed;
                    response.Success = false;
                    response.Message = "Email already exists.";
                    return response;
                }

                var member = new TMember
                {
                    MemberId = Guid.NewGuid(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    DateOfBirth = dto.DateOfBirth.HasValue ? dto.DateOfBirth.Value : throw new ArgumentException("DateOfBirth cannot be null"),
                    IsDeleted = false
                };

                await _repository.AddAsync(member);
                await _repository.SaveChangesAsync();

                response.Code = ErrorCodes.Successful;
                response.Success = true;
                response.Message = "Member created successfully";
                response.Data = new MemberViewModel
                {
                    MemberId = member.MemberId,
                    FullName = $"{member.FirstName} {member.LastName}",
                    Email = member.Email,
                    Status = member.Status,
                    PhoneNumber = member.PhoneNumber,
                    DateOfBirth = member.DateOfBirth,
                    CreatedAt = member.CreatedAt,
                    CreatedBy = member.CreatedBy != null ? Guid.Parse(member.CreatedBy) : Guid.Empty,
                };

                _logger.LogInformation("Created member {MemberId} with email {Email}", member.MemberId, member.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating member with email {Email}", dto.Email);
                response.Code = ErrorCodes.Failed;
                response.Success = false;
                response.Message = "An error occurred while creating the member.";
            }

            return response;
        }

        /// <summary>
        /// Retrieves a member by their unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseModel<MemberViewModel>> GetMemberAsync(Guid id)
        {
            var response = new ResponseModel<MemberViewModel>();

            try
            {
                var member = await _repository.GetByIdAsync(id);
                if (member == null || member.IsDeleted)
                {
                    response.Code = ErrorCodes.DataNotFound;
                    response.Success = false;
                    response.Message = "Member not found.";
                    return response;
                }

                response.Code = ErrorCodes.Successful;
                response.Success = true;
                response.Data = new MemberViewModel
                {
                    MemberId = member.MemberId,
                    FullName = $"{member.FirstName} {member.LastName}",
                    Email = member.Email,
                    Status = member.Status,
                    PhoneNumber = member.PhoneNumber,
                    DateOfBirth = member.DateOfBirth,
                    CreatedAt = member.CreatedAt,
                    UpdatedAt = member.UpdatedAt,
                    DeletedAt = member.DeletedAt,
                    IsDeleted = member.IsDeleted,
                    CreatedBy = member.CreatedBy != null ? Guid.Parse(member.CreatedBy) : Guid.Empty,
                    UpdatedBy = member.UpdatedBy != null ? Guid.Parse(member.UpdatedBy) : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving member {MemberId}", id);
                response.Code = ErrorCodes.Failed;
                response.Success = false;
                response.Message = "An error occurred while retrieving the member.";
            }

            return response;
        }

        /// <summary>
        /// Retrieves a paginated list of members based on search criteria.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<ResponseModel<PagedResponse<MemberViewModel>>> GetAllMembersAsync(MemberSearchDto payload)
        {
            var response = new ResponseModel<PagedResponse<MemberViewModel>>();

            try
            {
                var pagedResult = await _repository.GetMembersPagedAsync(payload);

                response.Code = ErrorCodes.Successful;
                response.Success = true;
                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving member list");
                response.Code = ErrorCodes.Failed;
                response.Success = false;
                response.Message = "An error occurred while retrieving members.";
            }

            return response;
        }


        /// <summary>
        /// Updates an existing member's details.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<MemberViewModel>> UpdateMemberAsync(Guid memberId, MemberUpdateDto dto)
        {
            var response = new ResponseModel<MemberViewModel>();

            try
            {
                var member = await _repository.GetByIdAsync(memberId);
                if (member == null || member.IsDeleted)
                {
                    response.Code = ErrorCodes.DataNotFound;
                    response.Success = false;
                    response.Message = "Member not found.";
                    return response;
                }

                if (await _repository.EmailExistsAsync(dto.Email, memberId))
                {
                    response.Code = ErrorCodes.Failed;
                    response.Success = false;
                    response.Message = "Email already exists.";
                    return response;
                }

                member.FirstName = dto.FirstName;
                member.LastName = dto.LastName;
                member.Email = dto.Email;
                member.PhoneNumber = dto.PhoneNumber;
                member.DateOfBirth = dto.DateOfBirth.HasValue ? dto.DateOfBirth.Value : throw new ArgumentException("DateOfBirth cannot be null");

                _repository.Update(member);
                await _repository.SaveChangesAsync();

                response.Code = ErrorCodes.Successful;
                response.Success = true;
                response.Message = "Member updated successfully";
                response.Data = new MemberViewModel
                {
                    MemberId = member.MemberId,
                    FullName = $"{member.FirstName} {member.LastName}",
                    Email = member.Email,
                    Status = member.Status,
                    PhoneNumber = member.PhoneNumber,
                    DateOfBirth = member.DateOfBirth,
                    CreatedAt = member.CreatedAt,
                    UpdatedAt = member.UpdatedAt,
                    DeletedAt = member.DeletedAt,
                    IsDeleted = member.IsDeleted,
                    CreatedBy = member.CreatedBy != null ? Guid.Parse(member.CreatedBy) : Guid.Empty,
                    UpdatedBy = member.UpdatedBy != null ? Guid.Parse(member.UpdatedBy) : null
                };

                _logger.LogInformation("Updated member {MemberId}", member.MemberId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating member {MemberId}", memberId);
                response.Code = ErrorCodes.Failed;
                response.Success = false;
                response.Message = "An error occurred while updating the member.";
            }

            return response;
        }

        /// <summary>
        /// Soft deletes a member by marking them as deleted without removing from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SoftDeleteMemberAsync(Guid id)
        {
            var response = new ResponseModel();

            try
            {
                var member = await _repository.GetByIdAsync(id);
                if (member == null || member.IsDeleted)
                {
                    response.Code = ErrorCodes.DataNotFound;
                    response.Success = false;
                    response.Message = "Member not found.";
                    return response;
                }

                member.IsDeleted = true;
                _repository.Update(member);
                await _repository.SaveChangesAsync();

                response.Code = ErrorCodes.Successful;
                response.Success = true;
                response.Message = "Member deleted successfully";

                _logger.LogInformation("Soft deleted member {MemberId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting member {MemberId}", id);
                response.Code = ErrorCodes.Failed;
                response.Success = false;
                response.Message = "An error occurred while deleting the member.";
            }

            return response;
        }
    }
}