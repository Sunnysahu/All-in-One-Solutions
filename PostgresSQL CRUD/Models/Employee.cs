using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostgresSQL_CRUD.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("email")]
        public string? Email { get; set; }

        [Column("salary")]
        public decimal Salary { get; set; }

        [Column("department")]
        public string? Department { get; set; }
    }
}
