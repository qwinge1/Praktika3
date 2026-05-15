using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AgroControl.API.Models;
using AgroControl.Operator.Services;
using AgroControl.Operator.Views;

namespace AgroControl.Operator
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _api = new();
        private int _currentBatchId;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadUserProfile(App.CurrentUser);
            MenuList.SelectedIndex = 0;
            lblShift.Text = "Смена 2";
            lblLine.Text = "L-01";
        }

        private async Task LoadUserProfile(string? username)
        {
            if (string.IsNullOrEmpty(username)) return;
            try
            {
                var users = await _api.GetUsersAsync();
                var user = users?.FirstOrDefault(u => u.ИмяПользователя == username);
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
            return parts.Length >= 2 ? $"{parts[0][0]}{parts[1][0]}".ToUpper() : parts[0][0].ToString().ToUpper();
        }

        private BitmapImage? ByteArrayToBitmapImage(byte[] imageData)
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

        // MainWindow.xaml.cs
        public void LoadActiveBatchesPage()
        {
            MainContent.Content = new ActiveBatchesPage(_api, this);
        }

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuList.SelectedItem is ListBoxItem item)
            {
                switch (item.Tag?.ToString())
                {
                    case "ActiveBatches":
                        LoadActiveBatchesPage();
                        break;
                    case "Program":
                        MainContent.Content = new ProgramPage(_api, _currentBatchId, this);
                        break;
                    case "Extruder":
                        MainContent.Content = new ExtruderPage(_api);
                        break;
                    case "Journal":
                        MainContent.Content = new JournalPage(_api, _currentBatchId);
                        break;
                    case "Report":
                        MainContent.Content = new ReportPage(_api, _currentBatchId);
                        break;
                    case "Issues":
                        MainContent.Content = new IssuesPage(_api);
                        break;
                }
            }
        }
        private async void DeleteBatch_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBatchId == 0)
            {
                MessageBox.Show("Сначала выберите партию в списке активных партий", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить партию ID {_currentBatchId}? Все данные выполнения шагов будут удалены безвозвратно.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            var success = await _api.DeleteBatchAsync(_currentBatchId);
            if (success)
            {
                MessageBox.Show("Партия удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _currentBatchId = 0;
                LoadActiveBatchesPage(); // переходим на список активных партий
            }
            else
            {
                MessageBox.Show("Не удалось удалить партию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SetCurrentBatch(int batchId)
        {
            _currentBatchId = batchId;
            foreach (ListBoxItem item in MenuList.Items)
                if (item.Tag?.ToString() == "Program")
                    MenuList.SelectedItem = item;
        }

        private async void CreateBatch_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateBatchWindow(_api);
            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show("Партия создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // Обновить список активных партий, если открыта страница
                if (MainContent.Content is ActiveBatchesPage page)
                    await page.RefreshBatches();
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}