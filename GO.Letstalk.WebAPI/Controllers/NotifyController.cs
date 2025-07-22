namespace GO.Letstalk.WebAPI.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.Letstalk.WebAPI.Constants;
using GO.Letstalk.WebAPI.Enums;
using GO.Letstalk.WebAPI.Models;
using GO.Letstalk.WebAPI.Models.Result;
using GO.Letstalk.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[Authorize]
public class NotifyController(
    ILogger<NotifyController> logger,
    List<WebhookConfiguration> webhookConfigurations,
    Func<int, ILetstalkService> letstalkService) : ControllerBase
{
    private readonly ILogger logger = logger;

    [HttpPost]
    [Route("api/notify")]
    public async Task<ObjectResult> Send([FromBody] NotifyModel model)
    {
        var errorMessage = string.Empty;
        try
        {
            await SendToLestalk(model.BotId, model.ConversationId, model.Message, model.ReplyMarkup);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            logger.LogError(ex, ex.Message);
        }

        return Ok(errorMessage);
    }

    private async Task SendToLestalk(int botId, string conversationId, string message, string replyMarkup)
    {
        var sendResult = new SendNotificationResult
        {
            ConversationId = string.Join(", ", conversationId)
        };

        var outGoingMsg = new OutgoingMessage
        {
            MessageType = MessageTypeNumber.Text,
            Text = message
        };

        var conversationIds = conversationId.Contains(";") ? conversationId.Split(";").ToList() : [conversationId];
        var botConfigs = webhookConfigurations.FirstOrDefault(config => config.BotId == botId);
        if (botConfigs is { IsAllowedSet: true })
        {
            var result = await letstalkService(botConfigs.BotId).PushMulticastAsync(conversationIds, outGoingMsg, replyMarkup);
            if (result.IsSuccess && !result.BlockList.Any())
            {
                sendResult.Status = SendNotificationStatus.Sent;
                sendResult.ErrMsg = string.Empty;
            }

            if (result.FailList.Any())
            {
                sendResult.Status = SendNotificationStatus.Error;
                sendResult.ErrMsg = SendNotificationErrMsg.LetstalkIdInvalidMsg;
            }

            if (result.BlockList.Any())
            {
                sendResult.Status = SendNotificationStatus.BotBlocked;
                sendResult.ErrMsg = string.Empty;
            }

            logger.LogInformation(JsonConvert.SerializeObject(sendResult, Formatting.Indented));
        }
    }
}