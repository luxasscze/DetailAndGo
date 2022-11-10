using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;

namespace DetailAndGo.Services
{
    public class JobService : IJobService
    {
        public ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {

            _context = context;

        }

        public List<Job> GetAllJobs()
        {
            return _context.Jobs.ToList();
        }

        public Job GetJobById(int? jobId)
        {
            return _context.Jobs.Where(s => s.Id == jobId).FirstOrDefault();
        }

        public List<Job> GetJobsByUserId(string? userId)
        {
            return _context.Jobs.Where(s => s.CustomerId == userId).ToList();
        }

        public void Addjob(Job job)
        {
            _context.Jobs.Add(job);
            _context.SaveChanges(); 
        }

        public void DeleteJobById(int? id)
        {
            Job jobToDelete = _context.Jobs.Where(s => s.Id == id).FirstOrDefault();
            _context.Jobs.Remove(jobToDelete);
            _context.SaveChanges(); 
        }

        public void UpdateJobLocation(int? id, double lat, double lon)
        {
            Job jobToUpdate = _context.Jobs.Where(s => s.Id == id).FirstOrDefault();
            jobToUpdate.LocationLat = lat;
            jobToUpdate.LocationLon = lon;
            _context.Jobs.Attach(jobToUpdate);
            _context.Entry(jobToUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
