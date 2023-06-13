using FluentValidation;
using PetTravelGuide.Backend.Extensions;
using PetTravelGuide.Backend.Models;
using PetTravelGuide.Backend.Models.AppSettings;

namespace PetTravelGuide.Backend.Validators;

public class PasswordStrengthValidator : AbstractValidator<PasswordString>
{
    public PasswordStrengthValidator(UserServiceSettings userServiceSettings)
    {
        RuleFor(x => x.Password)
            .PasswordStrength(userServiceSettings);
    }
}
