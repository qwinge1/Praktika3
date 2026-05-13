using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Laboratory.Views
{
    public partial class ProductBatchesPage : UserControl
    {
        private ApiService api;
        private List<ProductionBatch> allBatches = new();

        public ProductBatchesPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadBatches();
        }

        private async Task LoadBatches()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<ProductionBatch>>>("api/Batches");
                allBatches = response.Data ?? new List<ProductionBatch>();
                batchesGrid.ItemsSource = allBatches;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadBatches();

        private async void CreateTest_Click(object sender, RoutedEventArgs e)
        {
            if (batchesGrid.SelectedItem is ProductionBatch selected)
            {
                // Проверяем, есть ли незавершённое испытание
                var existingTests = await api.GetAsync<ApiResponse<List<LabTest>>>($"api/QualityControl?batchId={selected.ID}");
                if (existingTests.Data?.Any(t => string.IsNullOrEmpty(t.Результат)) == true)
                {
                    MessageBox.Show("Для этой партии уже есть незавершённое испытание.");
                    return;
                }
                var dialog = new LabTestEditWindow(api, selected); // нужно добавить конструктор для ProductionBatch
                if (dialog.ShowDialog() == true) await LoadBatches();
            }
            else MessageBox.Show("Выберите партию");
        }

        private void BatchesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (batchesGrid.SelectedItem is ProductionBatch selected)
            {
                var cardWindow = new ProductBatchCard(api, selected.ID);
                cardWindow.Owner = Window.GetWindow(this);
                cardWindow.ShowDialog();
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.ToLower();
            batchesGrid.ItemsSource = string.IsNullOrWhiteSpace(term) ? allBatches :
                allBatches.Where(b => b.НомерПартии.ToLower().Contains(term) ||
                                      (b.Статус?.ToLower().Contains(term) ?? false)).ToList();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            batchesGrid.ItemsSource = allBatches;
        }
    }
}