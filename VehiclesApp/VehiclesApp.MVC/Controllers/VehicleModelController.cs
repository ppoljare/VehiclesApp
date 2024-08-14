using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VehiclesApp.Model.Entities;
using VehiclesApp.MVC.Models.InputModels;
using VehiclesApp.MVC.Models.ViewModels;
using VehiclesApp.Service.Interfaces;

namespace VehiclesApp.MVC.Controllers
{
    public class VehicleModelController : Controller
    {
        private readonly IVehicleModelService _service;
        private readonly IVehicleMakeService _makeService;
        private readonly IMapper _mapper;

        public VehicleModelController(
            IVehicleModelService service, 
            IVehicleMakeService makeService, 
            IMapper mapper)
        {
            _service = service;
            _makeService = makeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string sortBy,
            string searchStringModel,
            string searchStringMake,
            int? pageNumber,
            int? pageSize)
        {
            // If page number or page size are not provided via route, set default values
            int pageNumberDefault = pageNumber ?? 1;
            int pageSizeDefault = pageSize ?? 5;

            // If already sorted by name (ascending), next click will sort descending
            // Otherwise, next click will sort ascending
            if (sortBy.IsNullOrEmpty())
                ViewData["NameSortParam"] = "Name_desc";
            else
                ViewData["NameSortParam"] = "";

            // Same as above
            if (sortBy == "Abrv_asc")
                ViewData["AbrvSortParam"] = "Abrv_desc";
            else
                ViewData["AbrvSortParam"] = "Abrv_asc";

            // Same as above
            if (sortBy == "Make_asc")
                ViewData["MakeSortParam"] = "Make_desc";
            else
                ViewData["MakeSortParam"] = "Make_asc";

            ViewData["SortBy"] = sortBy;

            // I feel like sending "" will cause less problems than sending null
            if (searchStringModel.IsNullOrEmpty())
                searchStringModel = "";

            ViewData["SearchStringModel"] = searchStringModel;

            if (searchStringMake.IsNullOrEmpty())
                searchStringMake = "";

            ViewData["SearchStringMake"] = searchStringMake;

            int itemCount = await _service.CountAsync(searchStringModel, searchStringMake);

            var vehicleMakesView = await GetVehicleMakes();

            if (vehicleMakesView != null)
            {
                ViewBag.data = vehicleMakesView;
            }

            // Get rows that fit all criteria
            var serviceResult = await _service.FindAsync(
                sortBy: sortBy,
                searchStringModel: searchStringModel,
                searchStringMake: searchStringMake,
                pageNumber: pageNumberDefault,
                pageSize: pageSizeDefault);

            var vehicleModelsView = _mapper
                .Map<List<VehicleModelViewModel>>(serviceResult);

            PaginatedList<VehicleModelViewModel> paginatedView = new
                PaginatedList<VehicleModelViewModel>(
                    vehicleModelsView,
                    itemCount,
                    pageNumberDefault,
                    pageSizeDefault);

            return View(paginatedView);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var serviceResult = await _service.GetAsync(id);

            if (serviceResult == null)
            {
                return NotFound();
            }

            var vehicleModelView = _mapper
                .Map<VehicleModelViewModel>(serviceResult);

            return View(vehicleModelView);
        }

        // Get all vehicle makes for dropdown selection
        private async Task<List<VehicleMakeViewModel>> GetVehicleMakes()
        {
            var serviceResult = await _makeService.FindAsync(
                sortBy: "",
                searchString: "",
                pageNumber: 0,
                pageSize: 0);

            var vehicleMakesView = _mapper
                .Map<List<VehicleMakeViewModel>>(serviceResult);

            return vehicleMakesView;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var vehicleMakesView = await GetVehicleMakes();

            if (vehicleMakesView != null)
            {
                ViewBag.data = vehicleMakesView;
                return View();
            }

            return BadRequest(new { error = "Could not retrieve the list of vehicle makes." });
        }

        [HttpPost]
        public async Task<IActionResult> Add(VehicleModelInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var vehicleModel = _mapper.Map<VehicleModel>(inputModel);

            var serviceResult = await _service.AddAsync(vehicleModel);
            if (!serviceResult)
            {
                return BadRequest(new { error = "Could not add item." });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var serviceResult = await _service.GetAsync(id);

            if (serviceResult == null)
            {
                return NotFound();
            }

            var inputModel = _mapper
                .Map<VehicleModelInputModel>(serviceResult);

            var vehicleMakesView = await GetVehicleMakes();

            if (vehicleMakesView != null)
            {
                ViewBag.data = vehicleMakesView;
                return View(inputModel);
            }

            return BadRequest(new { error = "Could not retrieve the list of vehicle makes." });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VehicleModelInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var vehicleModel = _mapper.Map<VehicleModel>(inputModel);

            var serviceResult = await _service.UpdateAsync(vehicleModel);
            if (!serviceResult)
            {
                return BadRequest(new { error = "Could not update item." });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var serviceResult = await _service.GetAsync(id);

            if (serviceResult == null)
            {
                return NotFound();
            }

            var VehicleModelView = _mapper
                .Map<VehicleModelViewModel>(serviceResult);

            return View(VehicleModelView);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(Guid id)
        {
            var serviceResult = await _service.DeleteAsync(id);

            if (!serviceResult)
            {
                return BadRequest(new { error = "Could not update item." });
            }

            return RedirectToAction("Index");
        }
    }
}
