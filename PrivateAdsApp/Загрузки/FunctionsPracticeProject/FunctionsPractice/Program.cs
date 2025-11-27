using System;

namespace FunctionsPractice
{
    public static class Functions
    {
        public static int Add(int a, int b) => a + b;

        public static int FindMax(int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
                throw new ArgumentException("Массив не должен быть пустым.");
            int max = numbers[0];
            foreach (int num in numbers)
                if (num > max) max = num;
            return max;
        }

        public static int Multiply(int a, int b) => a * b;

        public static int MaxOfThree(int a, int b, int c) => Math.Max(a, Math.Max(b, c));

        public static int CountLetters(string input)
        {
            if (input == null) return 0;
            int count = 0;
            foreach (char c in input)
                if (char.IsLetter(c)) count++;
            return count;
        }

        public static int CountDigits(string input)
        {
            if (input == null) return 0;
            int count = 0;
            foreach (char c in input)
                if (char.IsDigit(c)) count++;
            return count;
        }

        public static int CountSpecialCharacters(string input)
        {
            if (input == null) return 0;
            int count = 0;
            foreach (char c in input)
                if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
                    count++;
            return count;
        }
    }
}
