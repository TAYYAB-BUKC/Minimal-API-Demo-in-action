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

		public DbSet<LocalUser> LocalUsers { get; set; }
		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Coupon>().HasData(CouponStore.Coupons);
			base.OnModelCreating(modelBuilder);
		}
	}
}