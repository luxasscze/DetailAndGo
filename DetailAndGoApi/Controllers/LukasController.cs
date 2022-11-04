using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace DetailAndGoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LukasController : ControllerBase
    {       

        private readonly ILogger<WeatherForecastController> _logger;        
        private readonly ICustomerService _customerService;

        public LukasController(ILogger<WeatherForecastController> logger, ICustomerService customerService)
        {
            _logger = logger;            
            _customerService = customerService;
        }

        [HttpGet(Name = "GetLukas")]
        public string Get(string aspNetUserId)
        {
            return _customerService.GetCustomerById(aspNetUserId).ToJson();
        }
    }
}
