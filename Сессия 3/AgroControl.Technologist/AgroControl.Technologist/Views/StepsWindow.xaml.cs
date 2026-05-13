using System;
using System.Windows;
using AgroControl.API.Models;
using AgroControl.Technologist.Services;

namespace AgroControl.Technologist.Views
{
    public partial class StepsWindow : Window
    {
        private readonly int techCardId;
        private readonly ApiService api;

        public StepsWindow(int techCardId, ApiService api)
        {
            InitializeComponent();
            this.techCardId = techCardId;
            this.api = api;
            Loaded += async (s, e) => await LoadSteps();
        }

        private async Task LoadSteps()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<TechCard>>($"api/TechCards/{techCardId}");
                stepsGrid.ItemsSource = response?.Data?.Шаги;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки шагов: " + ex.Message); }
        }

        private async void AddStep_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(stepNameBox.Text))
            {
                MessageBox.Show("Введите название шага");
                return;
            }

            var step = new TechCardStep
            {
                ТехКартаID = techCardId,
                НаименованиеШага = stepNameBox.Text,
                ПланТемпература = decimal.TryParse(tempBox.Text, out var t) ? t : null,
                ПланДлительностьМинут = int.TryParse(durationBox.Text, out var d) ? d : null,
                ПланДавление = decimal.TryParse(pressureBox.Text, out var p) ? p : null,
                Обязательный = mandatoryCheck.IsChecked == true,
                НомерШага = 0
            };

            try
            {
                var result = await api.PostAsync<ApiResponse<TechCardStep>>($"api/TechCards/{techCardId}/steps", step);
                if (result.Success)
                {
                    MessageBox.Show("Шаг добавлен");
                    await LoadSteps();
                }
                else MessageBox.Show(result.Message ?? "Ошибка добавления шага");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }
    }
}