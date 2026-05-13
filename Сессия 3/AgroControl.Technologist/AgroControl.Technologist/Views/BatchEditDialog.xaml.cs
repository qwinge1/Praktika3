using AgroControl.API.Models;
using AgroControl.Technologist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Technologist.Views
{
    public partial class BatchEditDialog : Window
    {
        private readonly ApiService api;

        public int OrderId => (int?)orderCombo.SelectedValue ?? 0;
        public decimal Quantity => decimal.TryParse(quantityBox.Text, out var q) ? q : 0;
        public string Status => ((ComboBoxItem)statusCombo.SelectedItem)?.Content?.ToString() ?? "запланирована";
        public DateTime? PlannedDate => plannedDatePicker.SelectedDate;

        public BatchEditDialog(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadOrders();
        }

        private async Task LoadOrders()
        {
            var response = await api.GetAsync<ApiResponse<List<ProductionOrder>>>("api/ProductionOrders");
            orderCombo.ItemsSource = response.Data;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (orderCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите заказ!");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}