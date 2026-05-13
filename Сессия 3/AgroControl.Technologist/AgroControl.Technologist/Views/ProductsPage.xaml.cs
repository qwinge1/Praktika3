using AgroControl.API.Models;
using AgroControl.Technologist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Technologist.Views
{
    public partial class ProductsPage : UserControl
    {
        private readonly ApiService api;
        private List<Product> allProducts = new();

        public ProductsPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<Product>>>("api/Products");
                allProducts = response.Data;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = allProducts;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadProducts();
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductEditDialog();
            if (dialog.ShowDialog() != true) return;
            var newProduct = new Product
            {
                Код = dialog.Code,
                Наименование = dialog.Name,
                Тип = dialog.ProductType,
                ФормаВыпуска = dialog.Form,
                Статус = dialog.Status
            };
            try
            {
                await api.PostAsync<ApiResponse<Product>>("api/Products", newProduct);
                MessageBox.Show("Продукт создан");
                await LoadProducts();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Product selected)
            {
                var dialog = new ProductEditDialog(selected.Код, selected.Наименование, selected.Тип, selected.ФормаВыпуска, selected.Статус);
                if (dialog.ShowDialog() == true)
                {
                    selected.Код = dialog.Code; selected.Наименование = dialog.Name;
                    selected.Тип = dialog.ProductType; selected.ФормаВыпуска = dialog.Form;
                    selected.Статус = dialog.Status;
                    try
                    {
                        await api.PutAsync<ApiResponse<Product>>($"api/Products/{selected.ID}", selected);
                        MessageBox.Show("Продукт обновлён");
                        await LoadProducts();
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
                }
            }
        }

        private async void Archive_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Product selected)
            {
                try
                {
                    await api.PutAsync<ApiResponse<object>>($"api/Products/{selected.ID}/archive", new { });
                    MessageBox.Show("Продукт архивирован");
                    await LoadProducts();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Product selected)
            {
                try
                {
                    await api.DeleteAsync($"api/Products/{selected.ID}");
                    MessageBox.Show("Продукт удалён");
                    await LoadProducts();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var term = searchBox.Text.ToLower();
            dataGrid.ItemsSource = string.IsNullOrWhiteSpace(term) ? allProducts :
                allProducts.Where(p => (p.Код?.ToLower().Contains(term) ?? false) ||
                                       (p.Наименование?.ToLower().Contains(term) ?? false) ||
                                       (p.Тип?.ToLower().Contains(term) ?? false) ||
                                       (p.Статус?.ToLower().Contains(term) ?? false)).ToList();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            dataGrid.ItemsSource = allProducts;
        }
    }
}