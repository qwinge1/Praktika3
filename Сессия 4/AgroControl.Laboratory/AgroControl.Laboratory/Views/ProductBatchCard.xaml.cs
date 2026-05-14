using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AgroControl.Laboratory.Views
{
    public partial class ProductBatchCard : Window
    {
        private ApiService api;
        private int batchId;
        private ProductionBatch batch;
        private List<LabTest> tests;
        private string currentUser;

        public ProductBatchCard(ApiService api, int batchId, string user)
        {
            InitializeComponent();
            this.api = api;
            this.batchId = batchId;
            currentUser = user;
            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                var batchResp = await api.GetAsync<ApiResponse<ProductionBatch>>($"api/Batches/{batchId}");
                batch = batchResp.Data;
                DataContext = batch;
                await LoadTests();
                LoadHistory();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async Task LoadTests()
        {
            var testsResp = await api.GetAsync<ApiResponse<List<LabTest>>>($"api/QualityControl?batchId={batchId}");
            tests = testsResp.Data ?? new List<LabTest>();
            testsGrid.ItemsSource = tests;
        }

        private void LoadHistory()
        {
            var history = new List<string>();
            if (!string.IsNullOrEmpty(batch.РешениеПринял))
                history.Add($"{batch.ДатаРешения:dd.MM.yyyy HH:mm} - {batch.РешениеПринял}: {batch.ЛабораторныйСтатус}. Комментарий: {batch.КомментарийРешения}");
            historyItems.ItemsSource = history;
        }

        private async void AddTest_Click(object sender, RoutedEventArgs e)
        {
            if (tests.Any(t => t.Статус != "завершено"))
            {
                MessageBox.Show("Для этой партии уже есть незавершённое испытание.");
                return;
            }
            var dialog = new LabTestEditWindow(api, batch, currentUser);
            if (dialog.ShowDialog() == true) await LoadTests();
        }

        private void EditTest_Click(object sender, RoutedEventArgs e)
        {
            if (testsGrid.SelectedItem is LabTest selected)
            {
                var dialog = new LabTestEditWindow(api, selected, currentUser);
                if (dialog.ShowDialog() == true) LoadTests();
            }
            else MessageBox.Show("Выберите испытание");
        }

        private async void DeleteTest_Click(object sender, RoutedEventArgs e)
        {
            if (testsGrid.SelectedItem is LabTest selected)
            {
                if (MessageBox.Show("Удалить испытание?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
                try
                {
                    await api.DeleteAsync($"api/QualityControl/{selected.ID}");
                    MessageBox.Show("Испытание удалено");
                    await LoadTests();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
            else MessageBox.Show("Выберите испытание");
        }

        private async void RefreshTests_Click(object sender, RoutedEventArgs e) => await LoadTests();

        private void TestsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { }

        private async void MakeDecision_Click(object sender, RoutedEventArgs e)
        {
            if (!tests.Any())
            {
                decisionError.Text = "Нет ни одного испытания.";
                return;
            }
            if (tests.Any(t => t.Статус != "завершено" || string.IsNullOrEmpty(t.Результат)))
            {
                decisionError.Text = "Есть незавершённые испытания.";
                return;
            }
            if (tests.All(t => t.Результат != "pass"))
            {
                decisionError.Text = "Ни одно испытание не пройдено.";
                return;
            }
            string decision = approveRadio.IsChecked == true ? "одобрена" : "заблокирована";
            string comment = commentBox.Text.Trim();
            if (decision == "заблокирована" && string.IsNullOrEmpty(comment))
            {
                decisionError.Text = "При блокировке комментарий обязателен.";
                return;
            }
            try
            {
                var updateDto = new ProductionBatch
                {
                    ЛабораторныйСтатус = decision,
                    КомментарийРешения = comment,
                    РешениеПринял = currentUser,
                    ДатаРешения = DateTime.Now
                };
                await api.PutAsync<ApiResponse<object>>($"api/Batches/{batchId}/lab-status", updateDto);
                MessageBox.Show($"Партия {decision}.");
                DialogResult = true;
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }
    }
}