using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AgroControl.API.Models;

namespace AgroControl.Operator.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "http://localhost:5173/api";

        public ApiService() => _http = new HttpClient();

        // --- Авторизация ---
        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/Auth/login", new { Username = username, Password = password });
            if (!response.IsSuccessStatusCode) return false;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result?.Success == true;
        }

        // --- Активные партии (использует DTO из API) ---
        public async Task<List<ActiveBatchDto>?> GetActiveBatchesAsync()
        {
            var response = await _http.GetAsync($"{BaseUrl}/Batches/active");
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ActiveBatchDto>>>();
            return result?.Success == true ? result.Data : null;
        }

        // --- Программа партии (возвращает анонимный объект) ---
        public async Task<object?> GetBatchProgramAsync(int batchId)
        {
            var response = await _http.GetAsync($"{BaseUrl}/Batches/{batchId}/program");
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result?.Success == true ? result.Data : null;
        }

        // --- Шаги ---
        public async Task<bool> StartStepAsync(int executionId)
        {
            var response = await _http.PostAsync($"{BaseUrl}/Batches/steps/{executionId}/start", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CompleteStepAsync(int executionId)
        {
            var response = await _http.PostAsync($"{BaseUrl}/Batches/steps/{executionId}/complete", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateActualsAsync(int executionId, decimal? temp, int? duration, decimal? pressure, string comment)
        {
            var dto = new { ActualTemp = temp, ActualDuration = duration, ActualPressure = pressure, Comment = comment };
            var response = await _http.PutAsJsonAsync($"{BaseUrl}/Batches/steps/{executionId}/actuals", dto);
            return response.IsSuccessStatusCode;
        }

        // --- События ---
        public async Task<List<EventLog>?> GetBatchEventsAsync(int batchId)
        {
            var response = await _http.GetAsync($"{BaseUrl}/Batches/{batchId}/events");
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<EventLog>>>();
            return result?.Success == true ? result.Data : null;
        }

        // --- Сообщить о проблеме ---
        public async Task<bool> ReportIssueAsync(int batchId, string message, int operatorId)
        {
            var dto = new { Message = message, OperatorId = operatorId };
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/Batches/{batchId}/report-issue", dto);
            return response.IsSuccessStatusCode;
        }

        // --- Экструдер LIVE ---
        public async Task<ExtruderLiveData?> GetExtruderLiveDataAsync()
        {
            var response = await _http.GetAsync($"{BaseUrl}/Extruder/live");
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ExtruderLiveData>>();
            return result?.Success == true ? result.Data : null;
        }

        // --- Пользователи для аватара ---
        public async Task<List<User>?> GetUsersAsync()
        {
            var response = await _http.GetAsync($"{BaseUrl}/Users");
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<User>>>();
            return result?.Success == true ? result.Data : null;
        }

        // --- Заказы для создания партии ---
        public async Task<List<ProductionOrder>?> GetProductionOrdersAsync()
        {
            var response = await _http.GetAsync($"{BaseUrl}/ProductionOrders");
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductionOrder>>>();
            return result?.Success == true ? result.Data : null;
        }
        // --- Удаление партии ---
        public async Task<bool> DeleteBatchAsync(int batchId)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/Batches/{batchId}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> CompleteBatchAsync(int batchId, decimal quantity)
        {
            var dto = new { FactQuantity = quantity, Status = "завершена" };
            var response = await _http.PutAsJsonAsync($"{BaseUrl}/Batches/{batchId}/complete", dto);
            return response.IsSuccessStatusCode;
        }
        // --- Все проблемы ---
        public async Task<List<EventLog>?> GetAllIssuesAsync()
        {
            var response = await _http.GetAsync($"{BaseUrl}/Batches/issues");
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<EventLog>>>();
            return result?.Success == true ? result.Data : null;
        }
        // --- Создание партии ---
        public async Task<ProductionBatch?> CreateBatchAsync(ProductionBatch batch)
        {
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/Batches", batch);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProductionBatch>>();
            return result?.Success == true ? result.Data : null;
        }
    }
}