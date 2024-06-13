using ASP.Data.DAL;
using ASP.Data.Entities;
using ASP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace ASP.Controllers
{
	[Route("api/location")]
	[ApiController]
	public class LocationController : ControllerBase
	{
		private readonly DataAccessor _dataAccessor;
		public LocationController(DataAccessor dataAccessor)
		{
			_dataAccessor = dataAccessor;
		}

		[HttpGet("{id}")]
		public List<Location> DoGet(String id)
		{
			return _dataAccessor.ContentDao.GetLocations(id);
		}

		[HttpPost]
		public String Post(LocationFormModel model)
		{
			try
			{
				String? fileName = null;
				if (model.Photo != null)
				{
					string ext = Path.GetExtension(model.Photo.FileName);
					String path = Directory.GetCurrentDirectory() + "/wwwroot/img/content/";
					String pathName;
					do
					{
						fileName = RandomStringService.GenerateFilename(10) + ext;
						pathName = path + fileName;
					}
					while (System.IO.File.Exists(pathName));

					using var steam = System.IO.File.OpenWrite(pathName);
					model.Photo.CopyTo(steam);
				}
				_dataAccessor.ContentDao.AddLocation(
					name: model.Name,
					description: model.Description,
					CategoryId: model.CategoryId,
					Stars: model.Stars,
					PhotoUrl: fileName,
					slug: model.Slug);
				Response.StatusCode = StatusCodes.Status201Created;
				return "OK";
			}
			catch (Exception ex)
			{
                Response.StatusCode = StatusCodes.Status400BadRequest;
				return ex.Message;
            }
		}

		[HttpPatch]
		public Location? DoPatch(String slug)
		{
			return _dataAccessor.ContentDao.GetLocationBySlug(slug);
		}
	}

	public class LocationFormModel
	{
        [FromForm(Name = "category-id")]
        public Guid CategoryId { get; set; }

        [FromForm(Name = "loc-name")]
        public String Name { get; set; } = null!;

        [FromForm(Name = "loc-description")]
        public String Description { get; set; } = null!;

        [FromForm(Name = "loc-slug")]
        public String Slug { get; set; } = null!;

        [FromForm(Name = "loc-stars")]
        public int Stars { get; set; }

        [FromForm(Name = "loc-photo")]
        public IFormFile Photo { get; set; } = null!;
    }
}
