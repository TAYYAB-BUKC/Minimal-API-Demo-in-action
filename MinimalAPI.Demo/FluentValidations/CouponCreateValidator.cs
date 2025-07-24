using FluentValidation;
using MinimalAPI.Demo.DTOs;

namespace MinimalAPI.Demo.FluentValidations
{
	public class CouponCreateValidator : AbstractValidator<CouponCreateDTO>
	{
		public CouponCreateValidator()
		{
			RuleFor(c => c.Name).NotEmpty();
			RuleFor(c => c.Percent).InclusiveBetween(1, 100);
		}
	}
}