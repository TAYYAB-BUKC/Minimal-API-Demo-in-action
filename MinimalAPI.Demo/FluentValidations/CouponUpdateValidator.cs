﻿using FluentValidation;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.DTOs;

namespace MinimalAPI.Demo.FluentValidations
{
	public class CouponUpdateValidator : AbstractValidator<CouponUpdateDTO>
	{
		public CouponUpdateValidator()
		{
			RuleFor(c => c.Id).NotEmpty().GreaterThan(0);
			RuleFor(c => c.Name).Must((coupon, name) => IsNameUnique(name, coupon)).NotEmpty();
			RuleFor(c => c.Percent).InclusiveBetween(1, 100);
		}

		private bool IsNameUnique(string name, CouponUpdateDTO coupon)
		{
			var existingCoupon = CouponStore.Coupons.FirstOrDefault(c => c.Name.ToLower() == name.ToLower() && c.Id != coupon.Id);
			return existingCoupon == null;
		}
	}
}