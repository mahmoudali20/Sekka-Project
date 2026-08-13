using AutoMapper;
using Sekka.BLL.ViewModels.CarVM;
using Sekka.BLL.ViewModels.DriverVM;
using Sekka.DAL.Models;

namespace Sekka.BLL.Profiles
{
    public class DriverProfile : Profile
    {

        public DriverProfile()
        {

            #region GetAll
            CreateMap<Driver, DriverVM>()
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
               .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));
            #endregion

            #region GetByID
            CreateMap<Driver, DriverDetailsVM>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.Car.Id))
                .ForMember(dest => dest.CarModel, opt => opt.MapFrom(src => src.Car.Model))
                .ForMember(dest => dest.CarColor, opt => opt.MapFrom(src => src.Car.Color))
                .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.Car.PlateNumber))
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.Car.LicensePlate))
                .ForMember(dest => dest.CarImage, opt => opt.MapFrom(src => src.Car.Image));
            #endregion

            #region Create
            CreateMap<CreateCarVM, Car>();
            CreateMap<CreateDriverVM, Driver>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Car, opt => opt.Ignore())
                .ForMember(dest => dest.CarId, opt => opt.Ignore())
                .ForMember(dest => dest.IsAvailable, opt => opt.Ignore())
                .ForMember(dest => dest.RatingAverage, opt => opt.Ignore());
            #endregion

            #region Update

            CreateMap<DriverEditVM, Driver>().ReverseMap();
            CreateMap<DriverEditVM, ApplicationUser>().ReverseMap();
            #endregion

        }

    }
}
