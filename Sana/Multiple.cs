using System;

namespace Sana
{
    public interface IAddition
    {
        void Add();
    }

    public interface ISubtraction
    {
        void Sub();
    }

    

    public class Functionality : IAddition, ISubtraction
    {
        public void Add()
        {
            Console.WriteLine("Addition Operation");
        }

        public void Sub()
        {
            Console.WriteLine("Subtraction Operation");
        }
    }

    public class Calculator : Functionality
    {

    }
}
