using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<int>> GetAvailableTimesForDate(DateTime date)
        {
            List<int> times = GetAllTimes(_context.GeneralSettings.FirstOrDefault(s => s.Name == "TimeStart").Value, _context.GeneralSettings.FirstOrDefault(s => s.Name == "TimeEnd").Value);
            int toSkip = int.Parse(_context.GeneralSettings.FirstOrDefault(s => s.Name == "TimeBetweenJobs").Value);
            List<Job> allTodayJobs = await _context.Jobs.Where(s => s.BookingDate.Date == date.Date).ToListAsync();

            if (allTodayJobs.Count > 0)
            {
                foreach (Job job in allTodayJobs)
                {
                    int time = int.Parse(job.BookingDate.ToString("HH:mm").Replace(":", ""));
                    int timeIndex = times.IndexOf(time);

                    if (timeIndex > toSkip && timeIndex < times.Count - toSkip)
                    {
                        times.RemoveRange(timeIndex - toSkip, toSkip * 2 + 1);
                    }
                    else if(timeIndex == times.Count - 1)
                    {
                        times.RemoveRange(timeIndex - toSkip + 1, toSkip);                        
                    }
                    else if(timeIndex > times.Count - toSkip)
                    {
                        times.RemoveRange(timeIndex, times.Count - timeIndex);
                        times.RemoveRange(timeIndex - toSkip, toSkip);
                    }
                    else
                    {
                        times.RemoveRange(0, timeIndex + toSkip);
                    }
                }
            }

            return times;
        }

        public List<int> GetAllTimes(string? starts, string? ends)
        {
            List<int> times = new List<int>()
            {
                0000, 0030, 0100, 0130, 0200, 0230,
                0300, 0330, 0400, 0430, 0500, 0530,
                0600, 0630, 0700, 0730, 0800, 0830,
                0900, 0930, 1000, 1030, 1100, 1130,
                1200, 1230, 1300, 1330, 1400, 1430,
                1500, 1530, 1600, 1630, 1700, 1730,
                1800, 1830, 1900, 1930, 2000, 2030,
                2100, 2130, 2200, 2230, 2300, 2330,
            };

            if (starts == null && ends == null)
            {
                return times;
            }
            else
            {
                return times.FindAll(s => s >= int.Parse(starts) && s <= int.Parse(ends));
            }
        }

        public string ConvertTimeFromIntToString(int time)
        {
            string firstPart = string.Empty;
            string secondPart = string.Empty;

            if (time < 999)
            {
                firstPart = "0" + time.ToString().Substring(0, 1);
                secondPart = time.ToString().Substring(1, 2);
            }
            else
            {
                firstPart = time.ToString().Substring(0, 2);
                secondPart = time.ToString().Substring(2, 2);
            }

            return firstPart + ":" + secondPart;
        }

        public async Task UpdateTimes(string startTime, string endTime, string toSkip)
        {
            GeneralSettings startTimeSettings = new GeneralSettings()
            {
                Name = "TimeStart",
                Value = startTime,
            };
            GeneralSettings endTimeSettings = new GeneralSettings()
            {
                Name = "TimeEnd",
                Value = endTime,
            };
            GeneralSettings toSkipSettings = new GeneralSettings()
            {
                Name = "TimeBetweenJobs",
                Value = toSkip,
            };

            _context.GeneralSettings.RemoveRange(_context.GeneralSettings.ToList());
            _context.GeneralSettings.UpdateRange(startTimeSettings, endTimeSettings, toSkipSettings);
            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<string, string>> GetAllSettings()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            List<GeneralSettings> allSettings = await _context.GeneralSettings.ToListAsync();

            values.Clear();
            foreach(GeneralSettings settings in allSettings)
            {
                values.Add(settings.Name, settings.Value);
            }
            return values;
        }
    }
}
