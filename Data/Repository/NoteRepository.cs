using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using Dapper.FastCrud;
using SimpleNote.Data.Filters;
using SimpleNote.Shared.Models;
using Dapper;

namespace SimpleNote.Data.Repository
{
	public class NoteRepository : RepositoryBase
	{
		public NoteRepository(IConfiguration configuration) : base(configuration)
		{
		}
		public async Task<bool> CreateAsync(Note model)
		{
			try
			{
				using (var cnn = this.GetConnection())
				{
					await cnn.InsertAsync<Note>(model);
					return true;
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<List<Note>> FindAllAsync(NoteFilter _filter)
		{
			var data = new List<Note>();
			var conditions = new System.Text.StringBuilder();

			conditions.AppendLine("1 = 1");
			if (!string.IsNullOrEmpty(_filter.Keywords))
			{
				_filter.Keywords = $"%{_filter.Keywords.ToLower().Replace(" ", "%")}%";
				conditions.AppendLine(" AND (n.Title LIKE @Keywords");
				conditions.AppendLine(" OR n.Text LIKE @Keywords)");
			}

			conditions.AppendLine(" AND n.UserId = @UserId");
			conditions.AppendLine(" AND n.Deleted = 0");

			try
			{
				using (var cnn = this.GetConnection())
				{
					var result = await cnn.FindAsync<Note>(x => x
						.Where($"{conditions}")
						.WithAlias("n")
					.OrderBy($"n.CreatedAt")
					.WithParameters(_filter));
					data = result.AsList();
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);

			}
			return data;
		}

		public async Task<Note> FindAsync(string id)
		{
			Note? note = null;
			try
			{
				using (var cnn = GetConnection())
				{
					note = await cnn.GetAsync<Note>(new Note() { Id = id });
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return note ?? new Note();
		}
		public async Task<bool> UpdateAsync(Note model)
		{
			try
			{
				using (var cnn = this.GetConnection())
				{
					await cnn.UpdateAsync<Note>(model);
					return true;
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}
	}
}