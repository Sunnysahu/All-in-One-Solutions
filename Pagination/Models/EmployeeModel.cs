using System.ComponentModel.DataAnnotations.Schema;

namespace Pagination.Models
{
    [Table("Employees")]
    public class EmployeeModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int Salary { get; set; }
    }
}
