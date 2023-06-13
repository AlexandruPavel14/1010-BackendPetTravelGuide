using FluentValidation;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models.AppSettings;

namespace PetTravelGuide.Backend.Validators;

public class UserValidator : AbstractValidator<User>
{
	public UserValidator(UserServiceSettings userServiceSettings)
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email-ul nu a fost specificat")
			.EmailAddress().WithMessage("Email invalid");

		RuleFor(x => x.FirstName)
			.NotEmpty().WithMessage("Prenumele nu a fost specificat")
			.MinimumLength(userServiceSettings.NameMinLength)
				.WithMessage($"Prenumele trebuie sa contina cel putin {userServiceSettings.NameMinLength} caractere")
			.MaximumLength(userServiceSettings.NameMaxLength)
				.WithMessage($"Prenumele trebuie sa contina maxim {userServiceSettings.NameMaxLength} caractere");

		RuleFor(x => x.LastName)
			.NotEmpty()
				.WithMessage("Numele nu a fost specificat")
			.MinimumLength(userServiceSettings.NameMinLength)
				.WithMessage($"Numele trebuie sa contina cel putin {userServiceSettings.NameMinLength} caractere")
			.MaximumLength(userServiceSettings.NameMaxLength)
				.WithMessage($"Numele trebuie sa contina maxim {userServiceSettings.NameMaxLength} caractere");
	}
}
