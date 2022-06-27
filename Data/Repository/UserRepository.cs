using SimpleNote.Shared.Models;
using Dapper.FastCrud;
using SimpleNote.Data.Configurations;

namespace SimpleNote.Data.Repository
{
	public class UserRepository : RepositoryBase
	{
		public UserRepository(IConfiguration configuration) : base(configuration) { }

		public async Task<bool> CreateAsync(User model)
		{
			try
			{
				using (var cnn = this.GetConnection())
				{
					await cnn.InsertAsync<User>(model);
					return true;
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<User> FindAsync(string id)
		{
			try
			{
				using (var cnn = this.GetConnection())
				{
					var data = await cnn.GetAsync<User>(new User() { Id = id });
					return data;
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<User> FindByUserNameAsync(string userName)
		{
			try
			{
				using (var cnn = this.GetConnection())
				{
					var result = await cnn.FindAsync<User>(x => x.Where($"UserName = @UserName").WithParameters(new { UserName = userName }));
					return result.ToList().FirstOrDefault();
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}
	}
}