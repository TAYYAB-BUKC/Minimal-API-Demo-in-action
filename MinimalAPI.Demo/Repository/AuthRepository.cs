using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.DTOs;
using MinimalAPI.Demo.Models;
using MinimalAPI.Demo.Repository.IRepository;

namespace MinimalAPI.Demo.Repository
{
	public class AuthRepository : IAuthRepository
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IMapper _mapper;
		public AuthRepository(ApplicationDbContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		public async Task<bool> IsUserUnique(string username)
		{
			var user = await _dbContext.LocalUsers.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
			return user is null;
		}

		public async Task<UserDTO> Register(RegistrationRequestDTO request)
		{
			LocalUser user = new()
			{
				Name = request.Name,
				Username = request.Username,
				Password = request.Password,
				Role = "Admin"
			};

			await _dbContext.LocalUsers.AddAsync(user);
			await _dbContext.SaveChangesAsync();
			return _mapper.Map<UserDTO>(user);
		}

		public Task<LoginResponseDTO> Authenticate(LoginRequestDTO request)
		{
			throw new NotImplementedException();
		}
	}
}
