#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using orm.attributes;
using ForeignKeyAttribute = orm.attributes.ForeignKeyAttribute;

namespace unittest.Models
{
    public class Company
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";
    }

    public class Order
    {
        [PrimaryKey]
        public int Id { get; set; }
    }

    public class User
    {
        [PrimaryKey]
        [Identity(Always = true)]
        public int Id { get; set; }

        [Required]
        [Unique]
        public string Email { get; set; } = "";

        [Default("active")]
        public string Status { get; set; } = "active";

        public string? Nickname { get; set; }

        [Unique(Group = "FullName")]
        public string FirstName { get; set; } = "";

        [Unique(Group = "FullName")]
        public string LastName { get; set; } = "";

        [ForeignKey(typeof(Company), OnDelete = "cascade")]
        public int CompanyId { get; set; }

        [HasMany]
        public List<Order> Orders { get; set; } = new();
    }

    public class LogEntry
    {
        public string Message { get; set; } = "";
    }

    public class CustomColumnEntity
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Column("user_email")]
        public string Email { get; set; } = "";
    }

    public class EmptyEntity
    {
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
    }
}
