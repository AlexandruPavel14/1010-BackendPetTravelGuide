namespace PetTravelGuide.Backend.Models.Response;

public interface IDataResponse<out T> : IResponse
{
    /// <summary>
    /// The response data
    /// </summary>
    T Data { get; }
}
