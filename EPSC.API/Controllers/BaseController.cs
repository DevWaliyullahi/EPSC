using EPSC.Application.DTOs.Contribution;
using EPSC.Application.DTOs.Member;
using Microsoft.AspNetCore.Mvc;
using RPNL.Net.Utilities.ResponseUtil;

namespace EPSC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult NotifyModelStateError()
        {
            var errorMessages = new List<string>();
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            foreach (var error in errors)
            {
                var message = error.Exception?.Message ?? error.ErrorMessage;
                errorMessages.Add(message);
            }

            var response = new ResponseModel
            {
                Code = ErrorCodes.Failed,
                Message = errorMessages.FirstOrDefault() ?? "Validation failed",
                Success = false
            };

            return BadRequest(response);
        }

        protected new IActionResult Response<T>(ResponseModel<T>? result = null)
        {
            if (result == null)
                return BadRequest(new ResponseModel { Code = ErrorCodes.Failed, Message = "No response data", Success = false });

            if (!result.Success)
            {
                if (result.Code == ErrorCodes.DataNotFound)
                    return NotFound(result);
                return BadRequest(result);
            }

            // Handle POST requests for creation
            if (HttpMethods.IsPost(Request?.Method ?? string.Empty))
            {
                // For Member creation
                if (result.Data is MemberViewModel member)
                {
                    if (member.MemberId == Guid.Empty)
                        return BadRequest(new ResponseModel { Code = ErrorCodes.Failed, Message = "MemberId cannot be empty", Success = false });

                    return CreatedAtAction("GetMemberById", "Members", new { id = member.MemberId }, result);
                }

                // For Contribution creation  
                if (result.Data is ContributionViewModel contribution)
                {
                    if (contribution.ContributionId == Guid.Empty)
                        return BadRequest(new ResponseModel { Code = ErrorCodes.Failed, Message = "ContributionId cannot be empty", Success = false });

                    return CreatedAtAction("GetContribution", "Contributions", new { id = contribution.ContributionId }, result);
                }
            }

            return Ok(result);
        }

        protected new IActionResult Response(ResponseModel? result = null)
        {
            if (result == null)
                return BadRequest(new ResponseModel { Code = ErrorCodes.Failed, Message = "No response data", Success = false });

            if (!result.Success)
            {
                if (result.Code == ErrorCodes.DataNotFound)
                    return NotFound(result);
                return BadRequest(result);
            }

            // Return NoContent for successful DELETE operations
            if (HttpMethods.IsDelete(Request?.Method ?? string.Empty))
                return NoContent();

            return Ok(result);
        }

        protected string GetUserId()
        {
            return User?.Identity?.Name ?? "system";
        }

        protected bool IsValidGuid(string value)
        {
            return Guid.TryParse(value, out _);
        }
    }
}