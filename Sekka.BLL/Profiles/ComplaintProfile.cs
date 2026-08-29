using AutoMapper;
using Sekka.BLL.ViewModels;
using Sekka.DAL.Models;

namespace Sekka.BLL.Mapping
{
	public class ComplaintProfile : Profile
	{
		public ComplaintProfile()
		{
			CreateMap<Complaint, ComplaintVM>()
				.ForMember(dest => dest.CategoryDisplay, opt => opt.MapFrom(src => src.Category.ToString()))
				.ForMember(dest => dest.PriorityDisplay, opt => opt.MapFrom(src => src.Priority.ToString()))
				.ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.ToString()));

			CreateMap<ComplaintVM, Complaint>()
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.ResolvedAt, opt => opt.Ignore());
		}
	}
}