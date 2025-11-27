using System;

public static class GenericSortTask
{
    public static void BubbleSort<T>(T[] array) where T : IComparable<T>
    {
        if (array == null) return;

        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (array[j].CompareTo(array[j + 1]) > 0)
                {
                    T temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }

    public static void Main()
    {
        int[] ints = { 64, 34, 25, 12, 22, 11, 90 };
        Console.WriteLine("int до: " + string.Join(", ", ints));
        BubbleSort(ints);
        Console.WriteLine("int после: " + string.Join(", ", ints));

        double[] doubles = { 3.5, 1.2, 4.8, 2.1 };
        Console.WriteLine("\ndouble до: " + string.Join(", ", doubles));
        BubbleSort(doubles);
        Console.WriteLine("double после: " + string.Join(", ", doubles));

        char[] chars = { 'z', 'a', 'x', 'c' };
        Console.WriteLine("\nchar до: " + string.Join(", ", chars));
        BubbleSort(chars);
        Console.WriteLine("char после: " + string.Join(", ", chars));

        string[] strings = { "banana", "apple", "cherry", "date" };
        Console.WriteLine("\nstring до: " + string.Join(", ", strings));
        BubbleSort(strings);
        Console.WriteLine("string после: " + string.Join(", ", strings));

        Circle[] circles = { new Circle(10), new Circle(5), new Circle(15), new Circle(7) };
        Console.WriteLine("\nCircle до:");
        Array.ForEach(circles, c => Console.WriteLine(c));
        BubbleSort(circles);
        Console.WriteLine("Circle после:");
        Array.ForEach(circles, c => Console.WriteLine(c));
    }
}

public class Circle : IComparable<Circle>
{
    public double Radius { get; set; }
    public Circle(double radius) => Radius = radius;
    public int CompareTo(Circle other) => other == null ? 1 : Radius.CompareTo(other.Radius);
    public override string ToString() => $"Circle(Radius = {Radius})";
}