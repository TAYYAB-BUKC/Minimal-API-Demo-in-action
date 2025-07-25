﻿using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Demo.DTOs;
using MinimalAPI.Demo.Models;
using MinimalAPI.Demo.Repository;
using MinimalAPI.Demo.Repository.IRepository;
using System.Net;

namespace MinimalAPI.Demo.Endpoints
{
	public static class CouponEndpoints
	{
		public static void ConfigureCouponEndpoints(this WebApplication app)
		{
			app.MapGet("api/coupon", GetCouponsAsync)
			   .WithName("GetCoupons")
			   .Produces<APIResponse>(200);
			   //.RequireAuthorization("AdminOnly");

			app.MapGet("api/coupon/{id:int}", async (ICouponRepository _couponRepository, int id) =>
			{
				Console.WriteLine("Endpoint executed");
				APIResponse response = new()
				{
					Data = await _couponRepository.GetCouponByIdAsync(id),
					IsSuccess = true,
					StatusCode = HttpStatusCode.OK,
				};
				return Results.Ok(response);
			})
			   .WithName("GetCouponById")
			   .Produces<APIResponse>(200)
			   .RequireAuthorization()
			   .AddEndpointFilter(async (context, next) => {
				   var id = context.GetArgument<int>(1);
				   if(id <= 0)
				   {
					   APIResponse response = new()
					   {
						   ErrorMessages = new List<string> { "Invalid coupon id!!!" },
						   IsSuccess = false,
						   StatusCode = HttpStatusCode.BadRequest
					   };
					   return Results.BadRequest(response);
				   }

				   Console.WriteLine("Before 1st filter");
				   var result = await next(context);
				   Console.WriteLine("After 1st filter");
				   // After filter logic goes here
				   return result;
			   })
			   .AddEndpointFilter(async (context, next) => {
				   Console.WriteLine("Before 2nd filter");
				   var result = await next(context);
				   Console.WriteLine("After 2nd filter");
				   return result;
			   }); ;

			app.MapPost("api/coupon", async (ICouponRepository _couponRepository, IMapper _mapper, IValidator<CouponCreateDTO> createCouponValidator, [FromBody] CouponCreateDTO couponCreateDTO) =>
			{
				APIResponse response = new();
				var results = await createCouponValidator.ValidateAsync(couponCreateDTO);
				if (!results.IsValid)
				{
					response.ErrorMessages = results.Errors.Select(e => e.ErrorMessage).ToList();
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.BadRequest;
					return Results.BadRequest(response);
				}

				var coupon = _mapper.Map<CouponCreateDTO, Coupon>(couponCreateDTO);
				coupon.CreatedDate = DateTime.Now;

				await _couponRepository.AddCouponAsync(coupon);
				await _couponRepository.SaveChangesAsync();

				var couponDTO = _mapper.Map<Coupon, CouponDTO>(coupon);
				response.Data = couponDTO;
				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.Created;
				return Results.CreatedAtRoute("GetCouponById", new { id = couponDTO.Id }, response);
			})
			.WithName("CreateCoupon")
			.Accepts<CouponCreateDTO>("application/json")
			.Produces<APIResponse>(201)
			.Produces<APIResponse>(400)
			.RequireAuthorization();

			app.MapPut("api/coupon", async (ICouponRepository _couponRepository, IMapper _mapper, IValidator<CouponUpdateDTO> couponUpdateValidator, [FromBody] CouponUpdateDTO couponUpdateDTO) =>
			{
				APIResponse response = new();
				var coupon = await _couponRepository.GetCouponByIdAsync(couponUpdateDTO.Id);
				if (coupon is null)
				{
					response.ErrorMessages = new() { "Coupon not found" };
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.NotFound;
					return Results.NotFound(response);
				}

				var results = await couponUpdateValidator.ValidateAsync(couponUpdateDTO);
				if (!results.IsValid)
				{
					response.ErrorMessages = results.Errors.Select(e => e.ErrorMessage).ToList();
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.BadRequest;
					return Results.BadRequest(response);
				}

				//var createdDate = coupon.CreatedDate;
				_mapper.Map(couponUpdateDTO, coupon);
				//coupon.CreatedDate = createdDate;
				coupon.LastUpdatedDate = DateTime.Now;
				await _couponRepository.SaveChangesAsync();

				response.Data = _mapper.Map<CouponDTO>(coupon);
				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.OK;
				return Results.Ok(response);
			})
			.WithName("UpdateCoupon")
			.Accepts<CouponUpdateDTO>("application/json")
			.Produces<APIResponse>(200)
			.Produces<APIResponse>(400)
			.Produces<APIResponse>(404)
			.RequireAuthorization();

			app.MapDelete("api/coupon/{id:int}", async (ICouponRepository _couponRepository, int id) =>
			{
				APIResponse response = new();
				if (id <= 0)
				{
					response.ErrorMessages = new() { "Invalid request" };
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.BadRequest;
					return Results.BadRequest(response);
				}

				var coupon = await _couponRepository.GetCouponByIdAsync(id);
				if (coupon is null)
				{
					response.ErrorMessages = new() { "Coupon not found" };
					response.IsSuccess = false;
					response.StatusCode = HttpStatusCode.NotFound;
					return Results.NotFound(response);
				}

				await _couponRepository.DeleteCouponByIdAsync(coupon);
				await _couponRepository.SaveChangesAsync();

				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.OK;
				return Results.Ok(response);
			})
			.WithName("DeleteCouponById")
			.Produces<APIResponse>(400)
			.Produces<APIResponse>(200)
			.Produces<APIResponse>(404)
			.RequireAuthorization();

			app.MapGet("api/coupon/special", async ([AsParameters] CouponRequest request) =>
			{
				var coupons = await request._couponRepository.GetCouponsAsync();
				if (request.CouponName is not null)
				{
					coupons = coupons.Where(c => c.Name.Contains(request.CouponName)).ToList();
				}

				coupons = coupons.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

				APIResponse response = new()
				{
					Data = coupons,
					IsSuccess = true,
					StatusCode = HttpStatusCode.OK,
				};
				return Results.Ok(response);
			})
			.WithName("GetSpecialCoupon")
			.Produces<APIResponse>(200)
			.RequireAuthorization();
		}

		[Authorize(Policy = "AdminOnly")]
		private static async Task<IResult> GetCouponsAsync(ICouponRepository _couponRepository, ILogger<Program> _logger)
		{
			_logger.Log(LogLevel.Information, "Getting All Coupons");
			APIResponse response = new()
			{
				Data = await _couponRepository.GetCouponsAsync(),
				IsSuccess = true,
				StatusCode = HttpStatusCode.OK,
			};
			return Results.Ok(response);
		}
	}

	public class CouponRequest
	{
		public string CouponName { get; set; }
		public int PageSize { get; set; }
		public int PageNumber { get; set; }
		public ICouponRepository _couponRepository { get; set; }
	}
}