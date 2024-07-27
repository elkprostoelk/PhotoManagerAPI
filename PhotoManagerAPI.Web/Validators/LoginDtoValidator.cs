using FluentValidation;
using PhotoManagerAPI.Core.DTO;

namespace PhotoManagerAPI.Web.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(dto => dto.UserNameOrEmail)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(dto => dto.Password)
                .NotEmpty()
                .Length(8, 20);
        }
    }
}
