using Microsoft.EntityFrameworkCore;
using PetTravelGuide.Backend.Database;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models.Response;

namespace PetTravelGuide.Backend.Services.PetService;

public class PetService : IPetService
{
    private readonly DatabaseContext _database;
    private readonly ILogger<PetService> _logger;
    
    public PetService(
        DatabaseContext database,
        ILogger<PetService> logger)
    {
        _database = database;
        _logger = logger;
    }
    
    // ALWAYS restrict pets by userId
    public async Task<List<Pet>> GetAll(long userId)
    {
        return await _database.Pets.Where(p => p.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Pet?> GetById(long id, long userId)
    {
        return await _database.Pets
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
    }

    public async Task<IDataResponse<Pet>> Insert(Pet toInsert, long userId)
    {
        toInsert.UserId = userId;
        await _database.AddAsync(toInsert);
        await _database.SaveChangesAsync();
        _logger.LogInformation("Pet with Id {Id} created successfully", toInsert.Id);
        return new SuccessDataResponse<Pet>(toInsert, "Pet created successfully");
    }

    public async Task<IDataResponse<Pet>> Update(Pet toUpdate, long userId)
    {
        if (toUpdate.UserId != userId)
        {
            _logger.LogWarning("Tried to update a pet with UserId {CurrentUserId} for the user with Id {UserId}", toUpdate.UserId, userId);
            return new ErrorDataResponse<Pet>(null!, "User ids do not match");
        }
        
        toUpdate.CreatedAt = DateTime.Now;
        _database.Attach(toUpdate);
        _database.Update(toUpdate);
        await _database.SaveChangesAsync();
        _logger.LogInformation("Pet with Id {Id} updated successfully", toUpdate.Id);
        
        return new SuccessDataResponse<Pet>(toUpdate, "Pet updated successfully");
    }

    public async Task<IResponse> DeleteById(long id, long userId)
    {
        Pet? pet = await _database.Pets
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (pet is null)
        {
            return new ErrorResponse("Pet with given Id does not exist");
        }

        _database.Remove(pet);
        await _database.SaveChangesAsync();
        return new SuccessResponse("Pet deleted successfully");
    }
}