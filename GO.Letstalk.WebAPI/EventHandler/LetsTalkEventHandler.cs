using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GO.Letstalk.WebAPI.Constants;
using GO.Letstalk.WebAPI.Enums;
using GO.Letstalk.WebAPI.Models;
using GO.Letstalk.WebAPI.Models.Webhook;
using GO.Letstalk.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GO.Letstalk.WebAPI.EventHandler;

public interface ILetsTalkEventHandler
{
    Task ReceiveAsync(HttpRequest request, int botId);
}

public class LetsTalkEventHandler(
    Func<int, ILetstalkService> letstalkService,
    ICryptographyService cryptographyService,
    IJwtService jwtService,
    IHRMConnector hrmConnector,
    List<BotConfig> botConfigs)
    : ILetsTalkEventHandler
{
    public async Task ReceiveAsync(HttpRequest request, int botId)
    {
        CheckValidBot(botConfigs, botId);
        var botConfig = botConfigs.Find(config => config.BotAccountId == botId);
        if (botConfig is not null)
        {
            string requestJson;
            using (TextReader reader = new StreamReader(request.Body))
            {
                requestJson = await reader.ReadToEndAsync();
            }

            var rawData = jwtService.Decode(botConfig.SecureKey, requestJson.Trim());
            var data = JObject.Parse(rawData);
            var hookEvent = data.ToObject<HookEvent>();
            Console.WriteLine(hookEvent);
            await EventHandlerAsync(hookEvent, botConfig);
        }
    }

    private async Task EventHandlerAsync(HookEvent hookEvent, BotConfig botConfig)
    {
        switch (hookEvent.Type)
        {

            case EventType.Message:
                await MessageHandlerAsync(hookEvent, botConfig);
                break;
            default:
                throw new KeyNotFoundException($"The event type {hookEvent.Type} have not implements!");
        }
    }

    private static void CheckValidBot(List<BotConfig> botConfigs, int botId)
    {
        if (!botConfigs.Exists(config => config.BotAccountId == botId))
        {
            throw new KeyNotFoundException($"Not found the bot ID {botId} in configurations!");
        }
    }

    #region Receive message

    private async Task MessageHandlerAsync(HookEvent hookEvent, BotConfig botConfig)
    {
        hookEvent.PayloadObject = JsonConvert.DeserializeObject<Payload>(hookEvent.EncodedPayload);
        var decryptedDataEvent = cryptographyService.Decrypt(hookEvent.PayloadObject.HashContent, botConfig.ChatKey, hookEvent.PayloadObject.HashContentKey);
        var dataEvent = JsonConvert.DeserializeObject<PostBackEvent>(decryptedDataEvent);
        if (!string.IsNullOrEmpty(dataEvent.PostBack.Data))
        {
            var result = await hrmConnector.Send(dataEvent.PostBack.Data);
            var message = "Successfully!";
            if (!result.IsSuccessful)
            {
                message =  $"{result.Message} This request has been processed.";
            }
            await letstalkService(botConfig.BotAccountId).PushMulticastAsync([hookEvent.Source.UserId],
                    new()
                    {
                        Text = message,
                        MessageType = MessageTypeNumber.Text
                    });
        }
    }

    #endregion Receive message
}