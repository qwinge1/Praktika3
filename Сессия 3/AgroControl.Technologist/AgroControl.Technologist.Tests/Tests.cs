using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgroControl.API.Models;
using AgroControl.Technologist;
using System.Collections.Generic;

namespace AgroControl.Technologist.Tests
{
    [TestClass]
    public class ModuleTests
    {
        [TestMethod]
        public void Test_Captcha_GeneratesSixCharacters()
        {
            string captcha = TestHelpers.GenerateCaptchaText();
            Assert.AreEqual(6, captcha.Length, "Капча должна быть длиной 6 символов");
        }

        [TestMethod]
        public void Test_Captcha_ImageNotNull()
        {
            string text = "ABC123";
            bool isGenerated = TestHelpers.IsCaptchaImageGenerated(text);
            Assert.IsTrue(isGenerated, "Изображение капчи должно создаваться");
        }

        [TestMethod]
        public void Test_ProductValidation_ValidProduct_ReturnsTrue()
        {
            var product = new Product { Код = "P001", Наименование = "Гербицид" };
            bool isValid = TestHelpers.IsProductValid(product);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_ProductValidation_InvalidProduct_ReturnsFalse()
        {
            var product = new Product { Код = "", Наименование = null };
            bool isValid = TestHelpers.IsProductValid(product);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Test_RecipeValidation_ValidRecipe_ReturnsTrue()
        {
            var recipe = new Recipe { ПродуктID = 1, Версия = 2 };
            bool isValid = TestHelpers.IsRecipeValid(recipe);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_RecipeValidation_InvalidRecipe_ReturnsFalse()
        {
            var recipe = new Recipe { ПродуктID = 0, Версия = 0 };
            bool isValid = TestHelpers.IsRecipeValid(recipe);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Test_OrderValidation_ValidOrder_ReturnsTrue()
        {
            var order = new ProductionOrder { НомерЗаказа = "ORD-001", ПланКоличество_кг = 100 };
            bool isValid = TestHelpers.IsOrderValid(order);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_OrderValidation_InvalidOrder_ReturnsFalse()
        {
            var order = new ProductionOrder { НомерЗаказа = "", ПланКоличество_кг = 0 };
            bool isValid = TestHelpers.IsOrderValid(order);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Test_TechCardValidation_Valid_ReturnsTrue()
        {
            var card = new TechCard { ПродуктID = 1, Версия = 1 };
            bool isValid = TestHelpers.IsTechCardValid(card);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_BatchValidation_Valid_ReturnsTrue()
        {
            var batch = new ProductionBatch { НомерПартии = "B-001", ЗаказID = 5 };
            bool isValid = TestHelpers.IsBatchValid(batch);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_GetInitials_FromFullName_ReturnsCorrect()
        {
            string fullName = "Иванов Иван Иванович";
            string initials = TestHelpers.GetInitials(fullName);
            Assert.AreEqual("ИИ", initials);
        }

        [TestMethod]
        public void Test_BatchStatus_ValidStatus_ReturnsTrue()
        {
            bool isValid = TestHelpers.IsValidBatchStatus("выполняется");
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_BatchStatus_InvalidStatus_ReturnsFalse()
        {
            bool isValid = TestHelpers.IsValidBatchStatus("неизвестный");
            Assert.IsFalse(isValid);
        }

        // Добавьте 10-й тест, если нужно – например, проверка суммы процентов рецепта
        [TestMethod]
        public void Test_RecipeComponentSum_EmptyComposition_ReturnsZero()
        {
            var recipe = new Recipe { Состав = new List<RecipeComponent>() };
            decimal sum = TestHelpers.SumComponentPercentages(recipe);
            Assert.AreEqual(0, sum);
        }
    }
}