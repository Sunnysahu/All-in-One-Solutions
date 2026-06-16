using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sana
{
    public class Mother
    {
        public void PrepareFood()
        {
            Console.WriteLine("Mother prepares food.");
        }
    }

    public class Father : Mother
    {
        public void FatherEat()
        {
            Console.WriteLine("Father eats food.");
        }
    }

    public class Son : Mother
    {
        public void SonEat()
        {
            Console.WriteLine("Son eats food.");
        }
    }

    public class Daughter : Mother
    {
        public void DaughterEat()
        {
            Console.WriteLine("Daughter eats food.");
        }
    }

    public class CompanyLogin
    {
        public void Login() => Console.WriteLine("Login");
    }

    // Method Overlaoding, Method OverRiding, Method Hiding

    public class Intern : CompanyLogin
    {
    }
    public class CEO : CompanyLogin
    {
    }
    public class HR : CompanyLogin
    {
    }



}
