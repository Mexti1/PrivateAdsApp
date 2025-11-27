using NUnit.Framework;
using FunctionsPractice;
using System;

namespace FunctionsPractice.Tests
{
    public class FunctionsTests
    {
        [TestCase(2, 3, 5)]
        [TestCase(-2, 5, 3)]
        [TestCase(0, 0, 0)]
        [TestCase(100, 200, 300)]
        [TestCase(-10, -20, -30)]
        public void Add_Test(int a, int b, int expected)
        {
            Assert.AreEqual(expected, Functions.Add(a, b));
        }

        [Test]
        public void FindMax_Test()
        {
            Assert.AreEqual(9, Functions.FindMax(new[] { 1, 9, 3 }));
            Assert.AreEqual(-1, Functions.FindMax(new[] { -5, -1, -10 }));
            Assert.AreEqual(0, Functions.FindMax(new[] { 0 }));
            Assert.AreEqual(100, Functions.FindMax(new[] { 10, 20, 100, 5 }));
            Assert.Throws<ArgumentException>(() => Functions.FindMax(new int[] { }));
        }

        [TestCase(2, 3, 6)]
        [TestCase(-2, 3, -6)]
        [TestCase(0, 10, 0)]
        [TestCase(-5, -4, 20)]
        [TestCase(7, 1, 7)]
        public void Multiply_Test(int a, int b, int expected)
        {
            Assert.AreEqual(expected, Functions.Multiply(a, b));
        }

        [TestCase(1, 2, 3, 3)]
        [TestCase(10, 5, 1, 10)]
        [TestCase(-5, -10, -2, -2)]
        [TestCase(0, 0, 0, 0)]
        [TestCase(5, 5, 10, 10)]
        public void MaxOfThree_Test(int a, int b, int c, int expected)
        {
            Assert.AreEqual(expected, Functions.MaxOfThree(a, b, c));
        }

        [TestCase("Hello123!", 5)]
        [TestCase("1234", 0)]
        [TestCase("", 0)]
        [TestCase("Привет!", 6)]
        [TestCase("C# Rocks!", 6)]
        public void CountLetters_Test(string input, int expected)
        {
            Assert.AreEqual(expected, Functions.CountLetters(input));
        }

        [TestCase("123abc", 3)]
        [TestCase("abc", 0)]
        [TestCase("1 2 3", 3)]
        [TestCase("", 0)]
        [TestCase("password2025", 4)]
        public void CountDigits_Test(string input, int expected)
        {
            Assert.AreEqual(expected, Functions.CountDigits(input));
        }

        [TestCase("Hello!", 1)]
        [TestCase("Hi, there.", 2)]
        [TestCase("NoSpecial", 0)]
        [TestCase("@#$%", 4)]
        [TestCase("123 456", 0)]
        public void CountSpecialCharacters_Test(string input, int expected)
        {
            Assert.AreEqual(expected, Functions.CountSpecialCharacters(input));
        }
    }
}
