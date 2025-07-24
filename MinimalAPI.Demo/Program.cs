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

app.MapGet("/helloworld", () => "Hello World From Get Endpoint");
app.MapPost("/helloworld", () => "Hello World From Post Endpoint");

app.UseHttpsRedirection();

app.Run();