using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Laboratory.Views
{
    public partial class LabTestEditWindow : Window
    {
        private ApiService api;
        private RawMaterialBatch rawBatch;
        private ProductionBatch prodBatch;
        private LabTest existingTest;

        // Конструктор для сырья
        public LabTestEditWindow(ApiService api, RawMaterialBatch batch)
        {
            InitializeComponent();
            this.api = api;
            this.rawBatch = batch;
        }

        // Конструктор для готовой продукции
        public LabTestEditWindow(ApiService api, ProductionBatch batch)
        {
            InitializeComponent();
            this.api = api;
            this.prodBatch = batch;
        }

        // Конструктор для редактирования существующего испытания
        public LabTestEditWindow(ApiService api, LabTest test)
        {
            InitializeComponent();
            this.api = api;
            this.existingTest = test;
            LoadTestData();
        }

        private void LoadTestData()
        {
            typeCombo.Text = existingTest.ТипОбразца;
            paramBox.Text = existingTest.НаименованиеПараметра;
            normBox.Text = existingTest.НормативноеЗначение;
            measuredBox.Text = existingTest.ИзмеренноеЗначение;
            unitBox.Text = existingTest.ЕдиницаИзмерения;
            commentBox.Text = existingTest.КомментарийАналитика;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var test = new LabTest
            {
                ПартияСырьяID = rawBatch?.ID ?? existingTest?.ПартияСырьяID,
                ПартияПроизводстваID = prodBatch?.ID ?? existingTest?.ПартияПроизводстваID,
                ТипОбразца = (typeCombo.SelectedItem as ComboBoxItem)?.Content.ToString(),
                НаименованиеПараметра = paramBox.Text,
                НормативноеЗначение = normBox.Text,
                ИзмеренноеЗначение = measuredBox.Text,
                ЕдиницаИзмерения = unitBox.Text,
                КомментарийАналитика = commentBox.Text,
                ДатаАнализа = DateTime.Now,
                Результат = CompareWithNorm() ? "pass" : "fail"
            };

            try
            {
                if (existingTest == null)
                {
                    await api.PostAsync<ApiResponse<LabTest>>("api/QualityControl", test);
                    MessageBox.Show("Испытание создано");
                }
                else
                {
                    test.ID = existingTest.ID;
                    await api.PutAsync<ApiResponse<LabTest>>($"api/QualityControl/{existingTest.ID}", test);
                    MessageBox.Show("Испытание обновлено");
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private bool CompareWithNorm()
        {
            if (decimal.TryParse(measuredBox.Text, out var measured) &&
                decimal.TryParse(normBox.Text, out var norm))
            {
                return Math.Abs(measured - norm) <= norm * 0.05m;
            }
            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}