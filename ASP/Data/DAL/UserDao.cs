using ASP.Data.Entities;

namespace ASP.Data.DAL
{
	public class UserDao
	{
		public readonly DataContext _dataContext;

		public UserDao(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
		public void Signup(User user)
		{
			if(user.Id == default)
			{
				user.Id = Guid.NewGuid();
			}
			_dataContext.Users.Add(user);
			_dataContext.SaveChanges();
		}
	}
}

/* DAL - Data Access Layer - сукупність ycіх DAO
 * DAO - Data Access Object - набір методів для роботи з сутністю
 */
