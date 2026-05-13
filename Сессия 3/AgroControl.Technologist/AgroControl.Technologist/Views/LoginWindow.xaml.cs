using System.Windows;
using AgroControl.Technologist.Helpers;
using AgroControl.Technologist.Services;

namespace AgroControl.Technologist.Views
{
    public partial class LoginWindow : Window
    {
        private string captchaText;
        private ApiService api;

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

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (captchaBox.Text.Trim().ToUpper() != captchaText)
            {
                errorText.Text = "Неверная капча";
                GenerateCaptcha();
                return;
            }

            var result = await api.LoginAsync(loginBox.Text.Trim(), passwordBox.Password);
            if (result != null && result.Success)
            {
                var mainWindow = new MainWindow(api);
                mainWindow.Show();
                await mainWindow.LoadUserProfile(loginBox.Text.Trim());
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