using Database.Entities;
using DataModels.ApiModels;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class UserRepository(RfidDatabaseContext context) : IUserRepository
{
    public async Task<UserDbEntity?> GetActiveUserByTagEPCAsync(long id)
    {
        return await context.Users
            .Include(u => u.TargetedEvents)
            .Include(u => u.CalledEvents)
            .FirstOrDefaultAsync(u => u.TagIdentifier == id && !u.Deleted);
    }

    public async Task<List<UserDbEntity>> GetAllUsersAsync()
    {
        return await context.Users
            .Include(u => u.TargetedEvents)
            .Include(u => u.CalledEvents)
            .ToListAsync();
    }

    public async Task<UserDbEntity> CreateUserFromTag(Tag tag)
    {
        var newUser = UserDbEntity.Create(tag);
        await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync();
        return newUser;
    }

    public async Task<UserDbEntity?> DeleteUser(Tag tag)
    {
        var user = await GetActiveUserByTagEPCAsync(tag.Id);
        
        if (user != null)
        {
            user.Deleted = true;
            user.DeletedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        return user;
    }

    public async Task<(UserDbEntity?, List<ProcessedEventDbEntity>, List<ProcessedEventDbEntity>)> GetUserAsync(Guid id)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
        var calledEvents = await context.Events.OfType<ProcessedEventDbEntity>()
            .Where(e => e.CallerId == id)
            .Include(e => e.Tools)
            .ToListAsync();
        var targetedEvents = await context.Events.OfType<ProcessedEventDbEntity>()
            .Where(e => e.TargetUserId == id)
            .Include(e => e.Tools)
            .ToListAsync();
        
        return (user, calledEvents, targetedEvents);
    }
}