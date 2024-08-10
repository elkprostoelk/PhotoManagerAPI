using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.Core.Services;
using PhotoManagerAPI.Web.Extensions;
using PhotoManagerAPI.Web.Models;

namespace PhotoManagerAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly IValidator<NewPictureModel> _newPictureModelValidator;
        private readonly IPictureService _pictureService;
        private readonly IMapper _mapper;

        public PicturesController(
            IValidator<NewPictureModel> newPictureModelValidator,
            IPictureService pictureService,
            IMapper mapper)
        {
            _newPictureModelValidator = newPictureModelValidator;
            _pictureService = pictureService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPicture([FromForm]NewPictureModel newPictureModel)
        {
            var validationResult = await _newPictureModelValidator.ValidateAsync(newPictureModel);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            var newPictureDto = _mapper.Map<NewPictureDto>(newPictureModel);
            newPictureDto.UserId = User.GetUserId();

            var creationResult = await _pictureService.AddAsync(newPictureDto);
            if (creationResult.IsSuccess)
            {
                return StatusCode(201, creationResult);
            }
            
            ModelStateExtensions.AddErrors(ModelState, creationResult);
            return BadRequest(ModelState);
        }
    }
}
