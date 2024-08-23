using Microsoft.AspNetCore.Mvc;
using WebPush;
using System.Threading.Tasks;
using RemindMe;  // Make sure this namespace is correct for your project

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] PushSubscription subscription)
    {
        if (subscription == null)
        {
            return BadRequest("Invalid subscription data");
        }

        await _subscriptionService.AddSubscriptionAsync(subscription);
        return Ok("Subscription added successfully");
    }

    [HttpDelete("{endpoint}")]
    public async Task<IActionResult> Unsubscribe(string endpoint)
    {
        await _subscriptionService.RemoveSubscriptionAsync(endpoint);
        return Ok("Subscription removed successfully");
    }

    // You might want to add a GET method to fetch all subscriptions, if needed
    [HttpGet]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
        return Ok(subscriptions);
    }
}