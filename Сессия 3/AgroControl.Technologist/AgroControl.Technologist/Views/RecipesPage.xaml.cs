using AgroControl.API.Models;
using AgroControl.Technologist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Technologist.Views
{
    public partial class RecipesPage : UserControl
    {
        private readonly ApiService api;
        private List<Recipe> allRecipes = new();

        public RecipesPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadRecipes();
        }

        private async Task LoadRecipes()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<Recipe>>>("api/Recipes");
                allRecipes = response?.Data ?? new List<Recipe>();
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = allRecipes;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadRecipes();

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RecipeEditDialog(api);
            if (dialog.ShowDialog() == true)
            {
                // Если пользователь ввел версию, используем её, иначе вычисляем следующую
                int version = dialog.Version > 0
                    ? dialog.Version
                    : (allRecipes.Any() ? allRecipes.Max(r => r.Версия) + 1 : 1);

                var recipe = new Recipe
                {
                    ПродуктID = dialog.ProductId,
                    Версия = version,
                    Статус = dialog.Status,
                    ДатаСоздания = DateTime.Now
                };
                try
                {
                    await api.PostAsync<ApiResponse<Recipe>>("api/Recipes", recipe);
                    MessageBox.Show("Рецепт создан");
                    await LoadRecipes();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка создания: " + ex.Message); }
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Recipe selected)
            {
                var dialog = new RecipeEditDialog(api);
                dialog.SetProductAndStatus(selected.ПродуктID, selected.Статус);
                dialog.SetVersion(selected.Версия);  // новый метод
                if (dialog.ShowDialog() == true)
                {
                    selected.ПродуктID = dialog.ProductId;
                    selected.Статус = dialog.Status;
                    selected.Версия = dialog.Version;  // обновляем версию
                    try
                    {
                        await api.PutAsync<ApiResponse<Recipe>>($"api/Recipes/{selected.ID}", selected);
                        MessageBox.Show("Рецепт обновлён");
                        await LoadRecipes();
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
                }
            }
            else MessageBox.Show("Выберите рецепт");
        }

        private async void Archive_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Recipe selected)
            {
                try
                {
                    await api.PutAsync<ApiResponse<object>>($"api/Recipes/{selected.ID}/status", "архив");
                    MessageBox.Show("Рецепт архивирован");
                    await LoadRecipes();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Recipe selected)
            {
                if (MessageBox.Show($"Удалить рецепт {selected.Версия}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                try
                {
                    await api.DeleteAsync($"api/Recipes/{selected.ID}");
                    MessageBox.Show("Рецепт удалён");
                    await LoadRecipes();
                }
                catch (HttpRequestException ex)
                {
                    if (ex.Message.Contains("405"))
                        MessageBox.Show("Метод DELETE не поддерживается API. Добавьте [HttpDelete] в контроллер.");
                    else
                        MessageBox.Show($"Ошибка: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
            else MessageBox.Show("Выберите рецепт");
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.ToLower();
            dataGrid.ItemsSource = string.IsNullOrWhiteSpace(term) ? allRecipes :
                allRecipes.Where(r => r.ПродуктID.ToString().Contains(term) ||
                                      r.Версия.ToString().Contains(term) ||
                                      (r.Статус?.ToLower().Contains(term) ?? false)).ToList();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            dataGrid.ItemsSource = allRecipes;
        }

        private void ShowComponents_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Recipe selected)
            {
                var window = new ComponentsWindow(selected.ID, api);
                window.Owner = Window.GetWindow(this);
                window.ShowDialog();
            }
        }
    }
}