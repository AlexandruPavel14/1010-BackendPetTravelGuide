namespace PetTravelGuide.Backend.Models;

public class PasswordString
{
    public string Password { get; set; }

    public PasswordString(string password)
    {
        Password = password;
    }
}
