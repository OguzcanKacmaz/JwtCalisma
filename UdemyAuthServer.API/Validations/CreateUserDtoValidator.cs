using FluentValidation;
using UdemyAuthServer.Core.DTOs;

namespace UdemyAuthServer.API.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x=>x.Email).NotEmpty().WithMessage("Email Boş Olamaz.").EmailAddress().WithMessage("Email Adresini Kontrol Ediniz.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre Boş Olamaz");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı Adı Boş Olamaz");
        }
    }
}
