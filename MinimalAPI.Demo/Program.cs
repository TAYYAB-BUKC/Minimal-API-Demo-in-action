using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.MapGet("api/coupon", () =>
{
	return Results.Ok(CouponStore.Coupons);
})
.WithName("GetCoupons")
.Produces(200);

app.MapGet("api/coupon/{id:int}", (int id) =>
{
	return Results.Ok(CouponStore.Coupons.FirstOrDefault(c => c.Id == id));
})
.WithName("GetCouponById")
.Produces(200);

app.MapPost("api/coupon", ([FromBody] Coupon coupon) =>
{
	if(coupon.Id > 0 || string.IsNullOrWhiteSpace(coupon.Name))
	{
		return Results.BadRequest("Invalid coupon request.");
	}

	if(CouponStore.Coupons.FirstOrDefault(c=> c.Name.ToLower() == coupon.Name.ToLower()) is not null)
	{
		return Results.BadRequest("Coupon already exists.");
	}

	coupon.Id = CouponStore.Coupons is null ? 1 : CouponStore.Coupons.Max(c => c.Id) + 1;
	CouponStore.Coupons.Add(coupon);
	return Results.CreatedAtRoute("GetCouponById", new { id = coupon.Id }, coupon);
})
.WithName("CreateCoupon")
.Produces<Coupon>(201)
.Produces(400);

#endregion

app.UseHttpsRedirection();

app.Run();