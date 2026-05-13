using AgroControl.API.Models;
using AgroControl.Technologist.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Technologist.Views
{
    public partial class RecipeEditDialog : Window
    {
        private readonly ApiService api;

        public int ProductId => (int?)productCombo.SelectedValue ?? 0;
        public int Version => int.TryParse(versionBox.Text, out var v) ? v : 0;
        public string Status => ((ComboBoxItem)statusCombo.SelectedItem)?.Content?.ToString() ?? "черновик";

        public RecipeEditDialog(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadProducts();
        }

        public void SetProductAndStatus(int productId, string status)
        {
            if (productCombo.ItemsSource == null) return;
            productCombo.SelectedValue = productId;
            foreach (ComboBoxItem item in statusCombo.Items)
                if (item.Content.ToString() == status) { statusCombo.SelectedItem = item; break; }
        }

        public void SetVersion(int version)
        {
            versionBox.Text = version.ToString();
        }

        private async Task LoadProducts()
        {
            var response = await api.GetAsync<ApiResponse<List<Product>>>("api/Products");
            productCombo.ItemsSource = response?.Data;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ProductId == 0)
            {
                MessageBox.Show("Выберите продукт");
                return;
            }
            if (Version <= 0)
            {
                MessageBox.Show("Введите корректную версию");
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