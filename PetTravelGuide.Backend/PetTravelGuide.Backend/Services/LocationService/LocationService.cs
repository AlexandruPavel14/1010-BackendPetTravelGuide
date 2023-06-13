using Microsoft.EntityFrameworkCore;
using PetTravelGuide.Backend.Database;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models.Response;

namespace PetTravelGuide.Backend.Services.LocationService;

public class LocationService : ILocationService
{
    private readonly DatabaseContext _database;
    private readonly ILogger<LocationService> _logger;

    public LocationService(
        DatabaseContext database,
        ILogger<LocationService> logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<List<Location>> GetAll(long? userId = null)
    {
        IQueryable<Location> query = _database.Locations.AsQueryable();
        if (userId is not null)
        {
            query = query.Where(l => l.UserId == userId.Value);
        }
        
        return await query.AsNoTracking()
            .ToListAsync();
    }

    public async Task<Location?> GetById(long id)
    {
        return await _database.Locations.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IDataResponse<Location>> Insert(Location toInsert, long userId)
    {
        toInsert.UserId = userId;
        await _database.AddAsync(toInsert);
        await _database.SaveChangesAsync();
        _logger.LogInformation("Location with Id {Id} created successfully", toInsert.Id);
        return new SuccessDataResponse<Location>(toInsert, "Location created successfully");
    }

    public async Task<IDataResponse<Location>> Update(Location toUpdate, long? userId)
    {
        if (userId != null && toUpdate.UserId != userId)
        {
            _logger.LogWarning("Tried to update a location with UserId {CurrentUserId} for the user with Id {UserId}", toUpdate.UserId, userId);
            return new ErrorDataResponse<Location>(null!, "User ids do not match");
        }

        _database.Attach(toUpdate);
        _database.Update(toUpdate);
        await _database.SaveChangesAsync();
        _logger.LogInformation("Location with Id {Id} updated successfully", toUpdate.Id);

        return new SuccessDataResponse<Location>(toUpdate, "Location updated successfully");
    }

    public async Task<IResponse> DeleteById(long id, long? userId)
    {
        IQueryable<Location> query = _database.Locations.AsQueryable();
        if (userId is not null)
        {
            query = query.Where(l => l.UserId == userId.Value);
        }

        query = query.Where(l => l.Id == id);

        Location? location = await query.FirstOrDefaultAsync();
        if (location is null)
        {
            return new ErrorResponse("Location with given Id does not exist");
        }

        _database.Remove(location);
        await _database.SaveChangesAsync();
        return new SuccessResponse("Location deleted successfully");
    }
}