using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.DTOs;
using MinimalAPI.Demo.Mappings;
using MinimalAPI.Demo.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddAutoMapper(typeof(MappingConfiguration).Assembly);
//builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());
//builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
//builder.Services.AddAutoMapper(
//		Type.Assembly(),
//		typeof(MappingConfiguration).Assembly
//	);
//builder.Services.AddAutoMapper(new[] { MappingConfiguration });

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingConfiguration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	//app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.MapGet("/helloworld/{id:int}", (int id) =>
//{
//	if (id <= 0)
//	{
//		return Results.BadRequest("Incorrect Id");
//	}

//	return Results.Ok($"Hello World From GET Endpoint with id {id}");
//});
//app.MapPost("/helloworld", (string? value) =>
//{
//	if (string.IsNullOrWhiteSpace(value))
//	{
//		return Results.BadRequest("Value is null");
//	}

//	return Results.Ok($"Hello World From POST Endpoint with value {value}");
//});

#region Coupon Endpoints
app.MapGet("api/coupon", (ILogger<Program> _logger) =>
{
	_logger.Log(LogLevel.Information, "Getting All Coupons");
	return Results.Ok(CouponStore.Coupons);
})
.WithName("GetCoupons")
.Produces<IEnumerable<Coupon>>(200);

app.MapGet("api/coupon/{id:int}", (int id) =>
{
	return Results.Ok(CouponStore.Coupons.FirstOrDefault(c => c.Id == id));
})
.WithName("GetCouponById")
.Produces<Coupon>(200);

app.MapPost("api/coupon", (IMapper _mapper, [FromBody] CouponCreateDTO couponCreateDTO) =>
{
	if(string.IsNullOrWhiteSpace(couponCreateDTO.Name))
	{
		return Results.BadRequest("Invalid coupon request.");
	}

	if(CouponStore.Coupons.FirstOrDefault(c=> c.Name.ToLower() == couponCreateDTO.Name.ToLower()) is not null)
	{
		return Results.BadRequest("Coupon already exists.");
	}

	var coupon = _mapper.Map<CouponCreateDTO, Coupon>(couponCreateDTO);
	coupon.CreatedDate = DateTime.Now;
	coupon.Id = CouponStore.Coupons is null ? 1 : CouponStore.Coupons.Max(c => c.Id) + 1;
	CouponStore.Coupons.Add(coupon);

	var couponDTO = _mapper.Map<Coupon, CouponDTO>(coupon);
	return Results.CreatedAtRoute("GetCouponById", new { id = couponDTO.Id }, couponDTO);
})
.WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<CouponDTO>(201)
.Produces(400);

#endregion

app.UseHttpsRedirection();

app.Run();