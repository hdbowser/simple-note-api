using SimpleNote.Data.Configurations;

namespace SimpleNote.Data.Repository
{
	public abstract class RepositoryBase : DbConnection
	{
		public RepositoryBase(IConfiguration configuration) : base(configuration) { }
	}
}