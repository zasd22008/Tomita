using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tomita
{

    public class OlympicGames
    {
        public int ArticleId;
        public DateTime Year;
    }

    class Program
    {
        static void Main(string[] args)
        {
            //1.) Получаем список id статей по шаблону "Зимние Олимпийские игры {год}"
            var olympicGames = GetOlympicArticles();

            //2.) Находим и парсим каждую статью с таким id-шником

            //3.) Сохраняем данные в файл

            //4.) Вызываем томиту

            //5.) Дополняем название страны данными о прочем..
          
            Console.ReadLine();             
        }

        private static List<OlympicGames> GetOlympicArticles()
        {
            var indexFile = @"D:\index.txt";

            var streamReader = new StreamReader(File.OpenRead(indexFile));

            var regex = new Regex(@"\A([0-9]+:(?<id>[0-9]+):Зимние Олимпийские игры (?<year>[0-9]{4}))\z");

            var olympicGames = new List<OlympicGames>();

            var s = "";
            while ((s = streamReader.ReadLine()) != null)
            {
                if (s.Contains("Зимние Олимпийские игры"))
                {
                    if (regex.IsMatch(s))
                    {
                        var match = regex.Match(s);

                        if (match != null)
                            olympicGames.Add(
                                new OlympicGames()
                                {
                                    ArticleId = Int32.Parse(match.Groups["id"].Value),
                                    Year = DateTime.ParseExact(match.Groups["year"].Value, "yyyy", CultureInfo.InvariantCulture)
                                }
                            );
                    }
                }
            }

            streamReader.Close();

            return olympicGames;
        }
    }
}
