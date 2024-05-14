﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASP.Data.Entities;
using ASP.Data.DAL;
using ASP.Models;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;
using System.Net;
using ASP.Middleware;

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
			String? userRole = HttpContext.User.Claims
				.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
			bool isAdmin = "Admin".Equals(userRole);
			return _dataAccessor.ContentDao.GetCategories(includedDeleted: isAdmin);
		}

		[HttpPost]
		public String DoPost([FromForm] CategoryPostModel model)
		{
			/*	У проєкті є дві авторизації: через сесії та через токени.
				Первинна авторизація за сесією (в силу того, що з неї починали)
				Дані авторизації за токеном шукаємо за типом авторизації, яку ми
				встановили як назва класу (AuthTokenMiddleware)
			 */
			var identity = User.Identities
				.FirstOrDefault(i => i.AuthenticationType == nameof(AuthTokenMiddleware));
			/*Токени:
			 *Передаються за стандартною схемою - заголовком
			 * Authorization: Bearer 1233242
			 * де 1233242 - токен
			 */
			if(identity == null)
			{
				// якщо авторизація не пройдена, то повідомлення в Items
				Response.StatusCode = StatusCodes.Status401Unauthorized;
				return HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? "";
			}
			if (identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != "Admin")
			{
				Response.StatusCode = StatusCodes.Status403Forbidden;
				return "Access to API forbidden";
			}
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
						fileName = Guid.NewGuid() + ext;
						pathName = path + fileName;
					}
					while (System.IO.File.Exists(pathName));
					using var steam = System.IO.File.OpenWrite(pathName);
					model.Photo.CopyTo(steam);
				}
				_dataAccessor.ContentDao.AddCategory(model.Name, model.Description, fileName, model.Slug);
				Response.StatusCode = StatusCodes.Status201Created;
				return "OK";
			}
			catch (Exception ex)
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return "ERROR";
			}

		}

		[HttpDelete("{id}")]
		public String DoDelete(Guid id)
		{
			_dataAccessor.ContentDao.DeleteCategory(id);
			Response.StatusCode = StatusCodes.Status202Accepted;
			return "OK";
		}

		// метод, НЕ позначений атрибутом, буде викликано, якщо не знайдеться
		// необхідний з позначених.Це дозволяє прийняти нестандартні запити
		public String DoOther()
		{
			// дані про метод запиту - у Request. Method
			if (Request.Method == "RESTORE")
			{
				return DoRestore();
			}
			Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
			return "Method not Allowed";
		}
		// Другий НЕ позначений метод має бути private щоб не було конфлікту
		private String DoRestore()
		{
			// Через відсутність атрибутів, автоматичного зв'язування параметрі
			// немає, параметри дістаємо з колекцій Request
			String? id = Request.Query["id"].FirstOrDefault();
			try
			{
				_dataAccessor.ContentDao.RestoreCategory(Guid.Parse(id!));
			}
			catch
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return "Empty or invalid id";
			}
			Response.StatusCode = StatusCodes.Status202Accepted;
			return "RESTORE works with id: " + id;
		}
	}
	public class CategoryPostModel
	{
		[FromForm(Name = "category-name")]
		public String Name { get; set; }


		[FromForm(Name = "category-description")]
		public String Description { get; set; }


		[FromForm(Name = "category-slug")]
		public String Slug { get; set; }


		[FromForm(Name = "category-photo")]
		public IFormFile? Photo { get; set; }
	}
}