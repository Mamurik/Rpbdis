using System;
using System.Linq;
using RadiostationWeb.Models;

namespace RadiostationWeb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(RadioStationDbContext db)
        {
            db.Database.EnsureCreated();

            // Проверка, есть ли записи в базе данных
            if (db.Artists.Any() || db.Records.Any() || db.Genres.Any() || db.Employees.Any() || db.RecordDetails.Any())
            {
                return; // База данных уже инициализирована
            }

            Random rand = new Random();

            // Вставка данных для Жанров
            string[] genres = { "Поп", "Рок", "Джаз", "Классика", "Хип-Хоп", "Кантри" };
            foreach (var genreName in genres)
            {
                db.Genres.Add(new Genre { Name = genreName, Description = "Описание для " + genreName });
            }
            db.SaveChanges();

            // Вставка данных для Артистов
            for (int i = 0; i < 100; i++)
            {
                db.Artists.Add(new Artist
                {
                    Name = "Артист " + (i + 1),
                    Members = "Участники Артиста " + (i + 1),
                    Description = "Описание для Артиста " + (i + 1)
                });
            }
            db.SaveChanges();

            // Вставка данных для Сотрудников
            for (int i = 0; i < 100; i++)
            {
                db.Employees.Add(new Employee
                {
                    FullName = "Сотрудник " + (i + 1),
                    Education = "Бакалавр музыки",
                    Position = "Диджей"
                });
            }
            db.SaveChanges();

            // Вставка данных для Записей
            for (int i = 0; i < 100; i++)
            {
                var record = new Record
                {
                    Title = "Запись " + (i + 1),
                    ArtistId = rand.Next(1, 101), // Случайный артист из 100
                    Album = "Альбом " + (i + 1),
                    Year = 2000 + rand.Next(0, 23), // Случайный год от 2000 до 2023
                    GenreId = rand.Next(1, genres.Length + 1)
                };

                db.Records.Add(record);
                db.SaveChanges(); // Сохраняем запись, чтобы получить RecordId для RecordDetail

                // Вставка данных для деталей записи
                db.RecordDetails.Add(new RecordDetail
                {
                    RecordId = record.RecordId,
                    RecordingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-rand.Next(100))), // Случайная дата записи
                    Duration = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(rand.Next(1, 10) * 60 + rand.Next(0, 60))), // Случайная продолжительность
                    Rating = rand.Next(1, 6) // Случайный рейтинг от 1 до 5
                });
            }
            db.SaveChanges();

            // Вставка данных для Расписания Эфиров
            for (int i = 0; i < 100; i++)
            {
                db.BroadcastSchedules.Add(new BroadcastSchedule
                {
                    BroadcastDate = DateTime.Now.AddDays(i), // Запланировать эфиры на следующие 100 дней
                    EmployeeId = rand.Next(1, 101), // Случайный сотрудник из 100
                    RecordId = rand.Next(1, 101) // Случайная запись из 100
                });
            }
            db.SaveChanges();
        }
    }
}