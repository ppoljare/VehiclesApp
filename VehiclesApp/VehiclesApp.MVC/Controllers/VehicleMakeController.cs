using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VehiclesApp.Model.Entities;
using VehiclesApp.MVC.Models.InputModels;
using VehiclesApp.MVC.Models.ViewModels;
using VehiclesApp.Service.Interfaces;

namespace VehiclesApp.MVC.Controllers
{
    public class VehicleMakeController : Controller
    {
        private readonly IVehicleMakeService _service;

        private readonly IMapper _mapper;

        public VehicleMakeController(IVehicleMakeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string sortBy,
            string searchString,
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

            ViewData["SortBy"] = sortBy;

            // I feel like sending "" will cause less problems than sending null
            if (searchString.IsNullOrEmpty())
                searchString = "";

            ViewData["SearchString"] = searchString;

            /*
             * Moguće poboljšanje: spremiti itemCount kao private property te ga računati
             *   samo ako se promijenio searchString
             * Drugim riječima, nema potrebe računati itemCount ako su se promijenili 
             *   sortBy, pageNumber ili pageSize jer svako dodatno uspostavljanje 
             *   komunikacije s bazom nepotrebno usporava rad aplikacije
             * Možda naknadno smislim kako ovo pametno riješiti
             */
            int itemCount = await _service.CountAsync(searchString);

            // Get rows that fit all criteria
            var serviceResult = await _service.FindAsync(
                sortBy: sortBy,
                searchString: searchString,
                pageNumber: pageNumberDefault,
                pageSize: pageSizeDefault);

            var vehicleMakesView = _mapper
                .Map<List<VehicleMakeViewModel>>(serviceResult);

            PaginatedList<VehicleMakeViewModel> paginatedView = new
                PaginatedList<VehicleMakeViewModel>(
                    vehicleMakesView,
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

            var vehicleMakeView = _mapper
                .Map<VehicleMakeViewModel>(serviceResult);

            return View(vehicleMakeView);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(VehicleMakeInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var vehicleMake = _mapper.Map<VehicleMake>(inputModel);

            var serviceResult = await _service.AddAsync(vehicleMake);
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
                .Map<VehicleMakeInputModel>(serviceResult);

            return View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VehicleMakeInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var vehicleMake = _mapper.Map<VehicleMake>(inputModel);

            var serviceResult = await _service.UpdateAsync(vehicleMake);
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

            var vehicleMakeView = _mapper
                .Map<VehicleMakeViewModel>(serviceResult);

            return View(vehicleMakeView);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(Guid id)
        {
            var serviceResult = await _service.DeleteAsync(id);

            if (!serviceResult)
            {
                return BadRequest(new { error = "Could not delete item." });
            }

            return RedirectToAction("Index");
        }
    }
}
