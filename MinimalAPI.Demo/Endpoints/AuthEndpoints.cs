using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Demo.DTOs;
using MinimalAPI.Demo.Models;
using MinimalAPI.Demo.Repository.IRepository;
using System.Net;

namespace MinimalAPI.Demo.Endpoints
{
	public static class AuthEndpoints
	{
		public static void ConfigureAuthEndpoints(this WebApplication app)
		{
			app.MapPost("api/register", RegisterAsync)
			   .WithName("Register")
			   .Produces<APIResponse>(200)
			   .Produces<APIResponse>(400);

			app.MapPost("api/login", LoginAsync)
			   .WithName("Login")
			   .Produces<APIResponse>(200)
			   .Produces<APIResponse>(400);
		}

		private static async Task<IResult> RegisterAsync(IAuthRepository _authRepository, [FromBody] RegistrationRequestDTO request)
		{
			APIResponse response = new();

			var isUserUnique = await _authRepository.IsUserUnique(request.Username);
			if (!isUserUnique)
			{
				response = new()
				{
					ErrorMessages = new List<string> { "Username already exists!!!" },
					IsSuccess = false,
					StatusCode = HttpStatusCode.BadRequest,
				};
				return Results.BadRequest(response);
			}

			var authResponse = await _authRepository.Register(request);
			
			response = new()
			{
				Data = authResponse,
				IsSuccess = true,
				StatusCode = HttpStatusCode.OK,
			};
			return Results.Ok(response);
		}

		private static async Task<IResult> LoginAsync(IAuthRepository _authRepository, [FromBody] LoginRequestDTO request)
		{
			APIResponse response = new();
			var authResponse = await _authRepository.Authenticate(request);
			if (authResponse is null)
			{
				response = new()
				{
					ErrorMessages = new List<string> { "Invalid username or password!!!" },
					IsSuccess = false,
					StatusCode = HttpStatusCode.BadRequest,
				};
				return Results.BadRequest(response);
			}

			response = new()
			{
				Data = authResponse,
				IsSuccess = true,
				StatusCode = HttpStatusCode.OK,
			};
			return Results.Ok(response);
		}
	}
}
