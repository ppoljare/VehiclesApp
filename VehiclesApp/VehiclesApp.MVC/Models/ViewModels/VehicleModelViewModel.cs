namespace VehiclesApp.MVC.Models.ViewModels
{
    public class VehicleModelViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abrv { get; set; }

        public VehicleMakeViewModel VehicleMake { get; set; }
    }
}
