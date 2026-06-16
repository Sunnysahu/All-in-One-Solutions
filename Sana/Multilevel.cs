using System;


namespace Sana
{
    public class Add
    {
        public void AddNumber(int num1, int num2) => Console.WriteLine(num1 + num2);
    }

    public class Subtract : Add
    {
        public void SubtractNumber(int num1, int num2) => Console.WriteLine(num1 - num2);
    }

    public class Multiply : Subtract
    {
        public void MultiplyNumber(int num1, int num2) => Console.WriteLine(num1 * num2);
    }
}
