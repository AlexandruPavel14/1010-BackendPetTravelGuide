using FluentValidation;
using PetTravelGuide.Backend.Models.AppSettings;

namespace PetTravelGuide.Backend.Extensions;

public static class RuleBuilderExtensions
{
    public static IRuleBuilder<T, string?> PasswordStrength<T>(
	    this IRuleBuilder<T, string?> ruleBuilder,
	    UserServiceSettings userServiceSettings) =>
	    ruleBuilder
		    .NotEmpty()
				.WithMessage("Parola nu a fost specificata")
		    .MinimumLength(userServiceSettings.PasswordMinLength)
				.WithMessage($"Parola trebuies sa contina cel putin {userServiceSettings.PasswordMinLength} caractere")
		    .MaximumLength(userServiceSettings.PasswordMaxLength)
				.WithMessage($"Parola trebuie sa contina maxim {userServiceSettings.PasswordMaxLength} caractere")
		    .Custom((password, context) =>
		    {
			    if (!(
				        password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit) && 
				        (
					        password.Any(char.IsPunctuation) || 
							password.Any(c => c is '@' or '#' or '$' or '%' or '^' or '&' or '*' or '(' or ')' or '+' 
					         or '=' or '{' or '}' or '[' or ']' or '/' or '\\' or '|')
					    )
				    )) 
			    {
				    context.AddFailure($"Parola trebuie sa contina cel putin {userServiceSettings.PasswordMinLength} caractere din care, cel putin o litera minuscula, cel putin o litera majuscula, cel putin o cifra si cel putin un semn de punctiatie.");
			    }
		    });
}
