using MinimalAPI.Demo.Models;

namespace MinimalAPI.Demo.Data
{
	public static class CouponStore
	{
		public static List<Coupon> Coupons = new List<Coupon>()
		{
			new Coupon { Id= 1, Name = "10 OFF", Percent = 10, IsActive = true },
			new Coupon { Id= 2, Name = "20 OFF", Percent = 20, IsActive = true },
		};
	}
}