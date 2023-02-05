using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface IJobService
    {
        public Job GetJobById(int? jobId);
        public List<Job> GetJobsByUserId(string? userId);
        public void Addjob(Job job);
        public void DeleteJobById(int? id);
        public void UpdateJobLocation(int? id, double lat, double lon);
        public List<Job> GetAllJobs();
        public Task<List<int>> GetAvailableTimesForDate(DateTime date);
        public string ConvertTimeFromIntToString(int time);
    }
}
