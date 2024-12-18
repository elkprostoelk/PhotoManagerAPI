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

        [HttpGet]
        public async Task<IActionResult> SearchPictures([FromQuery]SearchPicturesDto searchPicturesDto, CancellationToken cancellationToken)
        {
            return Ok(await _pictureService.SearchAsync(searchPicturesDto, cancellationToken));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPicture(Guid id)
        {
            var picture = await _pictureService.GetAsync(id);
            return picture is not null
                ? Ok(picture)
                : NotFound();
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

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePicture(Guid id)
        {
            var deletingResult = await _pictureService.DeletePictureAsync(id, User.GetUserId());

            if (deletingResult.IsSuccess)
            {
                return NoContent();
            }
            
            ModelStateExtensions.AddErrors(ModelState, deletingResult);
            return BadRequest(ModelState);
        }
    }
}
