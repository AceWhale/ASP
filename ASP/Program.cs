using ASP.Data;
using ASP.Services.Hash;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
/* ������ ����� ������ �� ���������� builder.Services
* �� ����� ������ � ��������� �������, ��� �� �������
* var app = builder. Build();
* ������, ������� � ����������� DIP ����������� ��
* ��'���� (binding) �� ����������� �� ������, �� ����
* ������. ���������� ����� �������� �� "�� �����
* �������� IHashService ��������� �� ��������� ��'���
* ����� Md5HashService"
*/
// builder.Services. AddSingleton<IHashService, Md5HashService>();
// ������� �� ������ ����������� ������ ������ - ���� ����� ���
builder.Services.AddSingleton<IHashService, ShaHashService>();


// ��������� ��������� ����� (MSSQL)
builder.Services.AddDbContext<DataContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("LocalMSSQL")));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(x => x
				.AllowAnyMethod()
				.AllowAnyHeader()
				.SetIsOriginAllowed(origin => true) // allow any origin
				.AllowCredentials());

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
