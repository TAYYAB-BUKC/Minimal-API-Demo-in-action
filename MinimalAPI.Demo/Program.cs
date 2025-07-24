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

app.MapGet("/helloworld", () => Results.Ok("Hello World From Get Endpoint"));
app.MapPost("/helloworld", (string? value) =>
{
	if (string.IsNullOrWhiteSpace(value))
	{
		return Results.BadRequest("Value is null");
	}

	return Results.Ok($"Hello World From POST Endpoint with value {value}");
});

app.UseHttpsRedirection();

app.Run();