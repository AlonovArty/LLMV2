using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GigaChatv2.Models;
using GigaChatv2.Service;

namespace GigaChatv2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string Token;

        public MainWindow()
        {
            InitializeComponent();
            InitToken();
        }
        private async void InitToken()
        {
            Token = await GigaChatService.GetToken();
            if (Token == null)
            {
                MessageBox.Show("Ошибка получения токена");
                Close();
            }
        }

        private async void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (Token == null)
            {
                MessageBox.Show("Токен не получен!");
                return;
            }

            string text = PromptBox.Text.Trim();
            ComboBoxItem style = StyleBox.SelectedItem as ComboBoxItem;
            ComboBoxItem color = ColorBox.SelectedItem as ComboBoxItem;
            ComboBoxItem aspect = AspectBox.SelectedItem as ComboBoxItem;

            string prompt =
                "Создай изображение по описанию: \"" + text + "\". " +
                "Стиль: " + style.Content + ". " +
                "Цветокоррекция: " + color.Content + ". " +
                "Размер: " + aspect.Content + ". ";
            
            GigaChatService.DialogHistory.Add(new Request.Message
            {
                role = "user",
                content = prompt
            });

            var answer = await GigaChatService.GetAnswer(Token, GigaChatService.DialogHistory);

            string content = answer.choices[0].message.content;
            MessageBox.Show(content);
            string fileId = GigaChatService.ExtractImageId(content);

            byte[] img = await GigaChatService.DownloadImage(Token, fileId);

            string path = GigaChatService.SaveImage(img);

            Classes.WallpaperSetter.SetWallpaper(path);

            MessageBox.Show("Обои успешно установлены!");
        }
    }
}

