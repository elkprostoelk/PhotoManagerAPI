using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.Core.Services;
using PhotoManagerAPI.Web.Extensions;

namespace PhotoManagerAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IUserService _userService;

        public AuthController(
            IValidator<LoginDto> loginValidator,
            IUserService userService)
        {
            _loginValidator = loginValidator;
            _userService = userService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ServiceResultDto<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            
            var loginResult = await _userService.SignInAsync(loginDto);
            if (loginResult.IsSuccess)
            {
                return Ok(loginResult);
            }

            ModelStateExtensions.AddErrors(ModelState, loginResult);
            return BadRequest(ModelState);
        }
    }
}
