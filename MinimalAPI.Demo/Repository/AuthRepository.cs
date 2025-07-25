using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalAPI.Demo.Data;
using MinimalAPI.Demo.DTOs;
using MinimalAPI.Demo.Models;
using MinimalAPI.Demo.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalAPI.Demo.Repository
{
	public class AuthRepository : IAuthRepository
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IMapper _mapper;
		private string secretKey;
		public AuthRepository(ApplicationDbContext dbContext, IMapper mapper, IConfiguration _configuration)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			secretKey = _configuration.GetValue<string>("JWTSecretKey");
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

		public async Task<LoginResponseDTO> Authenticate(LoginRequestDTO request)
		{
			var user = await _dbContext.LocalUsers.FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower() && u.Password.ToLower() == request.Password.ToLower());
			if (user is null)
			{
				return new LoginResponseDTO();
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new(ClaimTypes.Name, user.Name),
					new(ClaimTypes.Email, user.Username),
					new(ClaimTypes.Role, user.Role),
				}),
				Expires = DateTime.Now.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return new()
			{
				User = _mapper.Map<UserDTO>(user),
				Token = tokenHandler.WriteToken(token)
			};
		}
	}
}
