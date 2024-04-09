namespace ASP.Data.DAL
{
	public class DataAccessor
	{
		public readonly DataContext _dataContext;
		public UserDao UserDao { get; private set; }
		public DataAccessor(DataContext dataContext)
		{
			_dataContext = dataContext;
			UserDao = new UserDao(dataContext);
		}
	}
}
