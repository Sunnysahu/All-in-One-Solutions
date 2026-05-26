using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Task.Data;
using Task.Repositories;
using Task.Utils;

namespace Task
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connectionString = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("DefaultConnection");

            int retryCount = 0;
            bool isValidChoice = false;

            while (retryCount < 3 && !isValidChoice)
            {
                Console.Clear();

                Console.Write("Welcome to OLX \nWhich Operation would you like to do ? \n 1. Create New Account \n 2. Update Account \n 3. Delete Account \n 4. Show My Profile \n\nPlease Give Command in Number as Above Shown : ");

                string userInput = Console.ReadLine();

                try
                {
                    int choice = Convert.ToInt32(userInput);
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("\nYou have selected to Create New Account\n");

                            UserInput input = new UserInput();

                            input.UserFirstNameInput();
                            input.UserLastNameInput();
                            input.UserAddressInput();
                            input.UserPhoneNumberInput();
                            input.UserCityInput();
                            input.UserStateInput();
                            input.UserEmailInput();
                            input.UserPostalCodeInput();
                            input.SaveCustomer(input, connectionString);
                            input.UserData();

                            //isValidChoice = true;
                            break;

                        case 2:
                            Console.WriteLine("\nYou have selected to Update Account");
                            isValidChoice = true;
                            break;

                        case 3:
                            Console.WriteLine("\nYou have selected to Delete Account");
                            isValidChoice = true;
                            break;

                        case 4:
                            Console.WriteLine("\nYou have selected to Show My Profile");
                            DbConnection db = new DbConnection(connectionString);

                            var customerRepository = new CustomerRepository(db);

                            customerRepository.GetAllCustomers();
                            //isValidChoice = true;
                            break;

                        default:
                            retryCount++;
                            Console.WriteLine($"\nInvalid Command! Attempts Left: {3 - retryCount}");
                            break;
                    }
                }
                catch
                {
                    retryCount++;
                    Console.WriteLine($"\nPlease enter numbers only! Attempts Left: {3 - retryCount}");
                }

                if (!isValidChoice && retryCount < 3)
                {
                    Console.WriteLine("\nPress any key to retry...");
                    Console.ReadKey();
                }
            }

            if (!isValidChoice)
            {
                Console.WriteLine("\nYou have exceeded maximum retry attempts.");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}