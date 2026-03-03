using Blockchain.Application.DTOs.User;
using Blockchain.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.API.Controllers
{
    /// <summary>User profile management</summary>
    [Tags("User")]
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;

        /// <summary>Get my profile</summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile(CancellationToken ct)
            => HandleResult(await _userService.GetProfileAsync(CurrentUserId, ct));

        /// <summary>Update my profile</summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken ct)
            => HandleResult(await _userService.UpdateProfileAsync(CurrentUserId, dto, ct));

        /// <summary>Delete my account</summary>
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteAccount([FromQuery] string password, CancellationToken ct)
            => HandleResult(await _userService.DeleteAccountAsync(CurrentUserId, password, ct));
    }
}
