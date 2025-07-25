using MinimalAPI.Demo.DTOs;

namespace MinimalAPI.Demo.Repository.IRepository
{
	public interface IAuthRepository
	{
		Task<bool> IsUserUnique(string username);
		Task<LoginResponseDTO> Authenticate(LoginRequestDTO request);
		Task<UserDTO> Register(RegistrationRequestDTO request);
	}
}