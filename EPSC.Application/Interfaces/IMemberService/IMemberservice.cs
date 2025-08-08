using EPSC.Application.DTOs.Member;
using EPSC.Utility.Pagination;
using RPNL.Net.Utilities.ResponseUtil;

namespace EPSC.Application.Interfaces.IMemberService
{
    public interface IMemberService
    {
        Task<ResponseModel<MemberViewModel>> CreateMemberAsync(MemberCreateDto dto);
        Task<ResponseModel<MemberViewModel>> GetMemberAsync(Guid id);
        Task<ResponseModel<PagedResponse<MemberViewModel>>> GetAllMembersAsync(MemberSearchDto payload);
        Task<ResponseModel<MemberViewModel>> UpdateMemberAsync(Guid memberId, MemberUpdateDto dto);
        Task<ResponseModel> SoftDeleteMemberAsync(Guid id);
    }
}
