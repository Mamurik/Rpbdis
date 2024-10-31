using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using RadiostationWeb.Data;

namespace RadiostationWeb.Middleware
{
    public class DbInitializerMiddleware
    {
        private readonly RequestDelegate _next;

        public DbInitializerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider, RadioStationDbContext db)
        {
            // Проверяем, инициализирована ли база данных
            if (!context.Session.Keys.Contains("dbInitialized"))
            {
                // Инициализируем базу данных
                DbInitializer.Initialize(db);
                context.Session.SetString("dbInitialized", "Yes");
            }

            await _next.Invoke(context);
        }
    }

    public static class DbInitializerExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DbInitializerMiddleware>();
        }
    }
}