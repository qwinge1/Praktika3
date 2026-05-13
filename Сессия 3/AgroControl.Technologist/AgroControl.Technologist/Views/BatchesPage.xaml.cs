using AgroControl.API.Models;
using AgroControl.Technologist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Technologist.Views
{
    public partial class BatchesPage : UserControl
    {
        private readonly ApiService api;
        private List<ProductionBatch> allBatches = new();

        public BatchesPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadBatches();
        }

        private async Task LoadBatches()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<ProductionBatch>>>("api/Batches");
                allBatches = response.Data;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = allBatches;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadBatches();

        private async void StartBatch_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is ProductionBatch selected)
            {
                try
                {
                    await api.PostAsync<object>($"api/Batches/{selected.ID}/start", null);
                    MessageBox.Show("Партия запущена");
                    await LoadBatches();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
            else MessageBox.Show("Выберите партию");
        }

        private async void StartStep_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is ProductionBatch batch)
            {
                var step = batch.ВыполнениеШагов?.FirstOrDefault();
                if (step != null)
                {
                    try
                    {
                        await api.PostAsync<object>($"api/Batches/steps/{step.ID}/start", null);
                        MessageBox.Show("Шаг начат");
                        await LoadBatches();
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
                }
                else MessageBox.Show("У партии нет шагов");
            }
            else MessageBox.Show("Выберите партию");
        }

        private async void CompleteStep_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is ProductionBatch batch)
            {
                var step = batch.ВыполнениеШагов?.FirstOrDefault();
                if (step != null)
                {
                    try
                    {
                        await api.PostAsync<object>($"api/Batches/steps/{step.ID}/complete", null);
                        MessageBox.Show("Шаг завершён");
                        await LoadBatches();
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
                }
                else MessageBox.Show("У партии нет шагов");
            }
            else MessageBox.Show("Выберите партию");
        }
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new BatchEditDialog(api);
            if (dialog.ShowDialog() == true)
            {
                string batchNumber = $"B-{DateTime.Now:yyyyMMddHHmmss}";
                var newBatch = new ProductionBatch
                {
                    НомерПартии = batchNumber,
                    ЗаказID = dialog.OrderId,
                    Статус = dialog.Status
                    // ПРИМЕЧАНИЕ: Поле ПланДатаСтарта НЕ передаём, так как в модели может отсутствовать
                };
                try
                {
                    await api.PostAsync<ApiResponse<ProductionBatch>>("api/Batches", newBatch);
                    MessageBox.Show("Партия создана");
                    await LoadBatches();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка создания: " + ex.Message);
                }
            }
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is ProductionBatch selected)
            {
                if (MessageBox.Show($"Удалить партию {selected.НомерПартии}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                try
                {
                    await api.DeleteAsync($"api/Batches/{selected.ID}");
                    MessageBox.Show("Партия удалена");
                    await LoadBatches(); // обновить список
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
            else MessageBox.Show("Выберите партию");
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.ToLower();
            dataGrid.ItemsSource = string.IsNullOrWhiteSpace(term) ? allBatches :
                allBatches.Where(b => (b.НомерПартии?.ToLower().Contains(term) ?? false) ||
                                      (b.Статус?.ToLower().Contains(term) ?? false)).ToList();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            dataGrid.ItemsSource = allBatches;
        }
    }
}