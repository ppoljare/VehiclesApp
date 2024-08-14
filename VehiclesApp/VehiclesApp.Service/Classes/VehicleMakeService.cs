using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VehiclesApp.Model.Entities;
using VehiclesApp.Service.Data;
using VehiclesApp.Service.Interfaces;

namespace VehiclesApp.Service.Classes
{
    public class VehicleMakeService : IVehicleMakeService
    {
        private readonly ApplicationDbContext _dbContext;

        public VehicleMakeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddAsync(VehicleMake vehicleMake)
        {
            vehicleMake.Id = Guid.NewGuid();

            await _dbContext.VehicleMakes.AddAsync(vehicleMake);
            var noOfRowsAdded = await _dbContext.SaveChangesAsync();

            return (noOfRowsAdded == 1);
        }

        public async Task<int> CountAsync(string searchString)
        {
            if (searchString.IsNullOrEmpty())
            {
                searchString = "";
            }

            return await _dbContext.VehicleMakes
                .Where(v => v.Name.Contains(searchString))
                .CountAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var vehicleMakeToDelete = await _dbContext.VehicleMakes.FindAsync(id);

            if (vehicleMakeToDelete is not null)
            {
                _dbContext.VehicleMakes.Remove(vehicleMakeToDelete);

                var saveResult = await _dbContext.SaveChangesAsync();
                return (saveResult == 1);
            }

            return false;
        }

        public async Task<List<VehicleMake>> FindAsync(
            string sortBy,
            string searchString,
            int pageNumber,
            int pageSize)
        {
            
            if (searchString.IsNullOrEmpty())
            {
                searchString = "";
            }

            var queryToExecute = _dbContext.VehicleMakes
                .Where(v => v.Name.Contains(searchString));

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
                default:
                    queryToExecute = queryToExecute.OrderBy(v => v.Name);
                    break;
            }

            // If missing pageNumber or pageSize, get ALL rows
            // This should only happen when retrieving data for the dropdown in ModelController
            if ((pageNumber != 0) && (pageSize != 0))
            {
                queryToExecute = queryToExecute
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }

            var vehicleMakes = await queryToExecute
                .AsNoTracking()
                .ToListAsync();

            return vehicleMakes;
        }

        public async Task<VehicleMake> GetAsync(Guid id)
        {
            var vehicleMake = await _dbContext.VehicleMakes
                .Include(m => m.VehicleModels)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            return vehicleMake;
        }

        public async Task<bool> UpdateAsync(VehicleMake vehicleMake)
        {
            var vehicleMakeToUpdate = await _dbContext.VehicleMakes
                .FindAsync(vehicleMake.Id);

            if (vehicleMakeToUpdate is not null)
            {
                vehicleMakeToUpdate.Name = vehicleMake.Name;
                vehicleMakeToUpdate.Abrv = vehicleMake.Abrv;

                var state = _dbContext.Entry(vehicleMakeToUpdate).State;
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
