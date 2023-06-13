using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models.Response;

namespace PetTravelGuide.Backend.Services.PetService;

public interface IPetService
{
    Task<List<Pet>> GetAll(long userId);

    Task<Pet?> GetById(long id, long userId);

    Task<IDataResponse<Pet>> Insert(Pet toInsert, long userId);

    Task<IDataResponse<Pet>> Update(Pet toUpdate, long userId);

    Task<IResponse> DeleteById(long id, long userId);
}