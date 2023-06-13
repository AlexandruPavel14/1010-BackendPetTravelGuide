namespace PetTravelGuide.Backend.Models.Response;

public abstract class DataResponse<T> : Response, IDataResponse<T>
{
    /// <inheritdoc />
    public T Data { get; }

    /// <summary>
    /// Constructor with data and success value
    /// </summary>
    /// <param name="data">The data that will be enclosed in the response</param>
    /// <param name="isSuccess">Value to signal if result is successful or an error</param>
    protected DataResponse(T data, bool isSuccess) : base(isSuccess)
    {
        Data = data;
    }

    /// <summary>
    /// Constructor with data, success value and message
    /// </summary>
    /// <param name="data">The data that will be enclosed in the response</param>
    /// <param name="isSuccess">Value to signal if result is successful or an error</param>
    /// <param name="message">Additional message to send with the data</param>
    protected DataResponse(T data, bool isSuccess, string? message) : base(isSuccess, message)
    {
        Data = data;
    }
}
