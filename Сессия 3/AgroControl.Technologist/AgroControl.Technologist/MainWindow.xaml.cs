using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AgroControl.API.Models;
using AgroControl.Technologist.Services;
using AgroControl.Technologist.Views;

namespace AgroControl.Technologist
{
    public partial class MainWindow : Window
    {
        private readonly ApiService api;

        public MainWindow(ApiService api)
        {
            InitializeComponent();
            this.api = api;
        }

        public async Task LoadUserProfile(string username)
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
            }
            catch { ShowPlaceholder(username); }
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

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuList.SelectedItem is ListBoxItem item)
            {
                var tag = item.Tag?.ToString();
                switch (tag)
                {
                    case "Dashboard": MainContent.Content = new DashboardPage(api); break;
                    case "Products": MainContent.Content = new ProductsPage(api); break;          // исправлено
                    case "Recipes": MainContent.Content = new RecipesPage(api); break;
                    case "TechCards": MainContent.Content = new TechCardsPage(api); break;
                    case "Orders": MainContent.Content = new OrdersPage(api); break;            // исправлено
                    case "Batches": MainContent.Content = new BatchesPage(api); break;          // исправлено
                    case "ExtruderPrograms": MainContent.Content = new ExtruderProgramsPage(api); break; // исправлено
                    case "Deviations": MainContent.Content = new DeviationsPage(api); break;    // исправлено
                    case "Reports": MainContent.Content = new ReportsPage(api); break;
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