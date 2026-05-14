using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Windows;

namespace AgroControl.Laboratory.Views
{
    public partial class RawMaterialBatchEditDialog : Window
    {
        private ApiService api;
        private RawMaterialBatch batch;

        public RawMaterialBatchEditDialog(ApiService api, RawMaterialBatch batch)
        {
            InitializeComponent();
            this.api = api;
            this.batch = batch;
            LoadData();
        }

        private void LoadData()
        {
            batchNumberBox.Text = batch.НомерПартии;
            supplierBox.Text = batch.Поставщик;
            datePicker.SelectedDate = batch.ДатаПоступления;
            quantityBox.Text = batch.Количество_кг?.ToString();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            batch.НомерПартии = batchNumberBox.Text.Trim();
            batch.Поставщик = supplierBox.Text.Trim();
            batch.ДатаПоступления = datePicker.SelectedDate;
            batch.Количество_кг = decimal.TryParse(quantityBox.Text, out var q) ? q : (decimal?)null;

            try
            {
                await api.PutAsync<ApiResponse<object>>($"api/RawMaterialBatches/{batch.ID}", batch);
                MessageBox.Show("Партия обновлена");
                DialogResult = true;
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }
    }
}