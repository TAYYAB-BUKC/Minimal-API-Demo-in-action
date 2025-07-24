using FluentValidation;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.DTOs;

namespace MinimalAPI.Demo.FluentValidations
{
	public class CouponCreateValidator : AbstractValidator<CouponCreateDTO>
	{
		public CouponCreateValidator()
		{
			RuleFor(c => c.Name).Must(IsNameUnique).NotEmpty();
			RuleFor(c => c.Percent).InclusiveBetween(1, 100);
		}

		private bool IsNameUnique(string name)
		{
			var existingCoupon = CouponStore.Coupons.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
			if (existingCoupon is not null)
			{
				return false;
			}
			return true;
		}
	}
}