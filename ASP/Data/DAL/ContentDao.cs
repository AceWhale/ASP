using ASP.Data.Entities;

namespace ASP.Data.DAL
{
	public class ContentDao
	{
		private readonly Object _dblocker;
		private readonly DataContext _context;
		public ContentDao(DataContext context, object dblocker)
		{
			_context = context;
			_dblocker = dblocker;
		}
		public void AddCategory(String name, String description, String? photoUrl)
		{
			_context.Categories.Add(new()
			{
				Id = Guid.NewGuid(),
				Name = name,
				Description = description,
				DeleteDt = null,
				PhotoUrl = photoUrl
			});
			_context.SaveChanges();
		}
		public List<Category> GetCategories()
		{
			List<Category> list;
			lock (_dblocker)
			{
				list = _context.Categories
				.Where(c => c.DeleteDt == null)
				.ToList();
			}
			return list;
		}
		public void UpdateCategory(Category category)
		{
			var ctg = _context
				.Categories
				.Find(category.Id);
			if (ctg != null)
			{
				ctg.Name = category.Name;
				ctg.Description = category.Description;
				ctg.DeleteDt = category.DeleteDt;
				_context.SaveChanges();
			}
		}
		public void DeleteCategory(Guid id)
		{
			var ctg = _context
				.Categories
				.Find(id);
			if (ctg != null)
			{
				ctg.DeleteDt = DateTime.Now;
				_context.SaveChanges();
			}
		}
		public void DeleteCategory(Category category)
		{
			DeleteCategory(category.Id);
		}
		public void AddLocation(String name, String description, Guid CategoryId,
			int? Stars = null, Guid? CountryId = null,
			Guid? CityId = null, string? Address = null, String? PhotoUrl = null)
		{
			lock (_dblocker)
			{
				_context.Locations.Add(new()
				{
					Id = Guid.NewGuid(),
					Name = name,
					Description = description,
					CategoryId = CategoryId,
					Stars = Stars,
					CountryId = CountryId,
					CityId = CityId,
					Address = Address,
					DeleteDt = null,
					PhotoUrl = PhotoUrl
				});
				_context.SaveChanges();
			}
		}

		public List<Location> GetLocations(Guid? categoryId = null)
		{
			var query = _context
				.Locations
				.Where(loc => loc.DeleteDt == null);
			if (categoryId != null)
			{
				query = query.Where(loc => loc.CategoryId == categoryId);
			}
			return query.ToList();
		}
	}
}
