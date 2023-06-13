namespace PetTravelGuide.Backend.Models.Response;

public interface IResponse
{
    /// <summary>
    /// Value to signal if result is successful or an error
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// The generic message. Used in text results or errors.
    /// </summary>
    string? Message { get; }
}