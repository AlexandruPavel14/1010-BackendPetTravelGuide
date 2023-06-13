namespace PetTravelGuide.Backend.Models.Response;

public class ErrorResponse : Response
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public ErrorResponse() : base(false)
    {
    }

    /// <summary>
    /// Constructor with message
    /// </summary>
    /// <param name="message">The message to insert into the response</param>
    public ErrorResponse(string? message) : base(false, message)
    {
    }
}