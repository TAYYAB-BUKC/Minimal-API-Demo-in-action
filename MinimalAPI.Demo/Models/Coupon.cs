namespace MinimalAPI.Demo.Models
{
	public class Coupon
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Percent { get; set; }
		public bool IsActive { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? LastUpdatedDate { get; set; }
	}
}