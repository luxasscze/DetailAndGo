using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface IDAGService
    {
        public Task<List<Service>> GetAllServices();
        public Task<List<Service>> GetAllServicesOrderedByCreated();
        public Task<List<Service>> GetAllSubServices();
        public Task<Service> GetServiceById(int serviceId);
        public Task<List<Service>> GetAllMainServices();
        public Task<List<Service>> GetServicesFromIdArray(string Ids);
    }
}
