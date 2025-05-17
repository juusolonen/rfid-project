using System.Text.Json;
using Database;
using Database.Entities;

namespace MqttWorkerService.MessageHandlers;

public class EventModifierDispatcher(IServiceProvider serviceProvider, ILogger<EventModifierDispatcher> logger)
{
    public async Task<bool> Dispatch(EventBaseDbEntity eventEntity)
    {

        var root = @eventEntity.Data.RootElement;
        if (root.TryGetProperty(MqttWorkerServiceConstants.ActionTypeDiscriminator, out JsonElement action))
        {
            var actionString = action.GetString();
            if (string.IsNullOrWhiteSpace(actionString))
            {
                eventEntity.SetFaulted("Empty action");
                return false;
            }

            var modelType = DataModels.Utility.MessageTypeResolver.GetModelType(actionString);

            var dbEntityType = typeof(EventBaseDbEntity);
            var messageHandlerType = typeof(IMessageHandler<,>).MakeGenericType(modelType,dbEntityType);
            var service = serviceProvider.GetRequiredService(messageHandlerType);
            var handler = messageHandlerType.GetMethod("Execute", [dbEntityType]);

            try
            {
                var result = handler.Invoke(service, [eventEntity]);

                if (result is Task<bool> invokedTask)
                {
                    return await invokedTask;
                } else if (result is Task anotherTask)
                {
                    await anotherTask;
                }
            
                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while processing action: {@actionString}, error: {error}", actionString, e.Message);
                return false;
            }
        }
        else
        {
            eventEntity.SetFaulted("Empty action");
            Console.WriteLine("Error: property 'action' is missing.");
            return false;
        }

    }
}
