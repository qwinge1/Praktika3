using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Laboratory.Views
{
    public partial class ProductBatchEditDialog : Window
    {
        private ApiService api;
        private ProductionBatch batch;

        public ProductBatchEditDialog(ApiService api, ProductionBatch batch)
        {
            InitializeComponent();
            this.api = api;
            this.batch = batch;
            LoadData();
        }

        private void LoadData()
        {
            batchNumberBox.Text = batch.НомерПартии;
            foreach (ComboBoxItem item in statusCombo.Items)
            {
                if (item.Content.ToString() == batch.Статус)
                {
                    statusCombo.SelectedItem = item;
                    break;
                }
            }
            quantityBox.Text = batch.ФактКоличество_кг?.ToString();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            batch.НомерПартии = batchNumberBox.Text.Trim();
            batch.Статус = (statusCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
            batch.ФактКоличество_кг = decimal.TryParse(quantityBox.Text, out var q) ? q : (decimal?)null;

            try
            {
                await api.PutAsync<ApiResponse<object>>($"api/Batches/{batch.ID}", batch);
                MessageBox.Show("Партия обновлена");
                DialogResult = true;
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }
    }
}