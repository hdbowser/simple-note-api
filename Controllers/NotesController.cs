using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleNote.Data.Repository;
using SimpleNote.Shared.Models;
using SimpleNote.Data.Filters;

namespace SimpleNote.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class NotesController : ControllerBase
	{
		private IConfiguration _configuration;
		public NotesController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public async Task<ActionResult> PostAsync(Note model)
		{
			if (model == null)
			{
				return BadRequest();
			}

			var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			model.Id = System.Guid.NewGuid().ToString();
			model.UserId = id;
			var noteRepo = new NoteRepository(_configuration);

			if (await noteRepo.CreateAsync(model))
			{
				return Ok();
			}
			else
			{
				return Problem("Error al crear la nota");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Note>> GetAsync(string id)
		{
			var repo = new NoteRepository(_configuration);
			return await repo.FindAsync(id);
		}


		[HttpPut("{id}")]
		public async Task<ActionResult> PutAsync(string id, [FromBody] Note update)
		{
			if (update == null)
			{
				return BadRequest("");
			}

			var repo = new NoteRepository(_configuration);
			var toUpdate = await repo.FindAsync(id);

			if (string.IsNullOrEmpty(toUpdate.Id))
			{
				return NotFound();
			}

			toUpdate.Title = update.Title;
			toUpdate.Text = update.Text;

			if (await repo.UpdateAsync(toUpdate))
			{
				return Ok("Done");
			}
			else
			{
				return Problem("Error al actualizar la nota");
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteAsync(string id)
		{
			var repo = new NoteRepository(_configuration);
			var toUpdate = await repo.FindAsync(id);

			if (string.IsNullOrEmpty(toUpdate.Id))
			{
				return NotFound();
			}

			toUpdate.Deleted = true;

			if (await repo.UpdateAsync(toUpdate))
			{
				return Ok("Done");
			}
			else
			{
				return Problem("Error al eliminar la nota");
			}
		}

		[HttpGet]
		public async Task<ActionResult<string>> GetAllAsync([FromQuery] NoteFilter filter)
		{
			filter = filter ?? new NoteFilter();

			var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			filter.UserId = id;
			var data = await new NoteRepository(_configuration).FindAllAsync(filter);
			return new JsonResult(data);
		}
	}
}