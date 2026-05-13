using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AgroControl.API.Models;
using AgroControl.Technologist.Services;

namespace AgroControl.Technologist.Views
{
    public partial class OrderEditDialog : Window
    {
        private readonly ApiService api;

        public string OrderNumber => orderNumberBox.Text.Trim();
        public int ProductId => (int?)productCombo.SelectedValue ?? 0;
        public int RecipeId => (int?)recipeCombo.SelectedValue ?? 0;
        public int TechCardId => (int?)techCardCombo.SelectedValue ?? 0;
        public decimal Quantity => decimal.TryParse(quantityBox.Text, out var q) ? q : 0;
        public string Status => ((ComboBoxItem)statusCombo.SelectedItem)?.Content?.ToString() ?? "запланирован";
        public DateTime? PlannedDate => plannedDatePicker.SelectedDate;

        public OrderEditDialog(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadDictionaries();
        }

        public void SetOrderData(ProductionOrder order)
        {
            orderNumberBox.Text = order.НомерЗаказа;
            productCombo.SelectedValue = order.ПродуктID;
            recipeCombo.SelectedValue = order.РецептID;
            techCardCombo.SelectedValue = order.ТехКартаID;
            quantityBox.Text = order.ПланКоличество_кг.ToString();
            foreach (ComboBoxItem item in statusCombo.Items)
                if (item.Content.ToString() == order.Статус) { statusCombo.SelectedItem = item; break; }
            plannedDatePicker.SelectedDate = order.ПланДатаСтарта;
        }

        private async Task LoadDictionaries()
        {
            var productsResp = await api.GetAsync<ApiResponse<List<Product>>>("api/Products");
            productCombo.ItemsSource = productsResp?.Data;

            var recipesResp = await api.GetAsync<ApiResponse<List<Recipe>>>("api/Recipes");
            recipeCombo.ItemsSource = recipesResp?.Data;

            var techCardsResp = await api.GetAsync<ApiResponse<List<TechCard>>>("api/TechCards");
            techCardCombo.ItemsSource = techCardsResp?.Data;
        }

        private void Save_Click(object sender, RoutedEventArgs e) { DialogResult = true; Close(); }
        private void Cancel_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }
    }
}