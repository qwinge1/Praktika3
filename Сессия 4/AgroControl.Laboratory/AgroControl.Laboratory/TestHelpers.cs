using System;
using System.Collections.Generic;
using System.Linq;
using AgroControl.API.Models;
using AgroControl.Laboratory.Helpers;

namespace AgroControl.Laboratory
{
    public static class TestHelpers
    {
        // 1. Генерация текста капчи
        public static string GenerateCaptchaText() => CaptchaGenerator.GenerateRandomText();

        // 2. Проверка, что изображение капчи создаётся
        public static bool IsCaptchaImageGenerated(string text)
            => CaptchaGenerator.GenerateImage(text) != null;

        // 3. Проверка, что испытание валидно (есть параметр и измеренное значение)
        public static bool IsLabTestValid(LabTest test)
            => test != null
               && !string.IsNullOrWhiteSpace(test.НаименованиеПараметра)
               && !string.IsNullOrWhiteSpace(test.ИзмеренноеЗначение);

        // 4. Проверка, что партия сырья имеет допустимый лабораторный статус
        public static bool IsValidRawMaterialStatus(string status)
        {
            var valid = new[] { "ожидает", "в работе", "одобрена", "заблокирована" };
            return valid.Contains(status);
        }

        // 5. Проверка, что партия готовой продукции имеет допустимый лабораторный статус
        public static bool IsValidProductBatchStatus(string status)
        {
            var valid = new[] { "ожидает", "одобрена", "заблокирована" };
            return valid.Contains(status);
        }

        // 6. Подсчёт количества завершённых испытаний в партии
        public static int CountCompletedTests(IEnumerable<LabTest> tests)
            => tests?.Count(t => t.Статус == "завершено") ?? 0;

        // 7. Определение, можно ли принимать решение по партии (все испытания завершены и есть хотя бы один pass)
        public static bool CanMakeDecision(IEnumerable<LabTest> tests)
        {
            if (tests == null || !tests.Any()) return false;
            return tests.All(t => t.Статус == "завершено" && t.Результат == "pass");
        }

        // 8. Получение инициалов из ФИО (как в MainWindow)
        public static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "??";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2) return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts[0][0].ToString().ToUpper();
        }

        // 9. Сравнение измеренного значения с нормативом (допуск ±5%)
        public static bool IsMeasuredValueInTolerance(decimal? measured, decimal? norm)
        {
            if (measured == null || norm == null) return false;
            return Math.Abs(measured.Value - norm.Value) <= norm.Value * 0.05m;
        }

        // 10. Проверка, что комментарий обязателен при блокировке
        public static bool IsCommentRequiredForBlock(string decision, string comment)
            => decision == "заблокирована" && string.IsNullOrWhiteSpace(comment);
    }
}