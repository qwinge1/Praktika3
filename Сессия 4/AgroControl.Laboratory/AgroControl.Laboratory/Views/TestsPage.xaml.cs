using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Laboratory.Views
{
    public partial class TestsPage : UserControl
    {
        private ApiService api;
        private List<LabTest> allTests = new();
        private string currentUser;

        public TestsPage(ApiService api, string user)
        {
            InitializeComponent();
            this.api = api;
            currentUser = user;
            Loaded += async (s, e) => await LoadTests();
        }

        private async Task LoadTests()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<LabTest>>>("api/QualityControl");
                allTests = response.Data ?? new List<LabTest>();
                // Загружаем номера партий для отображения
                foreach (var test in allTests)
                {
                    if (test.ПартияСырьяID != null)
                    {
                        var batch = await api.GetAsync<ApiResponse<RawMaterialBatch>>($"api/RawMaterialBatches/{test.ПартияСырьяID}");
                        test.BatchNumber = batch.Data?.НомерПартии;
                    }
                    else if (test.ПартияПроизводстваID != null)
                    {
                        var batch = await api.GetAsync<ApiResponse<ProductionBatch>>($"api/Batches/{test.ПартияПроизводстваID}");
                        test.BatchNumber = batch.Data?.НомерПартии;
                    }
                }
                testsGrid.ItemsSource = allTests;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadTests();

        private async void CreateTest_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new LabTestEditWindow(api, currentUser);
            if (dialog.ShowDialog() == true) await LoadTests();
        }

        private async void EditTest_Click(object sender, RoutedEventArgs e)
        {
            if (testsGrid.SelectedItem is LabTest selected && selected.ID > 0)
            {
                var dialog = new LabTestEditWindow(api, selected, currentUser);
                if (dialog.ShowDialog() == true) await LoadTests();
            }
            else MessageBox.Show("Выберите испытание");
        }

        private async void DeleteTest_Click(object sender, RoutedEventArgs e)
        {
            if (testsGrid.SelectedItem is LabTest selected)
            {
                if (MessageBox.Show("Удалить выбранное испытание?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    return;
                try
                {
                    await api.DeleteAsync($"api/QualityControl/{selected.ID}");
                    MessageBox.Show("Испытание удалено");
                    await LoadTests();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
            }
            else MessageBox.Show("Выберите испытание");
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(term))
            {
                testsGrid.ItemsSource = allTests;
                return;
            }
            var filtered = allTests.Where(t => (t.НаименованиеПараметра?.ToLower().Contains(term) ?? false) ||
                                               (t.ТипОбразца?.ToLower().Contains(term) ?? false) ||
                                               (t.BatchNumber?.ToLower().Contains(term) ?? false)).ToList();
            testsGrid.ItemsSource = filtered;
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            testsGrid.ItemsSource = allTests;
        }
    }
}