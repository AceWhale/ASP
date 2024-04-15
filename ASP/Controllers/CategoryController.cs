using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASP.Data.Entities;
using ASP.Data.DAL;

namespace ASP.Controllers
{
	[Route("api/category")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly DataAccessor _dataAccessor;
		public CategoryController(DataAccessor dataAccessor)
		{
			_dataAccessor = dataAccessor;
		}

		[HttpGet]
		public List<Category> DoGet()
		{
			return _dataAccessor.ContentDao.GetCategories();
		}

		[HttpPost]
		public String DoPost([FromBody] CategoryPostModel model)
		{
			try
			{
				_dataAccessor.ContentDao.AddCategory(model.Name, model.Description);
				Response.StatusCode = StatusCodes.Status201Created;
				return "OK";
			}
			catch (Exception ex)
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return "ERROR";
			}

		}
	}
	public class CategoryPostModel
	{
		public String Name { get; set; }
		public String Description { get; set; }

	}
}
