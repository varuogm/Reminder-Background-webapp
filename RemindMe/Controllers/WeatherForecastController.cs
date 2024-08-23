using Microsoft.AspNetCore.Mvc;
using WebPush;

namespace RemindMe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        [HttpGet(Name = "GenerateKeys")]
        public VapidDetails Get()
        {
            var vapidKeys = VapidHelper.GenerateVapidKeys();
            Console.WriteLine($"Public key: {vapidKeys.PublicKey}");
            Console.WriteLine($"Private key: {vapidKeys.PrivateKey}");
            return vapidKeys;
        }
    }
}
