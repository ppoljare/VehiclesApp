using AutoMapper;
using VehiclesApp.Model.Entities;
using VehiclesApp.MVC.Models.InputModels;
using VehiclesApp.MVC.Models.ViewModels;

namespace VehiclesApp.MVC.MappingConfigurations
{
    public class VehicleMakeProfile : Profile
    {
        public VehicleMakeProfile()
        {
            CreateMap<VehicleMake, VehicleMakeViewModel>();
            CreateMap<VehicleMakeInputModel, VehicleMake>().ReverseMap();
        }
    }
}
