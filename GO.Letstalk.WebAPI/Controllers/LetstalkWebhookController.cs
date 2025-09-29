namespace GO.Letstalk.WebAPI.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.Letstalk.WebAPI.EventHandler;
using GO.Letstalk.WebAPI.Models;
using GO.Letstalk.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

public class LetstalkWebhookController(
    ILetsTalkEventHandler eventHandler,
    Func<int, ILetstalkService> letstalkService,
    List<WebhookConfiguration> webhookConfigurations) : ControllerBase
{
    [Route("api/webhook/{id}")]
    public async Task WebhookReceiver(int id)
    {
        await eventHandler.ReceiveAsync(Request, id);
    }

    [Route("api/testwebhook/{id}")]
    public async Task<IActionResult> TestWebhookReceiver(int id)
    {
        var res = await eventHandler.TestReceiveAsync(Request, id);
        return Ok(res);
    }

    [Route("api/setwebhook/{id}")]
    public async Task<IActionResult> SetWebhook(int id)
    {
        var botConfigs = webhookConfigurations.FirstOrDefault(config => config.BotId == id);
        if (botConfigs is { IsAllowedSet: true })
        {
            var webhookUrl = $"{botConfigs.WebhookBaseUrl}api/webhook/{id}";
            var result = await letstalkService(id).SetWebhookAsync(webhookUrl);
            return Ok(result);
        }

        return NotFound();
    }

    [Route("api/getwebhook/{id}")]
    public async Task<IActionResult> GetWebhook(int id)
    {
        var botConfigs = webhookConfigurations.FirstOrDefault(config => config.BotId == id);
        if (botConfigs is { IsAllowedGet: true })
        {
            var result = await letstalkService(id).GetWebhookAsync();
            return Ok(result);
        }

        return NotFound();
    }
}