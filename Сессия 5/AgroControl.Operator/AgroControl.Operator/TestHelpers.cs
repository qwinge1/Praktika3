using System;
using System.Collections.Generic;
using System.Linq;
using AgroControl.API.Models;

namespace AgroControl.Operator
{
    public static class TestHelpers
    {
        // 1. Проверка, что номер партии не пустой
        public static bool IsBatchNumberValid(string batchNumber)
            => !string.IsNullOrWhiteSpace(batchNumber);

        // 2. Проверка, что статус партии допустим
        public static bool IsValidBatchStatus(string status)
        {
            var valid = new[] { "запланирована", "выполняется", "завершена", "заблокирована" };
            return valid.Contains(status);
        }

        // 3. Проверка, что шаг партии может быть начат (статус "не начат" или "выполняется"? по логике: можно начать если не начат)
        public static bool CanStartStep(string stepStatus)
            => stepStatus == "не начат";

        // 4. Проверка, что шаг можно завершить (статус "выполняется")
        public static bool CanCompleteStep(string stepStatus)
            => stepStatus == "выполняется";

        // 5. Проверка допустимых значений температуры (например, в разумных пределах)
        public static bool IsTemperatureValid(decimal? temp)
            => temp == null || (temp >= -50 && temp <= 500);

        // 6. Проверка допустимых значений давления
        public static bool IsPressureValid(decimal? pressure)
            => pressure == null || (pressure >= 0 && pressure <= 100);

        // 7. Проверка, что количество продукции при завершении партии >0
        public static bool IsQuantityValid(decimal quantity)
            => quantity > 0;

        // 8. Получение инициалов из ФИО (как в MainWindow)
        public static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "??";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2) return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts[0][0].ToString().ToUpper();
        }

        // 9. Определение, есть ли критическое отклонение в шаге
        public static bool IsCriticalDeviation(decimal? actualTemp, decimal? planTemp,
                                               decimal? actualPressure, decimal? planPressure,
                                               int? actualDuration, int? planDuration)
        {
            bool tempCritical = actualTemp.HasValue && planTemp.HasValue && Math.Abs(actualTemp.Value - planTemp.Value) > 5;
            bool pressureCritical = actualPressure.HasValue && planPressure.HasValue && Math.Abs(actualPressure.Value - planPressure.Value) > 0.5m;
            bool durationCritical = actualDuration.HasValue && planDuration.HasValue && Math.Abs(actualDuration.Value - planDuration.Value) > 30;
            return tempCritical || pressureCritical || durationCritical;
        }

        // 10. Преобразование статуса партии в цветовую индикацию (для тестирования конвертера)
        public static string GetStatusColor(string status)
        {
            switch (status?.ToLower())
            {
                case "одобрена":
                case "pass":
                    return "Green";
                case "заблокирована":
                case "blocked":
                    return "DarkRed";
                case "выполняется":
                    return "DarkOrange";
                case "критическое":
                    return "Red";
                default:
                    return "Black";
            }
        }
    }
}