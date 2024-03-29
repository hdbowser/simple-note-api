using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Namespace
{
	[Route("api/[controller]")]
	[ApiController]
	public class InfoController : ControllerBase
	{
		[HttpGet]
		public ActionResult Get()
		{
			return new JsonResult(new
			{
				Name = "SimpleNote.Api",
				Detail = "A simple WebAPI for testing",
				By = "hdbowser@gmail.com",
				Github = "https://github.com/hdbowser/simple-note-api",
				Date = DateTime.Now.ToLongDateString()
			});
		}
	}
}