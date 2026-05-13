using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AgroControl.API.Models;
using AgroControl.Technologist.Services;

namespace AgroControl.Technologist.Views
{
    public partial class DashboardPage : UserControl
    {
        private readonly ApiService api;

        public DashboardPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadDashboard();
        }

        private async Task LoadDashboard()
        {
            try
            {
                var productsTask = api.GetAsync<ApiResponse<List<Product>>>("api/Products");
                var recipesTask = api.GetAsync<ApiResponse<List<Recipe>>>("api/Recipes");
                var batchesTask = api.GetAsync<ApiResponse<List<ProductionBatch>>>("api/Batches");

                await Task.WhenAll(productsTask, recipesTask, batchesTask);

                var products = productsTask.Result?.Data ?? new List<Product>();
                var recipes = recipesTask.Result?.Data ?? new List<Recipe>();
                var batches = batchesTask.Result?.Data ?? new List<ProductionBatch>();

                activeProductsText.Text = products.Count(p => p.Статус == "активен").ToString();
                activeRecipesText.Text = recipes.Count(r => r.Статус == "активна").ToString();
                batchesInProgressText.Text = batches.Count(b => b.Статус == "выполняется").ToString();
                batchesWithDeviationsText.Text = batches.Count(b =>
                    b.ВыполнениеШагов?.Any(s => s.Отклонение) == true).ToString();
                pendingLabText.Text = batches.Count(b => b.Статус == "завершена" && b.ВыполнениеШагов?.Any() == true).ToString();
            }
            catch { }
        }
    }
}