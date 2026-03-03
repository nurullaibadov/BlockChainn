using Blockchain.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blockchain.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        protected Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        protected string CurrentUserEmail =>
            User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return result.StatusCode == 201
                    ? StatusCode(201, ApiResponse<T>.Ok(result.Data!, result.Message))
                    : Ok(ApiResponse<T>.Ok(result.Data!, result.Message));

            return result.StatusCode switch
            {
                404 => NotFound(ApiResponse<T>.Fail(result.Errors)),
                401 => Unauthorized(ApiResponse<T>.Fail(result.Errors)),
                403 => StatusCode(403, ApiResponse<T>.Fail(result.Errors)),
                _ => BadRequest(ApiResponse<T>.Fail(result.Errors))
            };
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess) return Ok(ApiResponse<object>.Ok(null, result.Message));
            return result.StatusCode switch
            {
                404 => NotFound(ApiResponse<object>.Fail(result.Errors)),
                401 => Unauthorized(ApiResponse<object>.Fail(result.Errors)),
                _ => BadRequest(ApiResponse<object>.Fail(result.Errors))
            };
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> Ok(T data, string? message = null)
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(List<string> errors)
            => new() { Success = false, Errors = errors };
    }
}
