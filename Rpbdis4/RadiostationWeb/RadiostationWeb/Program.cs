using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using Microsoft.AspNetCore.Identity;
using RadiostationWeb.Models;
using RadiostationWeb.Services;
using MarriageAgency.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Получаем строку подключения из конфигурации
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Регистрируем DbContext с использованием строки подключения
builder.Services.AddDbContext<RadioStationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Добавляем сервисы Identity для управления пользователями, если это нужно
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<RadioStationDbContext>();

// Регистрируем необходимые сервисы для MVC, кэширования и сессий
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddTransient<IBroadcastSheduleService, BroadcastSheduleService>();

var app = builder.Build();

// Проверка окружения разработки для включения страниц с ошибками
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Используем инициализацию базы данных
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

// Настройка обработки запросов
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

// Устанавливаем маршрутизацию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
