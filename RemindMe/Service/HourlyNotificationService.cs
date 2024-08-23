using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebPush;

namespace RemindMe.Service
{
    public class HourlyNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HourlyNotificationService> _logger;

        public HourlyNotificationService(IServiceProvider serviceProvider, ILogger<HourlyNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                try
                {
                    await SendNotifications();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending notifications");
                }
            }
        }

        private async Task SendNotifications()
        {
            using var scope = _serviceProvider.CreateScope();
            var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
            var webPushClient = scope.ServiceProvider.GetRequiredService<WebPushClient>();
            var subscriptions = await subscriptionService.GetAllSubscriptionsAsync();

            foreach (var subscription in subscriptions)
            {
                try
                {
                    var payload = @"{""message"":""Hi there!""}";
                    await webPushClient.SendNotificationAsync(subscription, payload);
                }
                catch (WebPushException ex)
                {
                    _logger.LogError(ex, $"Error sending notification to subscription {subscription.Endpoint}");
                }
            }
        }

    }
}