
    using Microsoft.EntityFrameworkCore;
    using RemindMe.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WebPush;

namespace RemindMe.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PushSubscription>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _context.Subscriptions.ToListAsync();
            return subscriptions.Select(s => new PushSubscription(s.Endpoint, s.P256dh, s.Auth));
        }

        public async Task AddSubscriptionAsync(PushSubscription subscription)
        {
            var newSubscription = new Subscription
            {
                Endpoint = subscription.Endpoint,
                P256dh = subscription.P256DH,
                Auth = subscription.Auth,
                CreatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(newSubscription);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveSubscriptionAsync(string endpoint)
        {
            var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Endpoint == endpoint);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }
    }
}
    