using DataLayer.Data;
using DataLayer.models;
using Radiostation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Radiostation.Services.ArtistsService;
using Radiostation.Services.BroadcastSchedulesService;
using Radiostation.Services.BroadcastSchedulesService;
using Microsoft.AspNetCore.Builder;
using Radiostation.Services.EmployeesService;
using Radiostation.Services.GenresService;
using Radiostation.Services.RecordsService;
using Radiostation.Services.RecordDetailsService;
using Radiostation.Middleware;
namespace Radiostation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<RadioStationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddMemoryCache();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddScoped<ICachedArtistsService, CachedArtistsService>();
            services.AddScoped<ICachedBroadcastSchedulesService, CachedBroadcastSchedulesService>();
            services.AddScoped<ICacheEmployeesService, CacheEmployeesService>();
            services.AddScoped<ICacheGenresService, CacheGenresService>();
            services.AddScoped<ICacheRecordsService, CacheRecordsService>();
            services.AddScoped<ICacheRecordDetailsService, CacheRecordDetailsService>();

            //Использование MVC - отключено
            //services.AddControllersWithViews();
            var app = builder.Build();

            // добавляем поддержку статических файлов
            app.UseStaticFiles();

            // добавляем поддержку сессий
            app.UseSession();



            // Вывод информации о клиенте
            app.Map("/info", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Формирование строки для вывода 
                    string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Информация:</H1>";
                    strResponse += "<BR> Сервер: " + context.Request.Host;
                    strResponse += "<BR> Путь: " + context.Request.PathBase;
                    strResponse += "<BR> Протокол: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
                    // Вывод данных
                    await context.Response.WriteAsync(strResponse);
                });
            });

            app.Map("/artists", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    ICachedArtistsService cachedArtistsService = context.RequestServices.GetService<ICachedArtistsService>();
                    IEnumerable<Artist> artists = cachedArtistsService.GetArtists(20);
                    string HtmlString = "<HTML><HEAD><TITLE>Таблица Артистов</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Артисты</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Айди</TH>";
                    HtmlString += "<TH>Имя</TH>";
                    HtmlString += "<TH>Описание</TH>";
                    HtmlString += "<TH>Участники</TH>";
                    HtmlString += "</TR>";
                    foreach (var artist in artists)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + artist.ArtistId + "</TD>";
                        HtmlString += "<TD>" + artist.Name + "</TD>";
                        HtmlString += "<TD>" + artist.Description + "</TD>";
                        HtmlString += "<TD>" + artist.Members + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Назад на главную</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    await context.Response.WriteAsync(HtmlString);
                });
            });

            //2.2
            app.Map("/searchArtists", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Получение значения из cookie
                    string artistName;
                    context.Request.Cookies.TryGetValue("artistName", out artistName);
                    ICachedArtistsService cachedArtistsService = context.RequestServices.GetService<ICachedArtistsService>();
                    cachedArtistsService.AddArtists("ArtistsCache", 1000);  // Если необходима инициализация кеша
                    IEnumerable<Artist> artists = cachedArtistsService.GetArtists(20);

                    // Формирование HTML-формы для поиска артистов
                    string HtmlString = "<HTML><HEAD><TITLE>Поиск Артистов</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список артистов по имени</H1>" +
                    "<BODY><FORM action ='/searchArtists' method='GET'>" +
                    "Имя артиста:<BR><INPUT type='text' name='artistName' value='" + artistName + "'>" +
                    "<BR><BR><INPUT type='submit' value='Сохранить в cookies и вывести артистов с заданным именем'></FORM>" +
                    "<TABLE BORDER=1>";

                    HtmlString += "<TR>";
                    HtmlString += "<TH>Айди</TH>";
                    HtmlString += "<TH>Имя</TH>";
                    HtmlString += "<TH>Описание</TH>";
                    HtmlString += "<TH>Участники</TH>";
                    HtmlString += "</TR>";

                    // Получение имени артиста из запроса
                    artistName = context.Request.Query["artistName"];
                    if (!string.IsNullOrEmpty(artistName))
                    {
                        // Сохранение значения в cookies
                        context.Response.Cookies.Append("artistName", artistName);
                    }

                    // Фильтрация артистов по имени
                    foreach (var artist in artists.Where(e => e.Name.Trim().ToLower() == artistName?.Trim().ToLower()))
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + artist.ArtistId + "</TD>";
                        HtmlString += "<TD>" + artist.Name + "</TD>";
                        HtmlString += "<TD>" + artist.Description + "</TD>";
                        HtmlString += "<TD>" + artist.Members + "</TD>";
                        HtmlString += "</TR>";
                    }

                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Отправка HTML-строки в ответ
                    await context.Response.WriteAsync(HtmlString);
                });
            });


            app.Map("/broadcasts", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    ICachedBroadcastSchedulesService cachedBroadcastSchedulesService = context.RequestServices.GetService<ICachedBroadcastSchedulesService>();
                    IEnumerable<BroadcastSchedule> broadcastSchedules = cachedBroadcastSchedulesService.GetBroadcastSchedules(20);

                    string HtmlString = "<HTML><HEAD><TITLE>Таблица Расписания</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Расписание передач</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Идентификатор</TH>";
                    HtmlString += "<TH>Дата трансляции</TH>";
                    HtmlString += "<TH>Сотрудник</TH>";
                    HtmlString += "<TH>Запись</TH>";
                    HtmlString += "</TR>";

                    foreach (var broadcastSchedule in broadcastSchedules)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + broadcastSchedule.ScheduleId + "</TD>";
                        HtmlString += "<TD>" + broadcastSchedule.BroadcastDate.ToString("yyyy-MM-dd HH:mm") + "</TD>";
                        HtmlString += "<TD>" + broadcastSchedule.Employee.FullName + "</TD>";  
                        HtmlString += "<TD>" + broadcastSchedule.Record.Title + "</TD>";    
                        HtmlString += "</TR>";
                    }

                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Назад на главную</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Map("/employyes", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    ICacheEmployeesService cacheEmployeesService = context.RequestServices.GetService<ICacheEmployeesService>();
                    IEnumerable<Employee> employees = cacheEmployeesService.GetEmployees(20);

                    string HtmlString = "<HTML><HEAD><TITLE>Таблица Сотрудников</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Сотрудники</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Идентификатор</TH>";
                    HtmlString += "<TH>ФИО</TH>";
                    HtmlString += "<TH>Образование</TH>";
                    HtmlString += "<TH>Должность</TH>";
                    HtmlString += "</TR>";

                    foreach (var employee in employees)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + employee.EmployeeId + "</TD>";
                        HtmlString += "<TD>" + employee.FullName + "</TD>";
                        HtmlString += "<TD>" + employee.Education + "</TD>";
                        HtmlString += "<TD>" + employee.Position + "</TD>";
                        HtmlString += "</TR>";
                    }

                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Назад на главную</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Map("/genres", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    ICacheGenresService cacheGenresService = context.RequestServices.GetService<ICacheGenresService>();
                    IEnumerable<Genre> genres = cacheGenresService.GetGenres(20);

                    string HtmlString = "<HTML><HEAD><TITLE>Таблица Жанров</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Жанры</H1>" +
                    "<TABLE BORDER=1>";

                    HtmlString += "<TR>";
                    HtmlString += "<TH>Айди</TH>";
                    HtmlString += "<TH>Название</TH>";
                    HtmlString += "<TH>Описание</TH>";
                    HtmlString += "</TR>";

                    foreach (var genre in genres)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + genre.GenreId + "</TD>";
                        HtmlString += "<TD>" + genre.Name + "</TD>";
                        HtmlString += "<TD>" + genre.Description + "</TD>";
                        HtmlString += "</TR>";
                    }

                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Назад на главную</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Map("/records", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    ICacheRecordsService cacheRecordsService = context.RequestServices.GetService<ICacheRecordsService>();
                    IEnumerable<Record> records = cacheRecordsService.GetRecords(20); 

                    string HtmlString = "<HTML><HEAD><TITLE>Таблица Записей</TITLE></HEAD>" +
                        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                        "<BODY><H1>Записи</H1>" +
                        "<TABLE BORDER=1>";

                    HtmlString += "<TR>";
                    HtmlString += "<TH>Айди</TH>";
                    HtmlString += "<TH>Название</TH>";
                    HtmlString += "<TH>Исполнитель</TH>";
                    HtmlString += "<TH>Альбом</TH>";
                    HtmlString += "<TH>Год</TH>";
                    HtmlString += "<TH>Жанр</TH>";
                    HtmlString += "</TR>";

                    foreach (var record in records)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + record.RecordId + "</TD>";
                        HtmlString += "<TD>" + record.Title + "</TD>";
                        HtmlString += "<TD>" + record.Artist?.Name + "</TD>"; 
                        HtmlString += "<TD>" + record.Album + "</TD>";
                        HtmlString += "<TD>" + record.Year?.ToString() + "</TD>"; 
                        HtmlString += "<TD>" + record.Genre?.Name + "</TD>"; 
                        HtmlString += "</TR>";
                    }

                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Назад на главную</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    await context.Response.WriteAsync(HtmlString);
                });
            });


            app.Map("/searchRecords", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    string recordName = string.Empty;

                    // Проверяем, передано ли значение employeeName в запросе
                    if (!string.IsNullOrEmpty(context.Request.Query["recordName"]))
                    {
                        recordName = context.Request.Query["recordName"];
                        // Сохраняем employeeName в сессии
                        context.Session.SetString("recordName", recordName);
                    }
                    // Если employeeName уже сохранено в сессии, извлекаем его
                    else if (context.Session.Keys.Contains("recordName"))
                    {
                        recordName = context.Session.GetString("recordName");
                    }
                    ICacheRecordsService cacheRecordsService = context.RequestServices.GetService<ICacheRecordsService>();
                    IEnumerable<Record> records = cacheRecordsService.GetRecords(30);
                    string HtmlString = "<HTML><HEAD><TITLE>Записи</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список записей по названию</H1>" +
                    "<BODY><FORM action ='/searchRecords' method='get'>" +
                    "Запись:<BR><INPUT type = 'text' name = 'recordName' value = '" + recordName + "'>" +
                    "<BR><BR><INPUT type ='submit' value='Сохранить в сессию и вывести сотрудников с заданным именем'></FORM>" +
                    "<TABLE BORDER=1>";

                    HtmlString += "<TR>";
                    HtmlString += "<TH>Айди</TH>";
                    HtmlString += "<TH>Название</TH>";
                    HtmlString += "<TH>Исполнитель</TH>";
                    HtmlString += "<TH>Альбом</TH>";
                    HtmlString += "<TH>Год</TH>";
                    HtmlString += "<TH>Жанр</TH>";
                    HtmlString += "</TR>";
                    foreach (var record in records.Where(e => e.Title.Trim() == recordName))
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + record.RecordId + "</TD>";
                        HtmlString += "<TD>" + record.Title + "</TD>";
                        HtmlString += "<TD>" + record.Artist?.Name + "</TD>";
                        HtmlString += "<TD>" + record.Album + "</TD>";
                        HtmlString += "<TD>" + record.Year?.ToString() + "</TD>";
                        HtmlString += "<TD>" + record.Genre?.Name + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    await context.Response.WriteAsync(HtmlString);
                });
            });


            app.Map("/recorddetails", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    ICacheRecordDetailsService cacheRecordDetailsService = context.RequestServices.GetService<ICacheRecordDetailsService>();
                    IEnumerable<RecordDetail> recordDetails = cacheRecordDetailsService.GetRecordDetails(20); 

                    string HtmlString = "<HTML><HEAD><TITLE>Таблица Деталей Записей</TITLE></HEAD>" +
                        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                        "<BODY><H1>Детали Записей</H1>" +
                        "<TABLE BORDER=1>";

                    HtmlString += "<TR>";
                    HtmlString += "<TH>Айди</TH>";
                    HtmlString += "<TH>Запись</TH>"; 
                    HtmlString += "<TH>Дата Записи</TH>";
                    HtmlString += "<TH>Продолжительность</TH>";
                    HtmlString += "<TH>Рейтинг</TH>";
                    HtmlString += "</TR>";

                    foreach (var recordDetail in recordDetails)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + recordDetail.RecordDetailId + "</TD>";
                        HtmlString += "<TD>" + recordDetail.Record?.Title + "</TD>"; 
                        HtmlString += "<TD>" + recordDetail.RecordingDate.ToString("yyyy-MM-dd") + "</TD>"; 
                        HtmlString += "<TD>" + recordDetail.Duration + "</TD>"; 
                        HtmlString += "<TD>" + recordDetail.Rating + "</TD>"; 
                        HtmlString += "</TR>";
                    }

                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Назад на главную</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Run((context) =>
            {
                // Кэширование данных для всех таблиц
                ICachedArtistsService cachedArtistsService = context.RequestServices.GetService<ICachedArtistsService>();
                cachedArtistsService.AddArtists("Artists20", 20);
                ICachedBroadcastSchedulesService cachedBroadcastSchedulesService = context.RequestServices.GetService<ICachedBroadcastSchedulesService>();
                cachedArtistsService.AddArtists("BroadcastSchedules20", 20);
                ICacheEmployeesService cacheEmployeesService = context.RequestServices.GetService<ICacheEmployeesService>();
                cachedArtistsService.AddArtists("Employyes20", 20);
                ICacheGenresService cacheGenresService = context.RequestServices.GetService<ICacheGenresService>();
                cachedArtistsService.AddArtists("Genres20", 20);
                ICacheRecordDetailsService cacheRecordDetailsService = context.RequestServices.GetService<ICacheRecordDetailsService>();
                cachedArtistsService.AddArtists("recordDetails20", 20);
                ICacheRecordsService cacheRecordsService = context.RequestServices.GetService<ICacheRecordsService>();
                cachedArtistsService.AddArtists("records20", 20);



                string HtmlString = "<HTML><HEAD><TITLE>Главная</TITLE></HEAD>" +
                "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                "<BODY><H1>Главная</H1>";
                HtmlString += "<H2>Данные записаны в кэш сервера</H2>";
                HtmlString += "<BR><A href='/'>Главная</A></BR>";
                HtmlString += "<BR><A href='/artists'>Артисты</A></BR>";
                HtmlString += "<BR><A href='/searchArtists'>Поиск по артистам</A></BR>";
                HtmlString += "<BR><A href='/broadcasts'>Расписание</A></BR>";
                HtmlString += "<BR><A href='/employyes'>Работники</A></BR>";
                HtmlString += "<BR><A href='/genres'>Жанры</A></BR>";
                HtmlString += "<BR><A href='/records'>Записи</A></BR>";
                HtmlString += "<BR><A href='/searchRecords'>Поиск по записям</A></BR>";
                HtmlString += "<BR><A href='/recordDetails'>Детали Записей</A></BR>";
                HtmlString += "<BR><A href='/info'>Информация </A></BR>";
                HtmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(HtmlString);
            });

            app.Run();
        }
    }
}