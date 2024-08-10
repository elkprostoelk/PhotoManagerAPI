using FluentValidation;
using Microsoft.Extensions.Options;
using PhotoManagerAPI.Core.Configurations;
using PhotoManagerAPI.Web.Models;

namespace PhotoManagerAPI.Web.Validators
{
    public class NewPictureModelValidator : AbstractValidator<NewPictureModel>
    {
        public NewPictureModelValidator(IOptions<ImageOptions> imageOptions)
        {
            RuleFor(m => m.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(m => m.Description)
                .MaximumLength(2000);

            RuleFor(m => m.Width)
                .GreaterThan(0);

            RuleFor(m => m.Height)
                .GreaterThan(0);

            RuleFor(m => m.Iso)
                .MaximumLength(15);

            RuleFor(m => m.CameraModel)
                .MaximumLength(20);

            RuleFor(m => m.FocusDistance)
                .MaximumLength(10);

            RuleFor(m => m.DelayTimeMilliseconds)
                .GreaterThanOrEqualTo(0f);

            RuleFor(m => m.ShootingDate)
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(m => m.File)
                .NotNull()
                .Must(file =>
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    return imageOptions.Value.AllowedFileTypes.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
                }).WithMessage($"File must be of one of the types: {string.Join(", ", imageOptions.Value.AllowedFileTypes)}")
                .Must(file => file.Length <= imageOptions.Value.MaxFileSizeBytes)
                .WithMessage($"File must be less than or equal to {imageOptions.Value.MaxFileSizeBytes / 1024} KB!");
        }
    }
}
