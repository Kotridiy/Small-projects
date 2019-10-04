using System;

namespace NotBinaryMath
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Number a = ");
            HumanInt a = new HumanInt(Console.ReadLine());
            Console.Write("Number b = ");
            HumanInt b = new HumanInt(Console.ReadLine());
            Console.WriteLine($"{a} * {b} = {a * b}");
            
        }
    }
}
