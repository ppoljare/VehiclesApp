using AutoMapper;
using VehiclesApp.Model.Entities;
using VehiclesApp.MVC.Models.InputModels;
using VehiclesApp.MVC.Models.ViewModels;

namespace VehiclesApp.MVC.MappingConfigurations
{
    public class VehicleModelProfile : Profile
    {
        public VehicleModelProfile()
        {
            CreateMap<VehicleModel, VehicleModelViewModel>();
            CreateMap<VehicleModelInputModel, VehicleModel>().ReverseMap();
        }
    }
}
