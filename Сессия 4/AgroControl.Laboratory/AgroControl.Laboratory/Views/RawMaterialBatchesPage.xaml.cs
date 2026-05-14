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
        private string currentUser;

        public RawMaterialBatchesPage(ApiService api, string user)
        {
            InitializeComponent();
            this.api = api;
            currentUser = user;
            Loaded += async (s, e) => await LoadBatches();
        }

        private async Task LoadBatches()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<RawMaterialBatch>>>("api/RawMaterialBatches");
                allBatches = response.Data ?? new List<RawMaterialBatch>();
                foreach (var batch in allBatches)
                {
                    try
                    {
                        var tests = await api.GetAsync<ApiResponse<List<LabTest>>>($"api/QualityControl?batchId={batch.ID}");
                        batch.HasTest = tests.Data?.Any() == true;
                        batch.LastTestDate = tests.Data?.OrderByDescending(t => t.ДатаАнализа).FirstOrDefault()?.ДатаАнализа;
                    }
                    catch { /* игнорируем ошибки отдельных партий */ }
                }
                LoadSuppliers();
                ApplyFilters();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void LoadSuppliers()
        {
            var suppliers = allBatches.Select(b => b.Поставщик).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
            var list = new List<string> { "Все" };
            list.AddRange(suppliers);
            supplierFilter.ItemsSource = list;
            supplierFilter.SelectedIndex = 0;
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadBatches();

        private void BatchesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (batchesGrid.SelectedItem is RawMaterialBatch selected)
            {
                var card = new RawMaterialBatchCard(api, selected.ID, currentUser);
                card.Owner = Window.GetWindow(this);
                card.ShowDialog();
                _ = LoadBatches();
            }
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e) => ApplyFilters();
        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            statusFilter.SelectedIndex = 0;
            supplierFilter.SelectedIndex = 0;
            dateFrom.SelectedDate = null;
            dateTo.SelectedDate = null;
            hasTestFilter.IsChecked = false;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = allBatches.AsEnumerable();
            var search = searchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => b.НомерПартии.ToLower().Contains(search) ||
                                         (b.Сырье?.Наименование?.ToLower().Contains(search) ?? false));
            var status = (statusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (status != "Все" && !string.IsNullOrEmpty(status))
                query = query.Where(b => b.ЛабораторныйСтатус == status);
            var supplier = supplierFilter.SelectedItem as string;
            if (supplier != "Все" && !string.IsNullOrEmpty(supplier))
                query = query.Where(b => b.Поставщик == supplier);
            if (dateFrom.SelectedDate.HasValue)
                query = query.Where(b => b.ДатаПоступления >= dateFrom.SelectedDate.Value);
            if (dateTo.SelectedDate.HasValue)
                query = query.Where(b => b.ДатаПоступления <= dateTo.SelectedDate.Value);
            if (hasTestFilter.IsChecked == true)
                query = query.Where(b => b.HasTest);
            batchesGrid.ItemsSource = query.ToList();
        }
    }
}