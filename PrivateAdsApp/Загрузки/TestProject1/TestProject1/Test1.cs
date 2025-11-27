using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// === Задача 1: Сложение двух чисел ===
public static class MathOperations
{
    public static int Add(int a, int b) => checked(a + b);
}

[TestClass]
public class AddTests
{
    [TestMethod] public void Add_PositiveNumbers_ReturnsSum() => Assert.AreEqual(5, MathOperations.Add(2, 3));
    [TestMethod] public void Add_NegativeNumbers_ReturnsSum() => Assert.AreEqual(-5, MathOperations.Add(-2, -3));
    [TestMethod] public void Add_ZeroAndPositive_ReturnsPositive() => Assert.AreEqual(7, MathOperations.Add(0, 7));
    [TestMethod] public void Add_MaxValueAndOne_ThrowsOverflow() => Assert.ThrowsException<OverflowException>(() => MathOperations.Add(int.MaxValue, 1));
    [TestMethod] public void Add_MinValueAndNegativeOne_ThrowsOverflow() => Assert.ThrowsException<OverflowException>(() => MathOperations.Add(int.MinValue, -1));
}

// === Задача 2: Наибольшее число в массиве ===
public static class ArrayOperations
{
    public static int FindMax(int[] arr)
    {
        if (arr == null) throw new ArgumentNullException(nameof(arr));
        if (arr.Length == 0) throw new ArgumentException("Array is empty");
        return arr.Max();
    }
}

[TestClass]
public class FindMaxTests
{
    [TestMethod] public void FindMax_SingleElement_ReturnsElement() => Assert.AreEqual(10, ArrayOperations.FindMax(new[] { 10 }));
    [TestMethod] public void FindMax_MultipleElements_ReturnsMax() => Assert.AreEqual(100, ArrayOperations.FindMax(new[] { 1, 50, 100, 23 }));
    [TestMethod] public void FindMax_NegativeNumbers_ReturnsLeastNegative() => Assert.AreEqual(-1, ArrayOperations.FindMax(new[] { -10, -5, -1 }));
    [TestMethod] public void FindMax_EmptyArray_ThrowsException() => Assert.ThrowsException<ArgumentException>(() => ArrayOperations.FindMax(Array.Empty<int>()));
    [TestMethod] public void FindMax_NullArray_ThrowsException() => Assert.ThrowsException<ArgumentNullException>(() => ArrayOperations.FindMax(null));
}

// === Задача 3: Умножение двух чисел ===
public static class MultiplyOperations
{
    public static int Multiply(int a, int b) => checked(a * b);
}

[TestClass]
public class MultiplyTests
{
    [TestMethod] public void Multiply_PositiveNumbers_ReturnsProduct() => Assert.AreEqual(12, MultiplyOperations.Multiply(3, 4));
    [TestMethod] public void Multiply_NegativeAndPositive_ReturnsNegative() => Assert.AreEqual(-15, MultiplyOperations.Multiply(-3, 5));
    [TestMethod] public void Multiply_ZeroAndNumber_ReturnsZero() => Assert.AreEqual(0, MultiplyOperations.Multiply(0, 100));
    [TestMethod] public void Multiply_MaxValueAndTwo_ThrowsOverflow() => Assert.ThrowsException<OverflowException>(() => MultiplyOperations.Multiply(int.MaxValue, 2));

    // Исправленный тест: int.MinValue * -1 вызывает переполнение
    [TestMethod]
    public void Multiply_MinValueAndNegativeOne_ThrowsOverflow()
        => Assert.ThrowsException<OverflowException>(() => MultiplyOperations.Multiply(int.MinValue, -1));
}

// === Задача 4: Максимальное из трёх чисел ===
public static class MaxOfThree
{
    public static int Max(int a, int b, int c) => Math.Max(a, Math.Max(b, c));
}

[TestClass]
public class MaxOfThreeTests
{
    [TestMethod] public void Max_FirstIsLargest_ReturnsFirst() => Assert.AreEqual(10, MaxOfThree.Max(10, 5, 3));
    [TestMethod] public void Max_SecondIsLargest_ReturnsSecond() => Assert.AreEqual(20, MaxOfThree.Max(5, 20, 15));
    [TestMethod] public void Max_AllEqual_ReturnsValue() => Assert.AreEqual(7, MaxOfThree.Max(7, 7, 7));
    [TestMethod] public void Max_NegativeNumbers_ReturnsLargest() => Assert.AreEqual(-1, MaxOfThree.Max(-10, -5, -1));
    [TestMethod] public void Max_ZeroAndNegatives_ReturnsZero() => Assert.AreEqual(0, MaxOfThree.Max(-5, 0, -10));
}

// === Задача 5: Количество букв в строке ===
public static class StringOperations
{
    public static int CountLetters(string s)
    {
        if (s == null) return 0;
        return s.Count(char.IsLetter);
    }
}

[TestClass]
public class CountLettersTests
{
    [TestMethod] public void CountLetters_OnlyLetters_ReturnsLength() => Assert.AreEqual(10, StringOperations.CountLetters("HelloWorld"));

    // Исправлено: в строке 6 букв (a,b,c,d,e,f)
    [TestMethod] public void CountLetters_WithNumbers_ReturnsLetterCount() => Assert.AreEqual(6, StringOperations.CountLetters("abc12def"));

    [TestMethod] public void CountLetters_EmptyString_ReturnsZero() => Assert.AreEqual(0, StringOperations.CountLetters(""));
    [TestMethod] public void CountLetters_NullString_ReturnsZero() => Assert.AreEqual(0, StringOperations.CountLetters(null));
    [TestMethod] public void CountLetters_WithSpacesAndPunctuation_ReturnsLetterCount() => Assert.AreEqual(7, StringOperations.CountLetters("Hi, there!"));
}

// === Задача 6: Количество цифр в строке ===
public static class DigitOperations
{
    public static int CountDigits(string s)
    {
        if (s == null) return 0;
        return s.Count(char.IsDigit);
    }
}

[TestClass]
public class CountDigitsTests
{
    [TestMethod] public void CountDigits_OnlyDigits_ReturnsLength() => Assert.AreEqual(5, DigitOperations.CountDigits("12345"));
    [TestMethod] public void CountDigits_WithLetters_ReturnsDigitCount() => Assert.AreEqual(3, DigitOperations.CountDigits("abc12d3"));
    [TestMethod] public void CountDigits_EmptyString_ReturnsZero() => Assert.AreEqual(0, DigitOperations.CountDigits(""));
    [TestMethod] public void CountDigits_NullString_ReturnsZero() => Assert.AreEqual(0, DigitOperations.CountDigits(null));

    // Исправлено: в строке 6 цифр: 1,2,3,4,5,6
    [TestMethod] public void CountDigits_WithSpecialChars_ReturnsDigitCount() => Assert.AreEqual(6, DigitOperations.CountDigits("12#34!56"));
}

// === Задача 7: Количество специальных символов в строке ===
public static class SpecialCharOperations
{
    public static int CountSpecialChars(string s)
    {
        if (s == null) return 0;
        return s.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
    }
}

[TestClass]
public class CountSpecialCharsTests
{
    [TestMethod] public void CountSpecialChars_OnlySpecial_ReturnsLength() => Assert.AreEqual(3, SpecialCharOperations.CountSpecialChars("!@#"));
    [TestMethod] public void CountSpecialChars_WithLettersAndDigits_ReturnsSpecialCount() => Assert.AreEqual(2, SpecialCharOperations.CountSpecialChars("a1!b@c"));
    [TestMethod] public void CountSpecialChars_EmptyString_ReturnsZero() => Assert.AreEqual(0, SpecialCharOperations.CountSpecialChars(""));
    [TestMethod] public void CountSpecialChars_NullString_ReturnsZero() => Assert.AreEqual(0, SpecialCharOperations.CountSpecialChars(null));
    [TestMethod] public void CountSpecialChars_WithSpaces_IgnoresSpaces() => Assert.AreEqual(3, SpecialCharOperations.CountSpecialChars(" ! @ # "));
}