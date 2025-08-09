using EPSC.API.Controllers.MembersController;
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
            var erroMesg = new List<string>();
            var erros = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var erro in erros)
            {
                var msg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                erroMesg.Add(msg);
            }
            ResponseModel response = new()
            {
                Code = ErrorCodes.Failed,
                Message = erroMesg.FirstOrDefault(),
                Success = false
            };
            return BadRequest(response);
        }

        protected new IActionResult Response<T>(ResponseModel<T>? result = null)
        {
            if (result == null)
                return BadRequest();

            if (!result.Success)
            {
                if (result.Code == ErrorCodes.DataNotFound)
                    return NotFound(result);
                return BadRequest(result);
            }

            // Handle POST requests for member creation
            if (HttpMethods.IsPost(Request?.Method ?? string.Empty) && result.Data is MemberViewModel member)
            {
                if (member.MemberId == Guid.Empty)
                    return BadRequest("MemberId cannot be empty.");

                // Use the correct action name that matches your controller method
                return CreatedAtAction(nameof(MemberController.GetMemberById), new { id = member.MemberId }, result);
            }

            // For all other successful operations (GET, PUT)
            return Ok(result);
        }

        protected new IActionResult Response(ResponseModel? result = null)
        {
            if (result == null)
                return BadRequest();

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
    }
}