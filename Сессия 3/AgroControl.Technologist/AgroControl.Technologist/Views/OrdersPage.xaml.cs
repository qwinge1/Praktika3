using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AgroControl.API.Models;
using AgroControl.Technologist.Services;

namespace AgroControl.Technologist.Views
{
    public partial class OrdersPage : UserControl
    {
        private readonly ApiService api;
        private List<ProductionOrder> allOrders = new();

        public OrdersPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadOrders();
        }

        private async Task LoadOrders()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<ProductionOrder>>>("api/ProductionOrders");
                allOrders = response.Data;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = allOrders;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadOrders();

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OrderEditDialog(api);
            if (dialog.ShowDialog() == true)
            {
                var order = new ProductionOrder
                {
                    НомерЗаказа = dialog.OrderNumber,
                    ПродуктID = dialog.ProductId,
                    РецептID = dialog.RecipeId,
                    ТехКартаID = dialog.TechCardId,
                    ПланКоличество_кг = dialog.Quantity,
                    Статус = dialog.Status,
                    ПланДатаСтарта = dialog.PlannedDate
                };
                try
                {
                    await api.PostAsync<ApiResponse<ProductionOrder>>("api/ProductionOrders", order);
                    MessageBox.Show("Заказ создан");
                    await LoadOrders();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка создания: " + ex.Message); }
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is ProductionOrder selected)
            {
                var dialog = new OrderEditDialog(api);
                dialog.SetOrderData(selected);
                if (dialog.ShowDialog() == true)
                {
                    selected.НомерЗаказа = dialog.OrderNumber;
                    selected.ПродуктID = dialog.ProductId;
                    selected.РецептID = dialog.RecipeId;
                    selected.ТехКартаID = dialog.TechCardId;
                    selected.ПланКоличество_кг = dialog.Quantity;
                    selected.Статус = dialog.Status;
                    selected.ПланДатаСтарта = dialog.PlannedDate;
                    try
                    {
                        await api.PutAsync<ApiResponse<ProductionOrder>>($"api/ProductionOrders/{selected.ID}", selected);
                        MessageBox.Show("Заказ обновлён");
                        await LoadOrders();
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
                }
            }
            else MessageBox.Show("Выберите заказ");
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is ProductionOrder selected)
            {
                try
                {
                    await api.DeleteAsync($"api/ProductionOrders/{selected.ID}");
                    MessageBox.Show("Заказ удалён");
                    await LoadOrders();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.ToLower();
            dataGrid.ItemsSource = string.IsNullOrWhiteSpace(term) ? allOrders :
                allOrders.Where(o => (o.НомерЗаказа?.ToLower().Contains(term) ?? false) ||
                                     (o.Статус?.ToLower().Contains(term) ?? false)).ToList();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            dataGrid.ItemsSource = allOrders;
        }
    }
}