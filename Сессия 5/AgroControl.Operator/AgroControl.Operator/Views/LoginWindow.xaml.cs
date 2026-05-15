using System.Windows;
using AgroControl.Operator.Services;

namespace AgroControl.Operator.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _api = new();

        public LoginWindow() => InitializeComponent();

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Collapsed;
            var success = await _api.LoginAsync(txtLogin.Text, txtPassword.Password);
            if (success)
            {
                App.CurrentUser = txtLogin.Text;
                // Определяем ID пользователя (можно запросить отдельно, но для простоты)
                if (txtLogin.Text == "test.user")
                    App.CurrentUserId = 14;
                else if (txtLogin.Text == "operator.zavodov")
                    App.CurrentUserId = 5;
                else
                    App.CurrentUserId = 5;

                var main = new MainWindow();
                main.Show();
                Close();
            }
            else
            {
                lblError.Text = "Неверный логин или пароль";
                lblError.Visibility = Visibility.Visible;
            }
        }
    }
}