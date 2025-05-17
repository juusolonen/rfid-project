using Database.Entities;
using Database.Repositories;
using DataModels.ApiModels;
using DataModels.Models;

namespace MqttWorkerService;

public class DeleteHandler(IUserRepository userRepository, IToolRepository toolRepository, IEventRepository eventRepository, ILogger<DeleteHandler> logger)
    : TagMessageHandler<DeleteTag, EventBaseDbEntity>(logger, eventRepository)
{
    
    [Obsolete("This method should not be used. Use the overload that accepts EventBaseDbEntity.")]
    public override async Task<bool> Handle(DeleteTag @event)
    {

        // if (@event.Tag.TagType == TagType.USER)
        // {
        //     var user = await userRepository.DeleteUser(@event.Tag);
        //
        //     logger.LogInformation("Removed user {@user}", user);    
        // } else if (@event.Tag.TagType == TagType.TOOL)
        // {
        //     var tool = await toolRepository.DeleteTool(@event.Tag);
        //
        //     logger.LogInformation("Removed user {@tool}", tool);
        // }

        return true;
    }

    public override async Task<HandleEventResponse> Handle(EventBaseDbEntity @event)
    {
        var data = Extract(@event);
        var caller = data.UserId.HasValue ? await userRepository.GetActiveUserByTagEPCAsync(data.UserId.Value) : null;

        if (data.Tag.TagType == TagType.USER)
        {
            var user = await userRepository.DeleteUser(data.Tag);
            logger.LogInformation("Removed user {@user}", user);

            return user == null 
                ? new HandleEventResponse 
                {
                    Success = false,
                    FaultReason = "User not found",
                    CallerId = caller?.Id,
                    TagType = TagType.USER,
                } 
                : new HandleEventResponse 
                {
                    Success = true,
                    TargetId = user.Id,
                    CallerId = caller?.Id,
                    TagType = TagType.USER,
                };
            
        } else if (data.Tag.TagType == TagType.TOOL)
        {
            var tool = await toolRepository.DeleteTool(data.Tag, @event);
            logger.LogInformation("Removed tool {@tool}", tool);
            return tool == null 
                ? new HandleEventResponse 
                {
                    Success = false,
                    FaultReason = "Tool not found",
                    CallerId = caller?.Id,
                    TagType = TagType.TOOL,
                } 
                : new HandleEventResponse 
                {
                    Success = true,
                    TargetId = tool.Id,
                    CallerId = caller?.Id,
                    TagType = TagType.TOOL,
                };
        }

        return new HandleEventResponse 
        {
            Success = false,
            FaultReason = "Invalid",
            CallerId = caller?.Id,
            TagType = TagType.TOOL,
        } ;
    }
}

