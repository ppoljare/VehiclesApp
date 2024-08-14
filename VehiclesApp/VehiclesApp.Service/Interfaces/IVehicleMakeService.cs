using VehiclesApp.Model.Entities;

namespace VehiclesApp.Service.Interfaces
{
    public interface IVehicleMakeService
    {
        Task<bool> AddAsync(VehicleMake vehicleMake);
        Task<int> CountAsync(string searchString);
        Task<bool> DeleteAsync(Guid id);
        Task<List<VehicleMake>> FindAsync(
            string sortBy,
            string searchString,
            int pageNumber,
            int pageSize);
        Task<VehicleMake> GetAsync(Guid id);
        Task<bool> UpdateAsync(VehicleMake vehicleMake);
    }
}
