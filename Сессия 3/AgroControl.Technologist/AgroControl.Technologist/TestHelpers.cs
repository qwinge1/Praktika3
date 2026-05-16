using System;
using System.Collections.Generic;
using AgroControl.API.Models;
using AgroControl.Technologist.Helpers;

namespace AgroControl.Technologist
{
    public static class TestHelpers
    {
        // 1. Генерация капчи
        public static string GenerateCaptchaText() => CaptchaGenerator.GenerateRandomText();

        // 2. Генерация изображения капчи (проверка, что не null)
        public static bool IsCaptchaImageGenerated(string text)
            => CaptchaGenerator.GenerateImage(text) != null;

        // 3. Валидация продукта (код и наименование обязательны)
        public static bool IsProductValid(Product product)
            => !string.IsNullOrWhiteSpace(product?.Код) && !string.IsNullOrWhiteSpace(product?.Наименование);

        // 4. Валидация рецепта (продукт и версия >0)
        public static bool IsRecipeValid(Recipe recipe)
            => recipe != null && recipe.ПродуктID > 0 && recipe.Версия > 0;

        // 5. Валидация заказа (номер не пустой, количество >0)
        public static bool IsOrderValid(ProductionOrder order)
            => order != null && !string.IsNullOrWhiteSpace(order.НомерЗаказа) && order.ПланКоличество_кг > 0;

        // 6. Валидация технологической карты (продукт >0, версия >0)
        public static bool IsTechCardValid(TechCard card)
            => card != null && card.ПродуктID > 0 && card.Версия > 0;

        // 7. Валидация партии (номер не пустой, заказ >0)
        public static bool IsBatchValid(ProductionBatch batch)
            => batch != null && !string.IsNullOrWhiteSpace(batch.НомерПартии) && batch.ЗаказID > 0;

        // 8. Подсчёт суммы процентов в рецепте (должно быть 100)
        public static decimal SumComponentPercentages(Recipe recipe)
        {
            if (recipe?.Состав == null) return 0;
            decimal sum = 0;
            foreach (var comp in recipe.Состав) sum += comp.Процент;
            return sum;
        }

        // 9. Преобразование ФИО в инициалы (как в MainWindow)
        public static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "??";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2) return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts[0][0].ToString().ToUpper();
        }

        // 10. Проверка, что статус партии допустим (список известных статусов)
        public static bool IsValidBatchStatus(string status)
        {
            var valid = new[] { "запланирована", "выполняется", "завершена", "заблокирована" };
            return valid.Contains(status);
        }
    }
}