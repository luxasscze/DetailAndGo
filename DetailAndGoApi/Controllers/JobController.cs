using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace DetailAndGoApi.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IJobService _jobService;

        public JobController(ILogger<WeatherForecastController> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        [HttpGet("JobById")]
        public string JobById(int? id)
        {
            return _jobService.GetJobById(id).ToJson();
        }

        [HttpGet("GetAllJobs")]
        public string GetAllJobs()
        {
            return _jobService.GetAllJobs().ToJson();
        }

        [HttpPost("AddJob")]
        public void AddJob(Job job)
        {
            _jobService.Addjob(job);
        }

        [HttpDelete("DeleteJob")]
        public void DeleteJob(int id)
        {
            _jobService.DeleteJobById(id);
        }

        [HttpPut("UpdateJobLocation")]
        public void UpdateJobLocation(int id, double lat, double lon)
        {
            _jobService.UpdateJobLocation(id, lat, lon);
        }
    }
}
