namespace MinimalAPI.Demo.DTOs
{
	public class CouponDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Percent { get; set; }
		public bool IsActive { get; set; }
		public DateTime? CreatedDate { get; set; }
	}
}