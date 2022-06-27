namespace SimpleNote.Shared.Models
{
	public class Note
	{
		public string? Id { get; set; }
		public string? Title { get; set; }
		public string? Text { get; set; }
		public string? UserId { get; set; }
		public DateTime? CreatedAt { get; set; }
		public bool Deleted { get; set; }

		public User? User { get; set; }
	}
}