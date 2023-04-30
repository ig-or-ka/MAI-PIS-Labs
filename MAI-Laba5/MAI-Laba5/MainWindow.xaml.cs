/*
 * Лабораторная работа №5,6 
 * Программа для считывания и вывода курсов валют с сайта rbc.ru
 */

//Импорт библиотеки WPF
using System.Windows;
//Импорт библиотеки для работы с сетью по протоколу HTTP
using System.Net.Http;
//Импорт библиотеки для работы с json форматом
using Newtonsoft.Json.Linq;
//Импорт основных инструментов C#
using System;
//Импорт коллекции Dictionary
using System.Collections.Generic;

//Модуль лабораторной работы №5
namespace MAI_Laba5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    //Класс главного окна программы
    public partial class MainWindow : Window
    {
        //Объявление словаря для хранения курсов валют 
        Dictionary<string, string> ValList = new Dictionary<string, string>();
        //Конструктор класса главного окна
        public MainWindow()
        {
            //Метод инициализации окна
            InitializeComponent();
        }

        //Обработчик нажатия на кнопку загрузки
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            //Создание объекта HTTP клиента
            var client = new HttpClient();
            //Добавление заголовка user-agent из браузера chrome
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36");
            //Добавление заголовка поддерживаемых типов контента
            client.DefaultRequestHeaders.Add("accept", "*/*");
            //Добавление заголовка url источника запроса
            client.DefaultRequestHeaders.Add("referer", "https://quote.rbc.ru/");
            //Добавление заголовка поддерживаемых языков
            client.DefaultRequestHeaders.Add("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7,zh;q=0.6");

            //Получение текущего времени в миллисекундах для передачи в параметры запроса
            var unix_time = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();

            //GET запрос на получение текущих курсов валют
            var resp = client.GetAsync($"https://quote.rbc.ru/v5/ajax/key-indicator-update/?_={unix_time}").Result;
            //Обработка ответного кода
            resp.EnsureSuccessStatusCode();

            //Считывание результата запроса
            var text = resp.Content.ReadAsStringAsync().Result;
            //Структуризация json результата в объекты библиотеки Newtonsoft.Json
            var result_json = JObject.Parse(text);

            //Обнуление словаря для хранения курсов валют
            ValList = new Dictionary<string, string>();

            //Цикл перебора списка валют
            foreach(JObject item in result_json["shared_key_indicators_under_topline"])
            {
                //Сохранение информации о валюте в переменную
                var vals = item["item"] as JObject;
                //Условие, что курс валюты определен
                if (vals["closevalue"].ToString() != "")
                {
                    //Запись валюты в словарь
                    ValList[vals["ticker"].ToString()] = vals["closevalue"].ToString();
                }                
                
            }

            //Запись списка валют в ComboBox
            ValSelect.ItemsSource = ValList.Keys;
        }

        //Обработчик выбора валюты в ComboBox
        private void ValSelect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //Запись курса выбранной валюты ComboBox
            ValSelected.Text = ValList[ValSelect.SelectedItem.ToString()];
        }
    }
}
