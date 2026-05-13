using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AgroControl.Laboratory.Views
{
    public partial class RawMaterialBatchCard : Window
    {
        private ApiService api;
        private int batchId;
        private RawMaterialBatch batch;
        private List<LabTest> tests;

        public RawMaterialBatchCard(ApiService api, int batchId)
        {
            InitializeComponent();
            this.api = api;
            this.batchId = batchId;
            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                var batchResp = await api.GetAsync<ApiResponse<RawMaterialBatch>>($"api/RawMaterialBatches/{batchId}");
                batch = batchResp.Data;
                DataContext = batch;

                var testsResp = await api.GetAsync<ApiResponse<List<LabTest>>>($"api/QualityControl?batchId={batchId}");
                tests = testsResp.Data ?? new List<LabTest>();
                testsGrid.ItemsSource = tests;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void TestsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (testsGrid.SelectedItem is LabTest selected)
            {
                var editWindow = new LabTestEditWindow(api, selected);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true) _ = LoadData();
            }
        }

        private async void MakeDecision_Click(object sender, RoutedEventArgs e)
        {
            if (!tests.Any(t => !string.IsNullOrEmpty(t.Результат)))
            {
                decisionError.Text = "Невозможно принять решение: нет завершённых испытаний.";
                return;
            }

            string decision = approveRadio.IsChecked == true ? "одобрена" : "заблокирована";
            string comment = commentBox.Text.Trim();

            if (decision == "заблокирована" && string.IsNullOrEmpty(comment))
            {
                decisionError.Text = "При блокировке партии комментарий обязателен.";
                return;
            }

            try
            {
                var updateDto = new { ЛабораторныйСтатус = decision };
                await api.PutAsync<ApiResponse<object>>($"api/RawMaterialBatches/{batchId}", updateDto);
                MessageBox.Show($"Партия {decision}.");
                DialogResult = true;
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }
    }
}