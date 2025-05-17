using Database.Entities;
using Database.Repositories;
using DataModels.ApiModels;
using DataModels.Models;

namespace MqttWorkerService;

public  class NewHandler(ILogger<NewHandler> logger, IEventRepository eventRepository, IUserRepository userRepository, IToolRepository toolRepository) 
    : TagMessageHandler<NewTag, EventBaseDbEntity>(logger, eventRepository)
{
    
    public override async Task<bool> Handle(NewTag @event)
    {
        throw new NotImplementedException();
        // if (@event.Tag.TagType == TagType.USER)
        // {
        //     var existingUser = await userRepository.GetActiveUserByTagEPCAsync(@event.Tag.Id);
        //
        //     if (existingUser == null)
        //     {
        //         var newUser = await userRepository.CreateUserFromTag(@event.Tag);
        //         logger.LogInformation("Created new user: {@newUser}", newUser);
        //     }
        // } else if (@event.Tag.TagType == TagType.TOOL)
        // {
        //     var existingTool = await toolRepository.GetActiveToolByTagEPCAsync(@event.Tag.Id);
        //
        //     if (existingTool == null)
        //     {
        //         var newTool = await toolRepository.CreateToolFromTag(@event.Tag);
        //         logger.LogInformation("Created new tool: {@newTool}", newTool);
        //     }
        // }
        //
        // return true;
    }

    public override async Task<HandleEventResponse> Handle(EventBaseDbEntity @event)
    {
        var data = Extract(@event);

        var caller = data.UserId.HasValue ? await userRepository.GetActiveUserByTagEPCAsync(data.UserId.Value) : null;
        
        if (data.Tag.TagType == TagType.USER)
        {
            var existingUser = await userRepository.GetActiveUserByTagEPCAsync(data.Tag.Id);

            if (existingUser != null)
            {
                return new HandleEventResponse
                {
                    Success = false,
                    FaultReason = "User already exists"
                };
            }
        
            var newUser = await userRepository.CreateUserFromTag(data.Tag);
            logger.LogInformation("Created new user: {@newUser}", newUser);
       
            return new HandleEventResponse
            {
                Success = true,
                TargetId = newUser.Id,
                CallerId = caller?.Id,
                TagType = data.Tag.TagType,
            };
        } else if (data.Tag.TagType == TagType.TOOL)
        {
            var existingTool = await toolRepository.GetActiveToolByTagEPCAsync(data.Tag.Id);

            if (existingTool != null)
            {
                return new HandleEventResponse
                {
                    Success = false,
                    FaultReason = "Tool already exists",
                };
            }
            
            var newTool = await toolRepository.CreateToolFromTag(data.Tag, @event);
            logger.LogInformation("Created new tool: {@newTool}", newTool);
            
            return new HandleEventResponse
            {
                Success = true,
                TargetId = newTool.Id,
                CallerId = caller?.Id,
                TagType = data.Tag.TagType,
            };
        }
        
        return new HandleEventResponse
        {
            Success = false,
            FaultReason = "something happened",
        };
    }
}
