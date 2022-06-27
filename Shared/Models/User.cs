namespace SimpleNote.Shared.Models
{
	public class User
	{
		public string Id { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string DisplayName { get; set; } = string.Empty;
		public byte[] PasswordHash { get; set; } = new byte[1];
		public byte[] PasswordSalt { get; set; } = new byte[1];
	}
}