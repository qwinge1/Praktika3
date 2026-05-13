using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Laboratory.Views
{
    public partial class RawMaterialBatchesPage : UserControl
    {
        private ApiService api;
        private List<RawMaterialBatch> allBatches = new();

        public RawMaterialBatchesPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadBatches();
        }

        private async Task LoadBatches()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<RawMaterialBatch>>>("api/RawMaterialBatches");
                allBatches = response.Data ?? new List<RawMaterialBatch>();
                batchesGrid.ItemsSource = null;
                batchesGrid.ItemsSource = allBatches;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadBatches();

        private async void CreateTest_Click(object sender, RoutedEventArgs e)
        {
            if (batchesGrid.SelectedItem is RawMaterialBatch selected)
            {
                var existingTests = await api.GetAsync<ApiResponse<List<LabTest>>>($"api/QualityControl?batchId={selected.ID}");
                if (existingTests.Data?.Any(t => string.IsNullOrEmpty(t.Результат)) == true)
                {
                    MessageBox.Show("Для этой партии уже есть незавершённое испытание.");
                    return;
                }
                var dialog = new LabTestEditWindow(api, selected);
                if (dialog.ShowDialog() == true) await LoadBatches();
            }
            else MessageBox.Show("Выберите партию сырья");
        }

        private void BatchesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (batchesGrid.SelectedItem is RawMaterialBatch selected)
            {
                var cardWindow = new RawMaterialBatchCard(api, selected.ID);
                cardWindow.Owner = Window.GetWindow(this);
                cardWindow.ShowDialog();
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.ToLower();
            batchesGrid.ItemsSource = string.IsNullOrWhiteSpace(term) ? allBatches :
                allBatches.Where(b => b.НомерПартии.ToLower().Contains(term) ||
                                      (b.Поставщик?.ToLower().Contains(term) ?? false)).ToList();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            batchesGrid.ItemsSource = allBatches;
        }
    }
}