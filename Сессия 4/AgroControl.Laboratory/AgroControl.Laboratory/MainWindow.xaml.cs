using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using AgroControl.Laboratory.Views;

namespace AgroControl.Laboratory
{
    public partial class MainWindow : Window
    {
        private readonly ApiService api;
        private string currentUser;

        public MainWindow(ApiService api, string userName)
        {
            InitializeComponent();
            this.api = api;
            currentUser = userName;
            Loaded += async (s, e) =>
            {
                await LoadUserProfile(userName);
                MainContent.Content = new RawMaterialBatchesPage(api);
            };
        }

        private async System.Threading.Tasks.Task LoadUserProfile(string username)
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<User>>>("api/Users");
                var user = response?.Data?.FirstOrDefault(u => u.ИмяПользователя == username);
                if (user != null)
                {
                    userFullName.Text = user.ПолноеИмя;
                    userRole.Text = user.Роль;
                    if (user.Фото != null && user.Фото.Length > 0)
                    {
                        var image = ByteArrayToBitmapImage(user.Фото);
                        if (image != null)
                        {
                            userPhoto.Source = image;
                            userPhoto.Visibility = Visibility.Visible;
                            userPhotoPlaceholder.Visibility = Visibility.Collapsed;
                            return;
                        }
                    }
                    ShowPlaceholder(user.ПолноеИмя);
                }
                else ShowPlaceholder(username);
            }
            catch { ShowPlaceholder(username); }
        }

        private void ShowPlaceholder(string fullName)
        {
            userPhoto.Visibility = Visibility.Collapsed;
            userPhotoPlaceholder.Visibility = Visibility.Visible;
            userPhotoPlaceholderText.Text = GetInitials(fullName);
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "??";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2) return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts[0][0].ToString().ToUpper();
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            try
            {
                var image = new BitmapImage();
                using (var mem = new MemoryStream(imageData))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
            catch { return null; }
        }

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuList.SelectedItem is ListBoxItem item)
            {
                var tag = item.Tag?.ToString();
                switch (tag)
                {
                    case "RawMaterialBatches":
                        MainContent.Content = new RawMaterialBatchesPage(api);
                        break;
                }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        private void Avatar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { }
        private void UserMenuItem_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Профиль пользователя (заглушка)");
        private void ChangePasswordMenuItem_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Смена пароля (заглушка)");
    }
}