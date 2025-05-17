using Database.Entities;
using DataModels.ApiModels;

namespace Database.Repositories;

public interface IUserRepository
{
    public Task<UserDbEntity?> GetActiveUserByTagEPCAsync(long id);

    public Task<List<UserDbEntity>> GetAllUsersAsync();
    
    public Task<UserDbEntity> CreateUserFromTag(Tag tag);
    
    public Task<UserDbEntity?> DeleteUser(Tag tag);
    
    public Task<(UserDbEntity?, List<ProcessedEventDbEntity>, List<ProcessedEventDbEntity>)> GetUserAsync(Guid id);
}