using System;

namespace FunctionsPractice
{
    public static class Functions
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }

        // 2. Функция поиска наибольшего числа в массиве
        public static int FindMax(int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
                throw new ArgumentException("Массив не должен быть пустым.");
            int max = numbers[0];
            foreach (int num in numbers)
            {
                if (num > max)
                    max = num;
            }
            return max;
        }

        // 3. Функция умножения двух чисел
        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        // 4. Функция нахождения максимального из трёх чисел
        public static int MaxOfThree(int a, int b, int c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        // 5. Функция определения количества букв в строке
        public static int CountLetters(string input)
        {
            if (input == null) return 0;
            int count = 0;
            foreach (char c in input)
            {
                if (char.IsLetter(c)) count++;
            }
            return count;
        }

        // 6. Функция определения количества цифр в строке
        public static int CountDigits(string input)
        {
            if (input == null) return 0;
            int count = 0;
            foreach (char c in input)
            {
                if (char.IsDigit(c)) count++;
            }
            return count;
        }

        // 7. Функция определения количества специальных символов в строке
        public static int CountSpecialCharacters(string input)
        {
            if (input == null) return 0;
            int count = 0;
            foreach (char c in input)
            {
                if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
                    count++;
            }
            return count;
        }
    }
}
