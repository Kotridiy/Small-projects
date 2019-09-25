using System;

namespace NotBinaryMath
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Number a = ");
            DecimalInt a = new DecimalInt(Console.ReadLine());
            Console.Write("Number b = ");
            DecimalInt b = new DecimalInt(Console.ReadLine());
            Console.WriteLine($"{a} + {b} = {a + b}");
        }
    }
}
