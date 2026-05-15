using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AgroControl.API.Models;
using AgroControl.Operator.Services;

namespace AgroControl.Operator.Views
{
    public partial class ActiveBatchesPage : UserControl
    {
        private readonly ApiService _api;
        private readonly MainWindow _parent;
        private List<ActiveBatchDto> _batches = new();

        public ActiveBatchesPage(ApiService api, MainWindow parent)
        {
            InitializeComponent();
            _api = api;
            _parent = parent;
            Loaded += async (s, e) => await LoadBatches();
        }

        private async Task LoadBatches()
        {
            _batches = await _api.GetActiveBatchesAsync() ?? new List<ActiveBatchDto>();
            ApplyFilters();
        }

        public async Task RefreshBatches() => await LoadBatches();

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadBatches();

        private void ApplyFilters_Click(object sender, RoutedEventArgs e) => ApplyFilters();
        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cmbLine.SelectedIndex = 0;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = _batches.AsEnumerable();
            var search = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => b.НомерПартии.ToLower().Contains(search) || b.Продукт.ToLower().Contains(search));
            var line = (cmbLine.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (line != "Все" && !string.IsNullOrEmpty(line))
                query = query.Where(b => b.Линия == line);
            dgBatches.ItemsSource = query.ToList();
        }

        private void Batch_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgBatches.SelectedItem is ActiveBatchDto batch)
                _parent.SetCurrentBatch(batch.ID);
        }
    }
}