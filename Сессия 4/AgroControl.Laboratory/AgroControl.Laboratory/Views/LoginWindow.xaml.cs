using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using AgroControl.Laboratory.Helpers;
using System.Windows;

namespace AgroControl.Laboratory.Views
{
    public partial class LoginWindow : Window
    {
        private ApiService api;
        private string captchaText;

        public LoginWindow()
        {
            InitializeComponent();
            api = new ApiService();
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            captchaText = CaptchaGenerator.GenerateRandomText();
            captchaImage.Source = CaptchaGenerator.GenerateImage(captchaText);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            // Проверка капчи
            if (captchaBox.Text.Trim().ToUpper() != captchaText)
            {
                errorText.Text = "Неверный код";
                GenerateCaptcha();
                return;
            }

            var result = await api.LoginAsync(loginBox.Text, passwordBox.Password);
            if (result != null && result.Success)
            {
                var mainWindow = new MainWindow(api, loginBox.Text);
                mainWindow.Show();
                Close();
            }
            else
            {
                errorText.Text = "Неверный логин или пароль";
                GenerateCaptcha();
            }
        }
    }
}