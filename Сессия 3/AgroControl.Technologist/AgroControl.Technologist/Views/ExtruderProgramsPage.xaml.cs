using AgroControl.API.Models;
using AgroControl.Technologist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Technologist.Views
{
    public partial class ExtruderProgramsPage : UserControl
    {
        private readonly ApiService api;

        public ExtruderProgramsPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
            Loaded += async (s, e) => await LoadPrograms();
        }

        private async Task LoadPrograms()
        {
            try
            {
                var response = await api.GetAsync<ApiResponse<List<TechCard>>>("api/TechCards");
                var cards = response?.Data ?? new List<TechCard>();
                var extruderCards = cards.Where(c => c.Шаги?.Any(s => s.НаименованиеШага?.Contains("Экструзия") ?? false) == true).ToList();
                dataGrid.ItemsSource = extruderCards;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }
    }
}