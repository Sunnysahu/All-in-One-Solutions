using System;
using System.Collections.Generic;
using Task.Data;
using Task.Models;
using Task.Repositories;

namespace Task.Utils
{
    public class UserInput
    {
        Customers customers = new Customers();
        List<Customers> customerList = new List<Customers>();

        public void UserFirstNameInput()
        {
            Console.Write("Please Enter Your First Name : ");
            customers.FirstName = Console.ReadLine();
        }

        public void UserLastNameInput()
        {
            Console.Write("Please Enter Your Last Name : ");
            customers.LastName = Console.ReadLine();
        }

        public void UserAddressInput()
        {
            Console.Write("Please Enter Your Address : ");
            customers.Address = Console.ReadLine();
        }

        public void UserPhoneNumberInput()
        {
            Console.Write("Please Enter Your Phone Number : ");
            customers.PhoneNumber = Console.ReadLine();
        }

        public void UserCityInput()
        {
            Console.Write("Please Enter Your City : ");
            customers.City = Console.ReadLine();
        }

        public void UserStateInput()
        {
            Console.Write("Please Enter Your State : ");
            customers.State = Console.ReadLine();

        }

        public void UserEmailInput()
        {
            Console.Write("Please Enter Your Email Address : ");
            customers.Email = Console.ReadLine();
        }

        public void UserPostalCodeInput()
        {
            Console.Write("Please Enter Your Postal Code : ");
            customers.PostalCode = Convert.ToInt32(Console.ReadLine());
        }
        public void SaveCustomer(UserInput input, string connectionString)
        {
            customerList.Add(input.customers);

            DbConnection db = new DbConnection(connectionString);

            var customerRepository = new CustomerRepository(db);
            customerRepository.AddCustomer(input.customers);
        }
        public void UserData()
        {
            foreach (var customer in customerList)
            {
                Console.WriteLine($"First Name : {customer.FirstName} \nLast Name : {customer.LastName} \nAddress : {customer.Address} \nPhone Number : {customer.PhoneNumber} \nCity : {customer.City} \nState : {customer.State} \nEmail : {customer.Email} \nPostal Code : {customer.PostalCode}");
            }
            Console.WriteLine("\nYour Account has been Created Successfully\n");
        }
    }
}