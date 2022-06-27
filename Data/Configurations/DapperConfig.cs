using System.Reflection;
using Dapper.FastCrud;
using SimpleNote.Shared.Models;

namespace SimpleNote.Data.Configurations
{
	public static class DapperCofig
	{
		public static void Configure()
		{
			OrmConfiguration.DefaultDialect = SqlDialect.MySql;

			OrmConfiguration.RegisterEntity<User>()
				.SetProperty(x => x.Id, prop => prop.SetPrimaryKey())
				.SetProperty(x => x.DisplayName)
				.SetProperty(x => x.PasswordHash)
				.SetProperty(x => x.PasswordSalt)
				.SetProperty(x => x.UserName);


			OrmConfiguration.RegisterEntity<Note>()
				.SetTableName("Notes")
				.SetProperty(x => x.Id, prop => prop.SetPrimaryKey())
				.SetProperty(x => x.Title)
				.SetProperty(x => x.Text)
				.SetProperty(x => x.UserId)
				.SetProperty(x => x.CreatedAt, prop => prop.IncludeInInserts(false))
				.SetProperty(x => x.Deleted, prop => prop.IncludeInInserts(false))
				.SetChildParentRelationship(x => x.User, x => x.UserId);
		}
	}
}