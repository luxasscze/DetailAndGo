using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface IDAGService
    {
        public Task<List<Service>> GetAllServices();        
    }
}
