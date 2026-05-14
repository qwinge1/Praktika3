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
        private string currentUser;

        public ProductBatchesPage(ApiService api, string user)
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
                var response = await api.GetAsync<ApiResponse<List<ProductionBatch>>>("api/Batches");
                allBatches = response.Data ?? new List<ProductionBatch>();
                batchesGrid.ItemsSource = allBatches;
                ApplyFilters();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadBatches();

        private void BatchesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (batchesGrid.SelectedItem is ProductionBatch selected)
            {
                var card = new ProductBatchCard(api, selected.ID, currentUser);
                card.Owner = Window.GetWindow(this);
                card.ShowDialog();
                _ = LoadBatches(); // обновить после закрытия
            }
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e) => ApplyFilters();
        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            statusFilter.SelectedIndex = 0;
            labStatusFilter.SelectedIndex = 0;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = allBatches.AsEnumerable();
            var search = searchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => b.НомерПартии.ToLower().Contains(search));

            var status = (statusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (status != "Все" && !string.IsNullOrEmpty(status))
                query = query.Where(b => b.Статус == status);

            var labStatus = (labStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (labStatus != "Все" && !string.IsNullOrEmpty(labStatus))
                query = query.Where(b => b.ЛабораторныйСтатус == labStatus);

            batchesGrid.ItemsSource = query.ToList();
        }
    }
}