using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.DTOs;
using MinimalAPI.Demo.Mappings;
using MinimalAPI.Demo.Models;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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
	APIResponse response = new()
	{
		Data = CouponStore.Coupons,
		IsSuccess = true,
		StatusCode = HttpStatusCode.OK,
	};
	return Results.Ok(response);
})
.WithName("GetCoupons")
.Produces<APIResponse>(200);

app.MapGet("api/coupon/{id:int}", (int id) =>
{
	APIResponse response = new()
	{
		Data = CouponStore.Coupons.FirstOrDefault(c => c.Id == id),
		IsSuccess = true,
		StatusCode = HttpStatusCode.OK,
	};
	return Results.Ok(response);
})
.WithName("GetCouponById")
.Produces<APIResponse>(200);

app.MapPost("api/coupon", async (IMapper _mapper, IValidator<CouponCreateDTO> createCouponValidator, [FromBody] CouponCreateDTO couponCreateDTO) => 
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
	coupon.Id = CouponStore.Coupons is null ? 1 : CouponStore.Coupons.Max(c => c.Id) + 1;
	CouponStore.Coupons.Add(coupon);

	var couponDTO = _mapper.Map<Coupon, CouponDTO>(coupon);
	response.Data = couponDTO;
	response.IsSuccess = true;
	response.StatusCode = HttpStatusCode.Created;
	return Results.CreatedAtRoute("GetCouponById", new { id = couponDTO.Id }, response);
})
.WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<APIResponse>(201)
.Produces<APIResponse>(400);

app.MapDelete("api/coupon/{id:int}", (int id) =>
{
	APIResponse response = new();
	if (id <= 0)
	{
		response.ErrorMessages = new() { "Invalid request" };
		response.IsSuccess = false;
		response.StatusCode = HttpStatusCode.BadRequest;
		return Results.BadRequest(response);
	}

	var coupon = CouponStore.Coupons.FirstOrDefault(c => c.Id == id);
	if(coupon is null)
	{
		response.ErrorMessages = new() { "Coupon not found" };
		response.IsSuccess = false;
		response.StatusCode = HttpStatusCode.NotFound;
		return Results.NotFound(response);
	}

	CouponStore.Coupons.Remove(coupon);
	response.IsSuccess = true;
	response.StatusCode = HttpStatusCode.OK;
	return Results.Ok(response);
})
.WithName("DeleteCouponById")
.Produces<APIResponse>(400)
.Produces<APIResponse>(200)
.Produces<APIResponse>(404);
#endregion

app.UseHttpsRedirection();

app.Run();