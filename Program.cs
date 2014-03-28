﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Tomita
{
    [Serializable]
    public class OlympicGames
    {
        public int ArticleId { get; set; }
        public DateTime Year { get; set; }
        public string Title { get; set; }
        public string Town { get; set; }
        public string Slogan { get; set; }
        public int CountriesCount { get; set; }
        public int ParticipantsCount { get; set; }
        public int MedalsCount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string OpenMan { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //тестирование

            var testString = @"|  Название               = XIX зимние Олимпийские Игры
|  Эмблема                = Изображение:2002W emblem b.jpg
|  Описание               = Эмблема Зимних Олимпийских игр 2002
|  Размер                 = 200
|  Город                  = {{Флаг США}}[[Солт-Лейк-Сити]], [[Соединённые Штаты Америки|США]]
|  Страны                 = 77 
|  Участников             = 2399 (1513 мужчин, 886 женщин)
|  Медалей                = 78 комплектов в 15 видах спорта 
|  Открытие               = [[8 февраля]] 
|  Закрытие               = [[24 февраля]]";

            var s = new OlympicGames();
            s.ArticleId = 34234;
            s.Year = new DateTime(2014, 1, 1);
            //FillGame(s, testString);


            //1.) Получаем список id статей по шаблону "Зимние Олимпийские игры {год}"
            var olympicGames = GetOlympicArticles();

            //2.) Находим и парсим каждую статью с таким id-шником
            FillOlympicGames(olympicGames);

            
            //3.) Сохраняем данные в файл
            SaveGames(olympicGames);

            //4.) Вызываем томиту

            //5.) Дополняем название страны данными о прочем..
          
            Console.ReadLine();             
        }

        private static void SaveGames(List<OlympicGames> list)
        {
            new XmlSerializer(typeof(List<OlympicGames>)).Serialize(File.OpenWrite("olympic.xml"), list);
        }

        private static void FillOlympicGames(List<OlympicGames> olympicGames)
        {
            var xmlFile = @"D:\wikipedia.xml";

            var s = "";
            var reader = new StreamReader(File.OpenRead(xmlFile));

            bool in_page = false;

            int full_count = 0;
            
            while ((s = reader.ReadLine()) != null && full_count < 21)
            {
                if (s.EndsWith("<page>"))
                {
                    var title = "";

                    for (int i = 0; i < 3; i++)
                        title += "\r\n" + reader.ReadLine();

                    var olympicGame = olympicGames.FirstOrDefault(o => title.Contains("<id>" + o.ArticleId + "</id>"));

                    if (olympicGame != null)
                    {
                        while (!(s = reader.ReadLine()).Contains("{{Олимпиада"))
                        {
                        }

                        var stringList = new StringBuilder();

                        while ((s = reader.ReadLine()).StartsWith("|") || s.EndsWith("|"))
                        {
                            if (s.StartsWith("|"))
                                stringList.AppendLine(s);
                            else
                            {
                                stringList.AppendLine("|" + s.Substring(0, s.Length - 1));
                            }
                        }

                        FillGame(olympicGame, stringList.ToString());

                        full_count++;
                    }
                }
            }
        }

        private static void FillGame(OlympicGames olympicGame, string list)
        {
            //var regex = new Regex(@"Название = .*\n");
            var regex = new Regex(@"Название\s*= (?<name>.+)\n");

            if (regex.IsMatch(list))
                olympicGame.Title = regex.Match(list).Groups["name"].Value;

            regex = new Regex(@"Город\s*= .* \[\[(?<town>.+)\]\]\n");

            if (regex.IsMatch(list))
                olympicGame.Town = regex.Match(list).Groups["town"].Value;

            regex = new Regex(@"Слоган\s*= (?<name>.+)\n");

            if (regex.IsMatch(list))
                olympicGame.Slogan = regex.Match(list).Groups["name"].Value;

            regex = new Regex(@"Страны\s*= (?<count>\d+).*\n");

            if (regex.IsMatch(list))
                olympicGame.CountriesCount = int.Parse(regex.Match(list).Groups["count"].Value);

            regex = new Regex(@"Участников\s*= (?<count>\d+).*\n");

            if (regex.IsMatch(list))
                olympicGame.ParticipantsCount = int.Parse(regex.Match(list).Groups["count"].Value);

            regex = new Regex(@"Медалей\s*= (?<count>\d+) .*\n");

            if (regex.IsMatch(list))
                olympicGame.MedalsCount = int.Parse(regex.Match(list).Groups["count"].Value);

            regex = new Regex(@"Открытие\s*= \[\[(?<md>.+)\]\].*\n");

            if (regex.IsMatch(list))
                olympicGame.StartDate = regex.Match(list).Groups["md"].Value;

            regex = new Regex(@"Открывал\s*= \[\[.*?\|(?<name>.*?)\]\]\n");

            if (regex.IsMatch(list))
                olympicGame.OpenMan = regex.Match(list).Groups["name"].Value;

            regex = new Regex(@"Закрытие\s*= \[\[(?<md>.+)\]\].*\n");

            if (regex.IsMatch(list))
                olympicGame.EndDate = regex.Match(list).Groups["md"].Value;
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
