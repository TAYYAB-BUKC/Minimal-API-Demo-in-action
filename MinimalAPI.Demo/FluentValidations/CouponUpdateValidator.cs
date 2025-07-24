using FluentValidation;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.DTOs;

namespace MinimalAPI.Demo.FluentValidations
{
	public class CouponUpdateValidator : AbstractValidator<CouponUpdateDTO>
	{
		public CouponUpdateValidator()
		{
			RuleFor(c => c.Name).Must(IsNameUnique).NotEmpty();
			RuleFor(c => c.Percent).InclusiveBetween(1, 100);
		}

		private bool IsNameUnique(string name)
		{
			var existingCoupons = CouponStore.Coupons.Where(c => c.Name.ToLower() == name.ToLower()).ToList();
			if (existingCoupons is not null && existingCoupons.Count > 1)
			{
				return false;
			}
			return true;
		}
	}
}