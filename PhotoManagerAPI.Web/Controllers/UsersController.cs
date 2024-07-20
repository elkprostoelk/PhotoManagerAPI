using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.Core.Services;

namespace PhotoManagerAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IValidator<NewUserDto> _newUserDtoValidator;
        private readonly IUserService _userService;

        public UsersController(
            IValidator<NewUserDto> newUserDtoValidator,
            IUserService userService)
        {
            _newUserDtoValidator = newUserDtoValidator;
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ServiceResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(NewUserDto newUserDto)
        {
            var validationResult = await _newUserDtoValidator.ValidateAsync(newUserDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateNewUserAsync(newUserDto);
            if (result.IsSuccess)
            {
                return StatusCode(201, result);
            }
            
            result.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }
    }
}
