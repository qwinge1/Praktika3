using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AgroControl.API.Models;
using AgroControl.Technologist.Services;

namespace AgroControl.Technologist.Views
{
    public partial class DeviationsPage : UserControl
    {
        private readonly ApiService api;
        private List<dynamic> allDeviations = new();

        public DeviationsPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadDeviations();
        }

        private async Task LoadDeviations()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<ProductionBatch>>>("api/Batches");
                var batches = response?.Data ?? new List<ProductionBatch>();
                var deviations = batches
                    .SelectMany(b => b.ВыполнениеШагов?.Where(s => s.Отклонение).Select(s => new
                    {
                        НомерПартии = b.НомерПартии,
                        Шаг = s.КомментарийОператора ?? "—",
                        Температура = s.ФактТемпература,
                        Давление = s.ФактДавление,
                        Длительность = s.ФактДлительностьМинут
                    }) ?? Enumerable.Empty<object>())
                    .ToList();
                allDeviations = deviations.Cast<dynamic>().ToList();
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = allDeviations;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadDeviations();

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.ToLower();
            dataGrid.ItemsSource = string.IsNullOrWhiteSpace(term) ? allDeviations :
                allDeviations.Where(d => (d.НомерПартии?.ToLower().Contains(term) ?? false) ||
                                         (d.Шаг?.ToLower().Contains(term) ?? false)).ToList();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            dataGrid.ItemsSource = allDeviations;
        }
    }
}