namespace VehiclesApp.MVC.Models.ViewModels
{
    public class VehicleMakeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abrv { get; set; }

        public ICollection<VehicleModelViewModel> VehicleModels { get; set; }
    }
}
