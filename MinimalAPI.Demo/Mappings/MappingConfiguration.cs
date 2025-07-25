using AutoMapper;
using MinimalAPI.Demo.DTOs;
using MinimalAPI.Demo.Models;

namespace MinimalAPI.Demo.Mappings
{
	public class MappingConfiguration : Profile
	{
		public MappingConfiguration()
		{
			CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
			CreateMap<Coupon, CouponDTO>().ReverseMap();
			CreateMap<Coupon, CouponUpdateDTO>().ReverseMap();
			CreateMap<LocalUser, UserDTO>().ReverseMap();
		}
	}
}