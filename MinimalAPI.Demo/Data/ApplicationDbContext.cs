using Microsoft.EntityFrameworkCore;
using MinimalAPI.Demo.Models;

namespace MinimalAPI.Demo.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			
		}

		public DbSet<Coupon> Coupons { get; set; }
	}
}