using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgroControl.Operator;

namespace AgroControl.Operator.Tests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void Test_BatchNumberValidation_Valid_ReturnsTrue()
        {
            bool isValid = TestHelpers.IsBatchNumberValid("B-2405-001");
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_BatchNumberValidation_Empty_ReturnsFalse()
        {
            bool isValid = TestHelpers.IsBatchNumberValid("");
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Test_BatchStatus_Valid_ReturnsTrue()
        {
            bool isValid = TestHelpers.IsValidBatchStatus("выполняется");
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_BatchStatus_Invalid_ReturnsFalse()
        {
            bool isValid = TestHelpers.IsValidBatchStatus("неизвестный");
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Test_CanStartStep_NotStarted_ReturnsTrue()
        {
            bool canStart = TestHelpers.CanStartStep("не начат");
            Assert.IsTrue(canStart);
        }

        [TestMethod]
        public void Test_CanStartStep_AlreadyStarted_ReturnsFalse()
        {
            bool canStart = TestHelpers.CanStartStep("выполняется");
            Assert.IsFalse(canStart);
        }

        [TestMethod]
        public void Test_CanCompleteStep_Started_ReturnsTrue()
        {
            bool canComplete = TestHelpers.CanCompleteStep("выполняется");
            Assert.IsTrue(canComplete);
        }

        [TestMethod]
        public void Test_CanCompleteStep_NotStarted_ReturnsFalse()
        {
            bool canComplete = TestHelpers.CanCompleteStep("не начат");
            Assert.IsFalse(canComplete);
        }

        [TestMethod]
        public void Test_TemperatureValidation_ValidRange_ReturnsTrue()
        {
            bool valid = TestHelpers.IsTemperatureValid(85.5m);
            Assert.IsTrue(valid);
        }

        [TestMethod]
        public void Test_PressureValidation_ValidRange_ReturnsTrue()
        {
            bool valid = TestHelpers.IsPressureValid(2.5m);
            Assert.IsTrue(valid);
        }

        [TestMethod]
        public void Test_QuantityValidation_Positive_ReturnsTrue()
        {
            bool valid = TestHelpers.IsQuantityValid(500m);
            Assert.IsTrue(valid);
        }

        [TestMethod]
        public void Test_QuantityValidation_Zero_ReturnsFalse()
        {
            bool valid = TestHelpers.IsQuantityValid(0);
            Assert.IsFalse(valid);
        }

        [TestMethod]
        public void Test_GetInitials_FromFullName_ReturnsCorrect()
        {
            string initials = TestHelpers.GetInitials("Зайцев Петр Николаевич");
            Assert.AreEqual("ЗП", initials);
        }

        [TestMethod]
        public void Test_CriticalDeviation_ExceedsThreshold_ReturnsTrue()
        {
            bool isCritical = TestHelpers.IsCriticalDeviation(100, 80, null, null, null, null);
            Assert.IsTrue(isCritical);
        }

        [TestMethod]
        public void Test_StatusColor_Approved_ReturnsGreen()
        {
            string color = TestHelpers.GetStatusColor("одобрена");
            Assert.AreEqual("Green", color);
        }

        [TestMethod]
        public void Test_StatusColor_Blocked_ReturnsDarkRed()
        {
            string color = TestHelpers.GetStatusColor("заблокирована");
            Assert.AreEqual("DarkRed", color);
        }
    }
}