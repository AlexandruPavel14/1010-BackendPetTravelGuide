namespace PetTravelGuide.Backend.Models.Response;

public class SuccessDataResponse<T> : DataResponse<T>
{
    /// <summary>
    /// Constructor with data
    /// </summary>
    /// <param name="data">The data that will be enclosed in the response</param>
    public SuccessDataResponse(T data) : base(data, true)
    {
    }

    /// <summary>
    /// Constructor with data and message
    /// </summary>
    /// <param name="data">The data that will be enclosed in the response</param>
    /// <param name="message">Additional message that comes with the data</param>
    public SuccessDataResponse(T data, string message) : base(data, true, message)
    {
    }
}