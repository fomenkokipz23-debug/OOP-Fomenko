using System;
using Xunit;
using lab30vN;

namespace lab30vN.Tests
{
    public class LoanCalculatorTests
    {
        private readonly LoanCalculator calculator = new LoanCalculator();

        [Fact]
        public void CalculateMonthlyPayment_ValidInput_ReturnsPositiveValue()
        {
            var result = calculator.CalculateMonthlyPayment(100000, 10, 12);
            Assert.True(result > 0);
        }

        [Theory]
        [InlineData(100000, 10, 12)]
        [InlineData(50000, 5, 24)]
        [InlineData(200000, 7, 36)]
        public void CalculateMonthlyPayment_DifferentLoans_ReturnsPayment(double loan, double rate, int months)
        {
            var result = calculator.CalculateMonthlyPayment(loan, rate, months);
            Assert.True(result > 0);
        }

        [Fact]
        public void CalculateMonthlyPayment_ZeroRate_ReturnsSimpleDivision()
        {
            var result = calculator.CalculateMonthlyPayment(12000, 0, 12);
            Assert.Equal(1000, result);
        }

        [Fact]
        public void CalculateMonthlyPayment_NegativeLoan_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                calculator.CalculateMonthlyPayment(-1000, 10, 12));
        }

        [Fact]
        public void CalculateMonthlyPayment_NegativeRate_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                calculator.CalculateMonthlyPayment(1000, -5, 12));
        }

        [Fact]
        public void CalculateMonthlyPayment_ZeroMonths_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                calculator.CalculateMonthlyPayment(1000, 5, 0));
        }

        [Theory]
        [InlineData(10000, 10, 6)]
        [InlineData(15000, 12, 12)]
        [InlineData(5000, 8, 24)]
        public void CalculateMonthlyPayment_EdgeCases_ReturnsValue(double loan, double rate, int months)
        {
            var result = calculator.CalculateMonthlyPayment(loan, rate, months);
            Assert.True(result > 0);
        }

        [Fact]
        public void CalculateMonthlyPayment_LargeLoan_ReturnsValue()
        {
            var result = calculator.CalculateMonthlyPayment(1000000, 15, 60);
            Assert.True(result > 0);
        }

        [Fact]
        public void CalculateMonthlyPayment_OneMonthLoan_ReturnsCorrect()
        {
            var result = calculator.CalculateMonthlyPayment(1000, 12, 1);
            Assert.True(result > 0);
        }
    }
}