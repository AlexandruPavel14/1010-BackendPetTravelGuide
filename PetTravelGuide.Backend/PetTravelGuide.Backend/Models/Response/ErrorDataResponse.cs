namespace PetTravelGuide.Backend.Models.Response;

public class ErrorDataResponse<T> : DataResponse<T>
{
    /// <summary>
    /// Constructor with data.
    /// </summary>
    /// <param name="data">The data that will be enclosed in the response.</param>
    public ErrorDataResponse(T data) : base(data, false)
    {
    }

    /// <summary>
    /// Constructor with data and message
    /// </summary>
    /// <param name="data">The data that will be enclosed in the response</param>
    /// <param name="message">Additional message that comes with the data</param>
    public ErrorDataResponse(T data, string message) : base(data, false, message)
    {
    }
}