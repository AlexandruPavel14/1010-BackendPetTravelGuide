using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models.Response;

namespace PetTravelGuide.Backend.Services.LocationService;

public interface ILocationService
{
    Task<List<Location>> GetAll(long? userId = null);

    Task<Location?> GetById(long id);

    Task<IDataResponse<Location>> Insert(Location toInsert, long userId);

    Task<IDataResponse<Location>> Update(Location toUpdate, long? userId);

    Task<IResponse> DeleteById(long id, long? userId = null);
}