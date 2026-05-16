using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AgroControl.API.Models;
using AgroControl.Laboratory;

namespace AgroControl.Laboratory.Tests
{
    [TestClass]
    public class LaboratoryTests
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
        public void Test_LabTestValidation_ValidTest_ReturnsTrue()
        {
            var test = new LabTest
            {
                НаименованиеПараметра = "Влажность",
                ИзмеренноеЗначение = "12.5"
            };
            bool isValid = TestHelpers.IsLabTestValid(test);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_LabTestValidation_InvalidTest_ReturnsFalse()
        {
            var test = new LabTest { НаименованиеПараметра = "", ИзмеренноеЗначение = null };
            bool isValid = TestHelpers.IsLabTestValid(test);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Test_RawMaterialStatus_Valid_ReturnsTrue()
        {
            bool isValid = TestHelpers.IsValidRawMaterialStatus("одобрена");
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_RawMaterialStatus_Invalid_ReturnsFalse()
        {
            bool isValid = TestHelpers.IsValidRawMaterialStatus("неизвестно");
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Test_ProductBatchStatus_Valid_ReturnsTrue()
        {
            bool isValid = TestHelpers.IsValidProductBatchStatus("ожидает");
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_CompletedTestsCount_ReturnsCorrectNumber()
        {
            var tests = new List<LabTest>
            {
                new LabTest { Статус = "завершено" },
                new LabTest { Статус = "завершено" },
                new LabTest { Статус = "в работе" }
            };
            int completed = TestHelpers.CountCompletedTests(tests);
            Assert.AreEqual(2, completed);
        }

        [TestMethod]
        public void Test_CanMakeDecision_AllPassAndCompleted_ReturnsTrue()
        {
            var tests = new List<LabTest>
            {
                new LabTest { Статус = "завершено", Результат = "pass" },
                new LabTest { Статус = "завершено", Результат = "pass" }
            };
            bool canDecide = TestHelpers.CanMakeDecision(tests);
            Assert.IsTrue(canDecide);
        }

        [TestMethod]
        public void Test_CanMakeDecision_MissingPass_ReturnsFalse()
        {
            var tests = new List<LabTest>
            {
                new LabTest { Статус = "завершено", Результат = "fail" }
            };
            bool canDecide = TestHelpers.CanMakeDecision(tests);
            Assert.IsFalse(canDecide);
        }

        [TestMethod]
        public void Test_GetInitials_FromFullName_ReturnsCorrect()
        {
            string fullName = "Петрова Анна Сергеевна";
            string initials = TestHelpers.GetInitials(fullName);
            Assert.AreEqual("ПА", initials);
        }

        [TestMethod]
        public void Test_MeasuredValueInTolerance_Valid_ReturnsTrue()
        {
            bool inTolerance = TestHelpers.IsMeasuredValueInTolerance(10.2m, 10m);
            Assert.IsTrue(inTolerance);
        }

        [TestMethod]
        public void Test_IsCommentRequiredForBlock_BlockWithoutComment_ReturnsTrue()
        {
            bool required = TestHelpers.IsCommentRequiredForBlock("заблокирована", "");
            Assert.IsTrue(required);
        }
    }
}