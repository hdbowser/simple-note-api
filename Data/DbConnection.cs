
using MySql.Data.MySqlClient;

namespace SimpleNote.Data
{
	public abstract class DbConnection
	{
		protected readonly IConfiguration _configuration;
		public DbConnection(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public MySqlConnection GetConnection()
		{
			return new MySqlConnection(_configuration.GetConnectionString("default").ToString());
		}
	}
}