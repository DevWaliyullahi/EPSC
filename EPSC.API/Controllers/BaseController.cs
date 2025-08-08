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

            // Return CreatedAtAction for POST requests that return MemberViewModel
            if (Request.Method == "POST" && typeof(T) == typeof(MemberViewModel))
            {
                var member = result.Data as MemberViewModel;
                if (member != null)
                {
                    return CreatedAtAction(nameof(MembersController.MemberController.GetMemberById),
                        new { id = member.MemberId }, member);
                }
            }

            // Default success response is 200 OK with data
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

            // Return NoContent for successful DELETE requests
            if (Request.Method == "DELETE")
                return NoContent();

            // Default success response is 200 OK
            return Ok(result);
        }
    }
}
