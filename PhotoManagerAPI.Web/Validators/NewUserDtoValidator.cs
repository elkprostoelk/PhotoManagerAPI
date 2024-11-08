using FluentValidation;
using PhotoManagerAPI.Core.DTO;

namespace PhotoManagerAPI.Web.Validators;

public class NewUserDtoValidator: AbstractValidator<NewUserDto>
{
    private const string PasswordRegex = "[0-9Z-Za-z+-_#$@!*&?:/<>]+";
    
    public NewUserDtoValidator()
    {
        RuleFor(dto => dto.UserName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(dto => dto.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50);

        RuleFor(dto => dto.Role)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(dto => dto.FullName)
            .MaximumLength(100);

        RuleFor(dto => dto.Password)
            .NotEmpty()
            .Length(8, 20)
            .Matches(PasswordRegex);
        
        RuleFor(dto => dto.ConfirmPassword)
            .NotEmpty()
            .Length(8, 20)
            .Matches(PasswordRegex)
            .Equal(dto => dto.Password, StringComparer.Ordinal);
    }
}