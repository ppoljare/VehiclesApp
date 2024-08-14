using System.ComponentModel.DataAnnotations.Schema;

namespace VehiclesApp.Model.Entities
{
    public class VehicleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abrv { get; set; }

        [ForeignKey("VehicleMake")]
        public Guid MakeId { get; set; }
        public VehicleMake VehicleMake { get; set; }
    }
}
