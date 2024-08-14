using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VehiclesApp.Model.Entities;
using VehiclesApp.Service.Data;
using VehiclesApp.Service.Interfaces;

namespace VehiclesApp.Service.Classes
{
    public class VehicleModelService : IVehicleModelService
    {
        private readonly ApplicationDbContext _dbContext;

        public VehicleModelService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddAsync(VehicleModel vehicleModel)
        {
            vehicleModel.Id = Guid.NewGuid();

            await _dbContext.VehicleModels.AddAsync(vehicleModel);
            var noOfRowsAdded = await _dbContext.SaveChangesAsync();

            return (noOfRowsAdded == 1);
        }

        public async Task<int> CountAsync(string searchStringModel, string searchStringMake)
        {
            if (searchStringModel.IsNullOrEmpty())
            {
                searchStringModel = "";
            }

            if (searchStringMake.IsNullOrEmpty())
            {
                searchStringMake = "";
            }

            return await _dbContext.VehicleModels
                .Where(v => v.Name.Contains(searchStringModel)
                    && v.VehicleMake.Name.Contains(searchStringMake))
                .CountAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var vehicleModelToDelete = await _dbContext.VehicleModels.FindAsync(id);

            if (vehicleModelToDelete is not null)
            {
                _dbContext.VehicleModels.Remove(vehicleModelToDelete);

                var saveResult = await _dbContext.SaveChangesAsync();
                return (saveResult == 1);
            }

            return false;
        }

        public async Task<List<VehicleModel>> FindAsync(
            string sortBy,
            string searchStringModel,
            string searchStringMake,
            int pageNumber,
            int pageSize)
        {
            // FILTERTING
            if (searchStringModel.IsNullOrEmpty())
            {
                searchStringModel = "";
            }

            if (searchStringMake.IsNullOrEmpty())
            {
                searchStringMake = "";
            }

            var queryToExecute = _dbContext.VehicleModels
                .Include(m => m.VehicleMake)
                .Where(v => v.Name.Contains(searchStringModel)
                    && v.VehicleMake.Name.Contains(searchStringMake));

            // SORTING
            if (sortBy.IsNullOrEmpty())
            {
                sortBy = "Name_asc";
            }

            switch (sortBy)
            {
                case "Name_desc":
                    queryToExecute = queryToExecute.OrderByDescending(v => v.Name);
                    break;
                case "Abrv_asc":
                    queryToExecute = queryToExecute.OrderBy(v => v.Abrv);
                    break;
                case "Abrv_desc":
                    queryToExecute = queryToExecute.OrderByDescending(v => v.Abrv);
                    break;
                case "Make_asc":
                    queryToExecute = queryToExecute.OrderBy(v => v.VehicleMake.Name);
                    break;
                case "Make_desc":
                    queryToExecute = queryToExecute.OrderByDescending(v => v.VehicleMake.Name);
                    break;
                default:
                    queryToExecute = queryToExecute.OrderBy(v => v.Name);
                    break;
            }

            // PAGINATION
            queryToExecute = queryToExecute
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // FINISH QUERY
            var vehicleModels = await queryToExecute
                .AsNoTracking()
                .ToListAsync();
            
            /*
            var vehicleModels = await _dbContext.VehicleModels
                .Include(m => m.VehicleMake)
                .ToListAsync();
            */
            return vehicleModels;
        }

        public async Task<VehicleModel> GetAsync(Guid id)
        {
            var vehicleModel = await _dbContext.VehicleModels
                .Include(m => m.VehicleMake)
                .FirstOrDefaultAsync(m => m.Id == id);

            return vehicleModel;
        }

        public async Task<bool> UpdateAsync(VehicleModel vehicleModel)
        {
            var vehicleModelToUpdate = await _dbContext.VehicleModels
                .FindAsync(vehicleModel.Id);

            if (vehicleModelToUpdate is not null)
            {
                vehicleModelToUpdate.MakeId = vehicleModel.MakeId;
                vehicleModelToUpdate.Name = vehicleModel.Name;
                vehicleModelToUpdate.Abrv = vehicleModel.Abrv;

                var state = _dbContext.Entry(vehicleModelToUpdate).State;
                if (state == EntityState.Modified)
                {
                    var saveResult = await _dbContext.SaveChangesAsync();
                    return (saveResult == 1);
                }

                return true;
            }

            return false;
        }
    }
}
