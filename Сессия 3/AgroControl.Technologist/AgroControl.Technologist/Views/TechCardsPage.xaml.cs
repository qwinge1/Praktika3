using AgroControl.API.Models;
using AgroControl.Technologist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Technologist.Views
{
    public partial class TechCardsPage : UserControl
    {
        private readonly ApiService api;
        private List<TechCard> allTechCards = new();

        public TechCardsPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadTechCards();
        }

        private async Task LoadTechCards()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<TechCard>>>("api/TechCards");
                allTechCards = response?.Data ?? new List<TechCard>();
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = allTechCards;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadTechCards();

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TechCardEditDialog(api);
            if (dialog.ShowDialog() == true)
            {
                int nextVer = allTechCards.Any() ? allTechCards.Max(c => c.Версия) + 1 : 1;
                var newCard = new TechCard
                {
                    ПродуктID = dialog.ProductId,
                    Версия = nextVer,
                    Статус = dialog.Status,
                    ДатаСоздания = DateTime.Now
                };
                try
                {
                    var result = await api.PostAsync<ApiResponse<TechCard>>("api/TechCards", newCard);
                    if (result.Success)
                    {
                        MessageBox.Show("Техкарта создана");
                        await LoadTechCards();
                    }
                    else MessageBox.Show(result.Message ?? "Ошибка создания");
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is TechCard selected)
            {
                var dialog = new TechCardEditDialog(api);
                dialog.SetCardData(selected.ПродуктID, selected.Версия, selected.Статус);
                if (dialog.ShowDialog() == true)
                {
                    selected.ПродуктID = dialog.ProductId;
                    selected.Версия = dialog.Version;
                    selected.Статус = dialog.Status;
                    try
                    {
                        var result = await api.PutAsync<ApiResponse<TechCard>>($"api/TechCards/{selected.ID}", selected);
                        if (result.Success)
                        {
                            MessageBox.Show("Техкарта обновлена");
                            await LoadTechCards();
                        }
                        else MessageBox.Show(result.Message ?? "Ошибка обновления");
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
                }
            }
            else MessageBox.Show("Выберите техкарту");
        }

        private async void Archive_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is TechCard selected)
            {
                try
                {
                    var result = await api.PutAsync<ApiResponse<object>>($"api/TechCards/{selected.ID}/status", "архив");
                    if (result.Success)
                    {
                        MessageBox.Show("Техкарта архивирована");
                        await LoadTechCards();
                    }
                    else MessageBox.Show(result.Message ?? "Ошибка архивирования");
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is TechCard selected)
            {
                if (MessageBox.Show($"Удалить техкарту (версия {selected.Версия})?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                try
                {
                    await api.DeleteAsync($"api/TechCards/{selected.ID}");
                    MessageBox.Show("Техкарта удалена");
                    await LoadTechCards();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
            else MessageBox.Show("Выберите техкарту");
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.ToLower();
            dataGrid.ItemsSource = string.IsNullOrWhiteSpace(term) ? allTechCards :
                allTechCards.Where(c => c.ID.ToString().Contains(term) ||
                                        c.Версия.ToString().Contains(term) ||
                                        (c.Статус?.ToLower().Contains(term) ?? false)).ToList();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            dataGrid.ItemsSource = allTechCards;
        }

        private void ShowSteps_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is TechCard selected)
            {
                var window = new StepsWindow(selected.ID, api);
                window.Owner = Window.GetWindow(this);
                window.ShowDialog();
                _ = LoadTechCards();
            }
            else MessageBox.Show("Выберите техкарту");
        }
    }
}