using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using Microsoft.AspNetCore.Identity;
using RadiostationWeb.Models;
using RadiostationWeb.Services;
using MarriageAgency.Middleware;

var builder = WebApplication.CreateBuilder(args);

// �������� ������ ����������� �� ������������
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ������������ DbContext � �������������� ������ �����������
builder.Services.AddDbContext<RadioStationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ��������� ������� Identity ��� ���������� ��������������, ���� ��� �����
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<RadioStationDbContext>();

// ������������ ����������� ������� ��� MVC, ����������� � ������
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddTransient<IBroadcastSheduleService, BroadcastSheduleService>();

var app = builder.Build();

// �������� ��������� ���������� ��� ��������� ������� � ��������
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ���������� ������������� ���� ������
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<RadioStationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.UseOperatinCache("BroadcastSchedules 10");

// ��������� ��������� ��������
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

// ������������� �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
