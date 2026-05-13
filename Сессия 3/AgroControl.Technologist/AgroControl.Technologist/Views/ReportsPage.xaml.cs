using AgroControl.Technologist.Services;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
            //ExcelPackage.License.SetNonCommercialLicense();   // ← исправлено
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

        private async Task<DataTable> FetchReportData(string endpoint)
        {
            try
            {
                string json = await api.GetStringAsync(endpoint);
                var dt = new DataTable();

                try
                {
                    var array = JArray.Parse(json);
                    if (array.Count > 0)
                    {
                        var first = array[0] as JObject;
                        if (first != null)
                        {
                            foreach (var prop in first.Properties())
                                dt.Columns.Add(prop.Name);
                        }
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

            var dlg = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files|*.*",
                FileName = "report.csv"
            };

            if (dlg.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine(string.Join(",", currentData.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
                foreach (DataRow row in currentData.Rows)
                {
                    var values = row.ItemArray.Select(f => $"\"{f?.ToString()?.Replace("\"", "\"\"")}\"");
                    sb.AppendLine(string.Join(",", values));
                }
                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("CSV файл сохранён");
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (currentData == null || currentData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта");
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files|*.*",
                FileName = "report.xlsx"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Report");
                        for (int i = 0; i < currentData.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = currentData.Columns[i].ColumnName;
                            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        }
                        for (int row = 0; row < currentData.Rows.Count; row++)
                        {
                            for (int col = 0; col < currentData.Columns.Count; col++)
                            {
                                worksheet.Cells[row + 2, col + 1].Value = currentData.Rows[row][col]?.ToString();
                            }
                        }
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                        package.SaveAs(new FileInfo(dlg.FileName));
                    }
                    MessageBox.Show("Excel файл сохранён");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}");
                }
            }
        }

        private void ClearTable_Click(object sender, RoutedEventArgs e)
        {
            reportGrid.ItemsSource = null;
            currentData = null;
        }
    }
}