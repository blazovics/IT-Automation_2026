using System;
using System.Reflection;
using Xunit;
using UltimateApp;

namespace UltimateApp.Tests
{
    public class CalculatorTests
    {
        private static int InvokePrivate(string name, int a, int b)
        {
            var mi = typeof(Calculator).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(mi);
            return (int)mi.Invoke(null, new object[] { a, b });
        }

        [Fact]
        public void Add_ReturnsSum()
        {
            Assert.Equal(3, InvokePrivate("Add", 1, 2));
            Assert.Equal(-1, InvokePrivate("Add", -3, 2));
        }

        [Fact]
        public void Subtract_ReturnsDifference()
        {
            Assert.Equal(2, InvokePrivate("Subtract", 5, 3));
            Assert.Equal(-8, InvokePrivate("Subtract", -5, 3));
        }

        [Fact]
        public void Multiply_ReturnsProduct()
        {
            Assert.Equal(20, InvokePrivate("Multiply", 4, 5));
            Assert.Equal(0, InvokePrivate("Multiply", 0, 100));
        }

        [Fact]
        public void Divide_ReturnsQuotient()
        {
            Assert.Equal(5, InvokePrivate("Divide", 10, 2));
            Assert.Equal(3, InvokePrivate("Divide", 7, 2)); // integer division
        }

        [Fact]
        public void Divide_ByZero_Throws()
        {
            var mi = typeof(Calculator).GetMethod("Divide", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(mi);
            Assert.Throws<TargetInvocationException>(() => mi.Invoke(null, new object[] { 1, 0 }));
            // Underlying cause should be DivideByZeroException; reflection wraps it in TargetInvocationException.
        }

        [Fact]
        public void Modulo_ReturnsRemainder()
        {
            Assert.Equal(1, InvokePrivate("Modulo", 10, 3));
            Assert.Equal(-1, InvokePrivate("Modulo", -10, 3));
        }

        [Fact]
        public void Calculate_AddOperation_ReturnsSum()
        {
            Assert.Equal(3, Calculator.calculate(1, 2, Operation.Add));
        }

        [Fact]
        public void Calculate_SubtractOperation_ReturnsDifference()
        {
            Assert.Equal(2, Calculator.calculate(5, 3, Operation.Subtract));
        }

        [Fact]
        public void Calculate_MultiplyOperation_ReturnsProduct()
        {
            Assert.Equal(20, Calculator.calculate(4, 5, Operation.Multiply));
        }

        [Fact]
        public void Calculate_DivideOperation_ReturnsQuotient()
        {
            Assert.Equal(5, Calculator.calculate(10, 2, Operation.Divide));
        }

        [Fact]
        public void Calculate_ModuloOperation_ReturnsRemainder()
        {
            // This test asserts the expected behavior (remainder). If implementation is incorrect this will fail.
            Assert.Equal(1, Calculator.calculate(10, 3, Operation.Modulo));
        }

        [Fact]
        public void Calculate_InvalidOperation_ReturnsNull()
        {
            var invalid = (Operation)999;
            Assert.Null(Calculator.calculate(1, 1, invalid));
        }
    }
}