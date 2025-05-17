using Database.Entities;
using Database.Repositories;
using DataModels.ApiModels;
using DataModels.Models;

namespace MqttWorkerService;

public class BorrowMessageHandler(IUserRepository userRepository, IToolRepository toolRepository, IEventRepository eventRepository, ILogger<BorrowMessageHandler> logger) 
    : BaseMessageHandler<BorrowMessage>(logger, eventRepository)
{
    
    [Obsolete("Do not use this method")]
    public override async Task<bool> Handle(BorrowMessage @event)
    {
       return await toolRepository.UpdateToolBorrowInfo(@event, new EventBaseDbEntity());
    }

    public override async Task<HandleEventResponse> Handle(EventBaseDbEntity @event)
    {
        var data = Extract(@event);
        var caller = data.UserId.HasValue ? await userRepository.GetActiveUserByTagEPCAsync(data.UserId.Value) : null;

        var success = await toolRepository.UpdateToolBorrowInfo(data, @event, caller?.Id);
        return new HandleEventResponse
        {
            Success = success,
            CallerId = caller?.Id,
            HackParameter = true
        };
    }
}
