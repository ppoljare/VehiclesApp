using VehiclesApp.Model.Entities;

namespace VehiclesApp.Service.Interfaces
{
    public interface IVehicleModelService
    {
        Task<bool> AddAsync(VehicleModel vehicleModel);
        Task<int> CountAsync(string searchStringModel, string searchStringMake);
        Task<bool> DeleteAsync(Guid id);
        Task<List<VehicleModel>> FindAsync(
            string sortBy,
            string searchStringModel,
            string searchStringMake,
            int pageNumber,
            int pageSize);
        Task<VehicleModel> GetAsync(Guid id);
        Task<bool> UpdateAsync(VehicleModel vehicleModel);
    }
}
