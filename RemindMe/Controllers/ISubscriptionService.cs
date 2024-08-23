using WebPush;

public interface ISubscriptionService
{
    Task AddSubscriptionAsync(PushSubscription subscription);
    Task<IEnumerable<PushSubscription>> GetAllSubscriptionsAsync();
    Task RemoveSubscriptionAsync(string endpoint);
}