using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.Endpoints;
using MinimalAPI.Demo.Mappings;
using MinimalAPI.Demo.Repository;
using MinimalAPI.Demo.Repository.IRepository;
using System.Text;

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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddAuthentication(options => {
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateAudience = false,
		ValidateIssuer = false,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecretKey")))
	};
});

builder.Services.AddAuthorization();

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

app.UseAuthentication();

app.UseAuthorization();

app.ConfigureCouponEndpoints();

app.ConfigureAuthEndpoints();

app.UseHttpsRedirection();

app.Run();