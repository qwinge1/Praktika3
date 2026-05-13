using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using AgroControl.Technologist.Services;

namespace AgroControl.Technologist.Views
{
    public partial class ReportsPage : UserControl
    {
        private readonly ApiService api;
        private DataTable currentData = null;

        public ReportsPage(ApiService api)
        {
            InitializeComponent();
            this.api = api;
        }

        private async void BatchesReport_Click(object sender, RoutedEventArgs e)
        {
            var dt = await FetchReportData("api/Reports/batches?startDate=2025-03-01&endDate=2025-03-31");
            DisplayData(dt);
        }

        private async void DeviationsReport_Click(object sender, RoutedEventArgs e)
        {
            var dt = await FetchReportData("api/Reports/deviations?startDate=2025-03-01&endDate=2025-03-31");
            DisplayData(dt);
        }

        private async void RecipeUsage_Click(object sender, RoutedEventArgs e)
        {
            var dt = await FetchReportData("api/Reports/recipe-usage?startDate=2025-03-01&endDate=2025-03-31");
            DisplayData(dt);
        }

        /// <summary>
        /// Запрашивает отчёт у API и возвращает DataTable.
        /// Если API возвращает массив – парсим его. Если строку – создаём таблицу с одним сообщением.
        /// </summary>
        private async Task<DataTable> FetchReportData(string endpoint)
        {
            try
            {
                // Используем новый метод GetStringAsync из ApiService
                string json = await api.GetStringAsync(endpoint);
                var dt = new DataTable();

                // Пытаемся обработать JSON как массив объектов
                try
                {
                    var array = JArray.Parse(json);
                    if (array.Count > 0)
                    {
                        // Строим столбцы по первому объекту
                        var first = array[0] as JObject;
                        if (first != null)
                        {
                            foreach (var prop in first.Properties())
                                dt.Columns.Add(prop.Name);
                        }
                        // Заполняем строки
                        foreach (JObject obj in array)
                        {
                            var row = dt.NewRow();
                            foreach (DataColumn col in dt.Columns)
                                row[col.ColumnName] = obj[col.ColumnName]?.ToString();
                            dt.Rows.Add(row);
                        }
                    }
                    return dt;
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    // Если ответ не является JSON-массивом (например, простая строка с сообщением)
                    dt.Columns.Add("Сообщение");
                    dt.Rows.Add(json);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении отчёта: {ex.Message}");
                return null;
            }
        }

        private void DisplayData(DataTable dt)
        {
            currentData = dt;
            reportGrid.ItemsSource = dt?.DefaultView;
        }

        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            if (currentData == null || currentData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта");
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files|*.*",
                FileName = "report.csv"
            };

            if (dlg.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                // Заголовки
                sb.AppendLine(string.Join(",", currentData.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
                // Данные
                foreach (DataRow row in currentData.Rows)
                {
                    var values = row.ItemArray.Select(f =>
                        $"\"{f?.ToString()?.Replace("\"", "\"\"")}\"");
                    sb.AppendLine(string.Join(",", values));
                }

                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Файл сохранён");
            }
        }

        private void ClearTable_Click(object sender, RoutedEventArgs e)
        {
            reportGrid.ItemsSource = null;
            currentData = null;
        }
    }
}