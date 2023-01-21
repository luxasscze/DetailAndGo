using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface IDAGService
    {
        public Task<List<Service>> GetAllServices();
        public Task<List<Service>> GetAllSubServices();
        public Task<Service> GetServiceById(int serviceId);
        public Task<List<Service>> GetAllMainServices();
    }
}
