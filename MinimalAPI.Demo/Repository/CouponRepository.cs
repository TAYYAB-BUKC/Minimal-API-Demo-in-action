using Microsoft.EntityFrameworkCore;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.Models;
using MinimalAPI.Demo.Repository.IRepository;

namespace MinimalAPI.Demo.Repository
{
	public class CouponRepository : ICouponRepository
	{
		private readonly ApplicationDbContext _dbContext;
		public CouponRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<Coupon>> GetCouponsAsync()
		{
			return await _dbContext.Coupons.ToListAsync();
		}

		public async Task<Coupon> GetCouponByIdAsync(int id)
		{
			return await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<Coupon> GetCouponByNameAsync(string name)
		{
			return await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
		}

		public async Task AddCouponAsync(Coupon coupon)
		{
			await _dbContext.Coupons.AddAsync(coupon);
		}

		public async Task UpdateCouponAsync(Coupon coupon)
		{
			_dbContext.Coupons.Update(coupon);
		}

		public async Task DeleteCouponByIdAsync(Coupon coupon)
		{
			_dbContext.Coupons.Remove(coupon);
		}

		public async Task SaveChangesAsync()
		{
			await _dbContext.SaveChangesAsync();
		}
	}
}
