using System;
using System.Collections.Generic;
using Rpbdis2.models;
using Rpbdis2.data;

namespace Rpbdis2
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new RadioStationDbContext();
            var dataOperations = new DataOperations(context);

            while (true)
            {
                Console.Clear(); // Очищаем экран для чистоты интерфейса
                Console.WriteLine("==============================================");
                Console.WriteLine("            Радиостанция - Меню           ");
                Console.WriteLine("==============================================");
                Console.WriteLine("1  Показать всех исполнителей");
                Console.WriteLine("2  Найти исполнителей по условию");
                Console.WriteLine("3  Показать количество записей по жанрам");
                Console.WriteLine("4  Показать названия записей и имена исполнителей");
                Console.WriteLine("5  Фильтр по названию записи");
                Console.WriteLine("6  Добавить исполнителя");
                Console.WriteLine("7  Добавить запись");
                Console.WriteLine("8  Удалить исполнителя");
                Console.WriteLine("9  Удалить запись");
                Console.WriteLine("10 Обновить записи по названию");
                Console.WriteLine("0  Выход");
                Console.WriteLine("==============================================");
                Console.Write("  Выберите действие: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllArtists(dataOperations);
                        break;
                    case "2":
                        FindArtistsByCondition(dataOperations);
                        break;
                    case "3":
                        ShowRecordsCountByGenre(dataOperations);
                        break;
                    case "4":
                        ShowArtistRecordTitles(dataOperations);
                        break;
                    case "5":
                        FilterArtistRecordTitles(dataOperations);
                        break;
                    case "6":
                        AddArtist(dataOperations);
                        break;
                    case "7":
                        AddRecord(dataOperations);
                        break;
                    case "8":
                        DeleteArtist(dataOperations);
                        break;
                    case "9":
                        DeleteRecord(dataOperations);
                        break;
                    case "10":
                        UpdateRecordsByCondition(dataOperations);
                        break;
                    case "0":
                        Console.WriteLine(" Спасибо за использование программы!");
                        return;
                    default:
                        Console.WriteLine("  Неверный выбор. Попробуйте еще раз.");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу, чтобы вернуться в меню...");
                Console.ReadKey(); // Пауза перед возвратом к меню
            }
        }
        static void ShowAllArtists(DataOperations dataOperations)
        {
            Console.Clear();
            Console.WriteLine("Все исполнители:");

            if (dataOperations == null)
            {
                Console.WriteLine("Ошибка: dataOperations не инициализирован.");
                return;
            }

            var artists = dataOperations.GetAllArtists();

            if (artists == null)
            {
                Console.WriteLine("Ошибка: список исполнителей пуст.");
                return;
            }

            foreach (var artist in artists)
            {
                Console.WriteLine($"ID: {artist.ArtistId}, Имя: {artist.Name}");
            }
        }

        static void FindArtistsByCondition(DataOperations dataOperations)
        {
            Console.Clear();
            Console.Write(" Введите имя исполнителя: ");
            var condition = Console.ReadLine();
            var artists = dataOperations.GetArtistsByCondition(condition);
            if (artists.Count > 0)
            {
                Console.WriteLine(" Найденные исполнители:");
                foreach (var artist in artists)
                {
                    Console.WriteLine($"ID: {artist.ArtistId}, Имя: {artist.Name}");
                }
            }
            else
            {
                Console.WriteLine(" Исполнители не найдены.");
            }
        }

        static void ShowRecordsCountByGenre(DataOperations dataOperations)
        {
            Console.Clear();
            Console.WriteLine(" Количество записей по жанрам:");
            var counts = dataOperations.GetRecordsCountByGenre();
            foreach (var count in counts)
            {
                Console.WriteLine($" Жанр ID: {count.GenreId}, Количество записей: {count.RecordCount}");
            }
        }

        static void ShowArtistRecordTitles(DataOperations dataOperations)
        {
            Console.Clear();
            Console.WriteLine(" Исполнители и их записи:");
            var records = dataOperations.GetArtistRecordTitles();
            foreach (var record in records)
            {
                Console.WriteLine($" Исполнитель: {record.ArtistName}, Название записи: {record.RecordTitle}");
            }
        }

        static void FilterArtistRecordTitles(DataOperations dataOperations)
        {
            Console.Clear();
            Console.Write(" Введите название записи: ");
            var condition = Console.ReadLine();
            var records = dataOperations.GetFilteredArtistRecordTitles(condition);
            if (records.Count > 0)
            {
                Console.WriteLine(" Найденные записи:");
                foreach (var record in records)
                {
                    Console.WriteLine($"Исполнитель: {record.ArtistName}, Название записи: {record.RecordTitle}");
                }
            }
            else
            {
                Console.WriteLine(" Записи не найдены.");
            }
        }

        static void AddArtist(DataOperations dataOperations)
        {
            Console.Clear();
            Console.Write(" Введите имя нового исполнителя: ");
            var name = Console.ReadLine();
            var artist = new Artist { Name = name };
            dataOperations.AddArtist(artist);
            Console.WriteLine(" Исполнитель добавлен.");
        }

        static void AddRecord(DataOperations dataOperations)
        {
            Console.Clear();
            Console.Write(" Введите ID исполнителя: ");
            var artistId = int.Parse(Console.ReadLine());
            Console.Write(" Введите ID жанра: ");
            var genreId = int.Parse(Console.ReadLine());
            Console.Write(" Введите название записи: ");
            var title = Console.ReadLine();

            var record = new Record { ArtistId = artistId, GenreId = genreId, Title = title };
            dataOperations.AddRecord(record);
            Console.WriteLine(" Запись добавлена.");
        }

        static void DeleteArtist(DataOperations dataOperations)
        {
            Console.Clear();
            Console.Write(" Введите ID исполнителя для удаления: ");
            var artistId = int.Parse(Console.ReadLine());
            dataOperations.DeleteArtist(artistId);
            Console.WriteLine(" Исполнитель удалён.");
        }

        static void DeleteRecord(DataOperations dataOperations)
        {
            Console.Clear();
            Console.Write(" Введите ID записи для удаления: ");
            var recordId = int.Parse(Console.ReadLine());
            dataOperations.DeleteRecord(recordId);
            Console.WriteLine(" Запись удалена.");
        }

        static void UpdateRecordsByCondition(DataOperations dataOperations)
        {
            Console.Clear();
            Console.Write(" Введите старое название записи: ");
            var oldTitle = Console.ReadLine();
            Console.Write(" Введите новое название записи: ");
            var newTitle = Console.ReadLine();
            dataOperations.UpdateRecordsByCondition(oldTitle, newTitle);
            Console.WriteLine(" Записи обновлены.");
        }
    }
}
