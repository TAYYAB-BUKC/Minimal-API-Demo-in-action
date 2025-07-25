using MinimalAPI.Demo.Models;

namespace MinimalAPI.Demo.Repository.IRepository
{
	public interface ICouponRepository
	{
		Task<IEnumerable<Coupon>> GetCouponsAsync();
		Task<Coupon> GetCouponByIdAsync(int id);
		Task<Coupon> GetCouponByNameAsync(string name);
		Task AddCouponAsync(Coupon coupon);
		Task UpdateCouponAsync(Coupon coupon);
		Task DeleteCouponByIdAsync(Coupon coupon);
		Task SaveChangesAsync();
	}
}