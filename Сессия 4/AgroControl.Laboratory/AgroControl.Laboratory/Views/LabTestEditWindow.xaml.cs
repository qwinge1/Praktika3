using AgroControl.API.Models;
using AgroControl.Laboratory.Services;
using System;
using System.Linq;
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
        private string currentUser;

        // Конструктор для нового испытания без привязки к партии (из страницы "Испытания")
        public LabTestEditWindow(ApiService api, string user)
        {
            InitializeComponent();
            this.api = api;
            currentUser = user;
            datePicker.SelectedDate = DateTime.Now;
            batchSelectionPanel.Visibility = Visibility.Visible;
            Loaded += async (s, e) => await LoadExecutorsAndBatches();
        }

        // Конструктор для нового испытания для конкретной партии сырья (из карточки)
        public LabTestEditWindow(ApiService api, RawMaterialBatch batch, string user)
        {
            InitializeComponent();
            this.api = api;
            this.rawBatch = batch;
            currentUser = user;
            datePicker.SelectedDate = DateTime.Now;
            batchSelectionPanel.Visibility = Visibility.Collapsed;
            Loaded += async (s, e) => await LoadExecutors();
        }

        // Конструктор для нового испытания для готовой продукции (из карточки)
        public LabTestEditWindow(ApiService api, ProductionBatch batch, string user)
        {
            InitializeComponent();
            this.api = api;
            this.prodBatch = batch;
            currentUser = user;
            datePicker.SelectedDate = DateTime.Now;
            batchSelectionPanel.Visibility = Visibility.Collapsed;
            Loaded += async (s, e) => await LoadExecutors();
        }

        // Конструктор для редактирования существующего испытания
        public LabTestEditWindow(ApiService api, LabTest test, string user)
        {
            InitializeComponent();
            this.api = api;
            this.existingTest = test;
            currentUser = user;
            datePicker.SelectedDate = test.ДатаНазначения ?? DateTime.Now;
            batchSelectionPanel.Visibility = Visibility.Visible;
            Loaded += async (s, e) =>
            {
                await LoadExecutorsAndBatches();
                LoadTestData();
            };
        }

        private async Task LoadExecutorsAndBatches()
        {
            await LoadExecutors();
            batchTypeCombo.SelectionChanged += async (s, e) => await LoadBatchesByType();
            await LoadBatchesByType();
        }

        private async Task LoadBatchesByType()
        {
            if (batchTypeCombo.SelectedItem is ComboBoxItem selected)
            {
                if (selected.Content.ToString() == "Сырьё")
                {
                    var response = await api.GetAsync<ApiResponse<List<RawMaterialBatch>>>("api/RawMaterialBatches");
                    batchCombo.ItemsSource = response.Data;
                }
                else
                {
                    var response = await api.GetAsync<ApiResponse<List<ProductionBatch>>>("api/Batches");
                    batchCombo.ItemsSource = response.Data;
                }
            }
        }

        private async Task LoadExecutors()
        {
            var usersResp = await api.GetAsync<ApiResponse<List<User>>>("api/Users");
            var labStaff = usersResp.Data?.Where(u => u.Роль == "laboratory").ToList();
            executorCombo.ItemsSource = labStaff;
            if (labStaff != null && labStaff.Any())
                executorCombo.SelectedValue = labStaff.FirstOrDefault(u => u.ИмяПользователя == currentUser)?.ID ?? labStaff.First().ID;
        }

        private async void LoadTestData()
        {
            if (existingTest == null) return;

            typeCombo.Text = existingTest.ТипОбразца;
            datePicker.SelectedDate = existingTest.ДатаНазначения ?? DateTime.Now;
            executorCombo.SelectedValue = existingTest.ИсполнительID;
            priorityCombo.Text = existingTest.Приоритет ?? "обычный";
            paramBox.Text = existingTest.НаименованиеПараметра;
            normBox.Text = existingTest.НормативноеЗначение;
            measuredBox.Text = existingTest.ИзмеренноеЗначение;
            unitBox.Text = existingTest.ЕдиницаИзмерения;
            commentBox.Text = existingTest.КомментарийЛаборанта;

            // Устанавливаем тип партии
            if (existingTest.ПартияСырьяID != null)
                batchTypeCombo.SelectedIndex = 0;
            else if (existingTest.ПартияПроизводстваID != null)
                batchTypeCombo.SelectedIndex = 1;
            else
                return;

            // Ждём загрузки списка партий
            await LoadBatchesByType();

            // Устанавливаем выбранную партию (через Dispatcher для надёжности)
            Dispatcher.Invoke(() =>
            {
                if (existingTest.ПартияСырьяID != null)
                    batchCombo.SelectedValue = existingTest.ПартияСырьяID;
                else if (existingTest.ПартияПроизводстваID != null)
                    batchCombo.SelectedValue = existingTest.ПартияПроизводстваID;
            });
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(paramBox.Text))
            {
                MessageBox.Show("Введите параметр");
                return;
            }
            if (string.IsNullOrWhiteSpace(measuredBox.Text))
            {
                MessageBox.Show("Введите измеренное значение");
                return;
            }
            if (typeCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип испытания");
                return;
            }
            if (executorCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите исполнителя");
                return;
            }

            // Определяем ID партии
            int? rawId = null;
            int? prodId = null;

            if (rawBatch != null)
                rawId = rawBatch.ID;
            else if (prodBatch != null)
                prodId = prodBatch.ID;
            else
            {
                if (batchTypeCombo.SelectedIndex == 0)
                {
                    var selected = batchCombo.SelectedItem as RawMaterialBatch;
                    rawId = selected?.ID;
                }
                else
                {
                    var selected = batchCombo.SelectedItem as ProductionBatch;
                    prodId = selected?.ID;
                }
            }

            if (rawId == null && prodId == null)
            {
                MessageBox.Show("Выберите партию");
                return;
            }

            try
            {
                if (existingTest == null)
                {
                    // Создание
                    var newTest = new LabTest
                    {
                        ПартияСырьяID = rawId,
                        ПартияПроизводстваID = prodId,
                        ТипОбразца = (typeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                        ДатаНазначения = datePicker.SelectedDate,
                        ИсполнительID = (int?)executorCombo.SelectedValue,
                        Приоритет = (priorityCombo.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                        НаименованиеПараметра = paramBox.Text.Trim(),
                        НормативноеЗначение = string.IsNullOrWhiteSpace(normBox.Text) ? null : normBox.Text.Trim(),
                        ИзмеренноеЗначение = measuredBox.Text.Trim(),
                        ЕдиницаИзмерения = string.IsNullOrWhiteSpace(unitBox.Text) ? null : unitBox.Text.Trim(),
                        КомментарийЛаборанта = string.IsNullOrWhiteSpace(commentBox.Text) ? null : commentBox.Text.Trim(),
                        ДатаАнализа = DateTime.Now,
                        Результат = CompareWithNorm() ? "pass" : "fail",
                        Статус = "завершено"
                    };
                    var result = await api.PostAsync<ApiResponse<LabTest>>("api/QualityControl", newTest);
                    if (result.Success)
                        MessageBox.Show("Испытание создано");
                    else
                        MessageBox.Show($"Ошибка: {result.Message}");
                }
                else
                {
                    // Обновление
                    existingTest.ПартияСырьяID = rawId;
                    existingTest.ПартияПроизводстваID = prodId;
                    existingTest.ТипОбразца = (typeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    existingTest.ДатаНазначения = datePicker.SelectedDate;
                    existingTest.ИсполнительID = (int?)executorCombo.SelectedValue;
                    existingTest.Приоритет = (priorityCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    existingTest.НаименованиеПараметра = paramBox.Text.Trim();
                    existingTest.НормативноеЗначение = string.IsNullOrWhiteSpace(normBox.Text) ? null : normBox.Text.Trim();
                    existingTest.ИзмеренноеЗначение = measuredBox.Text.Trim();
                    existingTest.ЕдиницаИзмерения = string.IsNullOrWhiteSpace(unitBox.Text) ? null : unitBox.Text.Trim();
                    existingTest.КомментарийЛаборанта = string.IsNullOrWhiteSpace(commentBox.Text) ? null : commentBox.Text.Trim();
                    existingTest.ДатаАнализа = DateTime.Now;
                    existingTest.Результат = CompareWithNorm() ? "pass" : "fail";
                    existingTest.Статус = "завершено";
                    var result = await api.PutAsync<ApiResponse<LabTest>>($"api/QualityControl/{existingTest.ID}", existingTest);
                    if (result.Success)
                        MessageBox.Show("Испытание обновлено");
                    else
                        MessageBox.Show($"Ошибка: {result.Message}");
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private bool CompareWithNorm()
        {
            if (decimal.TryParse(measuredBox.Text, out var measured) && decimal.TryParse(normBox.Text, out var norm))
                return Math.Abs(measured - norm) <= norm * 0.05m;
            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}