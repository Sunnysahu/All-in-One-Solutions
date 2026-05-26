using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Models
{
    public class Customers
    {
        public string FirstName { get; set; } = default;
        public string LastName { get; set; } = default;
        public string Address { get; set; } = default;
        public string PhoneNumber { get; set; } = default;
        public string City { get; set; } = default;
        public string State { get; set; } = string.Empty;
        public string Email { get; set; } = default;
        public int PostalCode { get; set; }
    }
}
