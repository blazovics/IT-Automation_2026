using System;
using System.IO;
using System.Reflection;
using Xunit;
using UltimateApp;

namespace UltimateApp.Tests
{
    public class ProgramTests : IDisposable
    {
        private readonly TextReader _originalIn;
        private readonly TextWriter _originalOut;
        private readonly StringWriter _output;

        public ProgramTests()
        {
            _originalIn = Console.In;
            _originalOut = Console.Out;
            _output = new StringWriter();
            Console.SetOut(_output);
        }

        public void Dispose()
        {
            Console.SetIn(_originalIn);
            Console.SetOut(_originalOut);
            _output.Dispose();
        }

        private object InvokeProgram(string methodName, params object[] parameters)
        {
            var mi = typeof(Program).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(mi);
            return mi.Invoke(null, parameters);
        }

        [Fact]
        public void GetInputOperands_InvalidInteger_ReturnsNull_AndWritesError()
        {
            Console.SetIn(new StringReader("notanumber\n"));
            var result = InvokeProgram("getInputOperands", "prompt");
            Assert.Null(result);

            var outText = _output.ToString();
            Assert.Contains("Unable to parse 'notanumber'", outText);
        }

        [Fact]
        public void GetInputOperands_ReadLineReturnsNull_ReturnsNull()
        {
            // StringReader with empty string causes ReadLine to return null immediately.
            Console.SetIn(new StringReader(string.Empty));
            var result = InvokeProgram("getInputOperands", "prompt");
            Assert.Null(result);

            var outText = _output.ToString();
            // No parse error should be written in this case.
            Assert.DoesNotContain("Unable to parse", outText);
        }

        [Theory]
        [InlineData("+", Operation.Add)]
        [InlineData("-", Operation.Subtract)]
        [InlineData("*", Operation.Multiply)]
        [InlineData("/", Operation.Divide)]
        [InlineData("%", Operation.Modulo)]
        public void GetOperationType_ValidSymbol_ReturnsOperation(string input, Operation expected)
        {
            Console.SetIn(new StringReader(input + "\n"));
            var result = InvokeProgram("getOperationType", "prompt");
            Assert.IsType<Operation>(result);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetOperationType_InvalidSymbol_ReturnsNull()
        {
            Console.SetIn(new StringReader("x\n"));
            var result = InvokeProgram("getOperationType", "prompt");
            Assert.Null(result);
        }

        [Fact]
        public void Main_ValidInputs_WritesResult()
        {
            // Provide: first operand, operation, second operand
            Console.SetIn(new StringReader("10\n+\n5\n"));
            // Main signature: static void Main(string[] args)
            var mi = typeof(Program).GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(mi);
            mi.Invoke(null, new object[] { new string[0] });

            var outText = _output.ToString();
            Assert.Contains("10 Add 5 = 15", outText);
            Assert.DoesNotContain("Bad input", outText);
        }

        [Fact]
        public void Main_InvalidFirstOperand_WritesBadInputAndParseMessage()
        {
            Console.SetIn(new StringReader("bad\n+\n5\n"));
            var mi = typeof(Program).GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(mi);
            mi.Invoke(null, new object[] { new string[0] });

            var outText = _output.ToString();
            Assert.Contains("Unable to parse 'bad'", outText);
            Assert.Contains("Bad input", outText);
        }
    }
}