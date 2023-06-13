namespace PetTravelGuide.Backend.Models.Response;

public class SuccessResponse : Response
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SuccessResponse() : base(true)
    {
    }

    /// <summary>
    /// Constructor with message
    /// </summary>
    /// <param name="message">The message to insert into the response</param>
    public SuccessResponse(string? message) : base(true, message)
    {
    }
}