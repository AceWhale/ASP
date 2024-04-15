using ASP.Data.DAL;
using ASP.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

		[HttpGet]
		public List<Location> DoGet(Guid? categoryId)
		{
			return _dataAccessor.ContentDao.GetLocations(categoryId);
		}

		[HttpPost]
		public String Post([FromBody] LocationPostModel model)
		{
			try
			{
				_dataAccessor.ContentDao.AddLocation(
					name: model.Name,
					description: model.Description,
					CategoryId: model.CategoryId,
					Stars: model.Stars);
				Response.StatusCode = StatusCodes.Status201Created;
				return "OK";
			}
			catch (Exception ex)
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return "Error";
			}
		}
	}

	public class LocationPostModel
	{
		public String Name { get; set; }
		public String Description { get; set; }
		public Guid CategoryId { get; set; }
		public int Stars {  get; set; }
	}
}
