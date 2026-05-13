using System;
using System.Collections.Generic;
using System.Windows;
using AgroControl.API.Models;
using AgroControl.Technologist.Services;

namespace AgroControl.Technologist.Views
{
    public partial class ComponentsWindow : Window
    {
        private readonly ApiService api;
        private readonly int recipeId;

        public ComponentsWindow(int recipeId, ApiService api)
        {
            InitializeComponent();
            this.recipeId = recipeId;
            this.api = api;
            Loaded += async (s, e) => await LoadComponents();
        }

        private async Task LoadComponents()
        {
            try
            {
                // Загружаем список материалов для выбора
                var materialsResponse = await api.GetAsync<ApiResponse<List<RawMaterial>>>("api/Materials");
                materialCombo.ItemsSource = materialsResponse.Data;

                // Загружаем текущие компоненты рецепта
                var recipeResponse = await api.GetAsync<ApiResponse<Recipe>>($"api/Recipes/{recipeId}");
                componentsGrid.ItemsSource = recipeResponse.Data.Состав;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void AddComponent_Click(object sender, RoutedEventArgs e)
        {
            if (materialCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите материал");
                return;
            }

            var material = (RawMaterial)materialCombo.SelectedItem;
            if (!decimal.TryParse(percentageBox.Text, out var percent) || !int.TryParse(orderBox.Text, out var order))
            {
                MessageBox.Show("Введите корректные долю и порядок");
                return;
            }

            try
            {
                var component = new RecipeComponent
                {
                    СырьеID = material.ID,
                    Процент = percent,
                    ПорядокЗагрузки = order
                };
                var result = await api.PostAsync<ApiResponse<RecipeComponent>>($"api/Recipes/{recipeId}/components", component);
                if (result.Success)
                {
                    MessageBox.Show("Компонент добавлен");
                    await LoadComponents();
                }
                else MessageBox.Show(result.Message ?? "Ошибка добавления компонента");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}