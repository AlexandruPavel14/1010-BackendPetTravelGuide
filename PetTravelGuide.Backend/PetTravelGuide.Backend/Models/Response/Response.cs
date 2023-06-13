namespace PetTravelGuide.Backend.Models.Response;

public abstract class Response : IResponse
{
    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public string? Message { get; }

    /// <summary>
    /// Constructor with only success value.
    /// </summary>
    /// <param name="isSuccess">Value to signal if result is successful or an error.</param>
    protected Response(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Constructor with success value and message
    /// </summary>
    /// <param name="isSuccess">Value to signal if result is successful or an error.</param>
    /// <param name="message">Additional message to send with the data.</param>
    protected Response(bool isSuccess, string? message) : this(isSuccess)
    {
        Message = message;
    }
}