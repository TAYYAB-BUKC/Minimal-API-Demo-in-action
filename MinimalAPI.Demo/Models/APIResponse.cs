using System.Net;

namespace MinimalAPI.Demo.Models
{
	public class APIResponse
	{
		public bool IsSuccess { get; set; }
		public object Data { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public List<string> ErrorMessages { get; set; } = new();
	}
}