namespace VehiclesApp.MVC.Models.InputModels
{
    public class VehicleModelInputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abrv { get; set; }

        public Guid MakeId { get; set; }
    }
}
