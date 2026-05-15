using System;
using System.Linq;
using System.Windows;
using AgroControl.API.Models;
using AgroControl.Operator.Services;

namespace AgroControl.Operator.Views
{
    public partial class CreateBatchWindow : Window
    {
        private readonly ApiService _api;
        public ProductionBatch? CreatedBatch { get; private set; }

        public CreateBatchWindow(ApiService api)
        {
            InitializeComponent();
            _api = api;
            Loaded += async (s, e) => await LoadOrders();
        }

        private async System.Threading.Tasks.Task LoadOrders()
        {
            var orders = await _api.GetProductionOrdersAsync();
            if (orders != null)
            {
                cmbOrder.ItemsSource = orders;
                if (orders.Any()) cmbOrder.SelectedIndex = 0;
            }
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBatchNumber.Text))
            {
                MessageBox.Show("Введите номер партии", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbOrder.SelectedItem is not ProductionOrder selectedOrder)
            {
                MessageBox.Show("Выберите заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newBatch = new ProductionBatch
            {
                НомерПартии = txtBatchNumber.Text.Trim(),
                ЗаказID = selectedOrder.ID,
                Статус = "запланирована"
            };

            var created = await _api.CreateBatchAsync(newBatch);
            if (created != null)
            {
                CreatedBatch = created;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при создании партии", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}