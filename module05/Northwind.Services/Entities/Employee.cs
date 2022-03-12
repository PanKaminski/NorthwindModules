﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Northwind.Services.Entities
{
    public partial class Employee
    {
        public Employee()
        {
            this.InverseReportsToNavigation = new HashSet<Employee>();
            this.Orders = new HashSet<Order>();
        }

        [Key]
        [Column("EmployeeID")]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required]
        [StringLength(10)]
        public string FirstName { get; set; }

        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(25)]
        public string TitleOfCourtesy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? BirthDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? HireDate { get; set; }

        [StringLength(60)]
        public string Address { get; set; }

        [StringLength(15)]
        public string City { get; set; }

        [StringLength(15)]
        public string Region { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(15)]
        public string Country { get; set; }

        [StringLength(24)]
        public string HomePhone { get; set; }

        [StringLength(4)]
        public string Extension { get; set; }

        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }

        [Column(TypeName = "ntext")]
        public string Notes { get; set; }

        public int? ReportsTo { get; set; }

        [StringLength(255)]
        public string PhotoPath { get; set; }

        [ForeignKey(nameof(ReportsTo))]
        [InverseProperty(nameof(Employee.InverseReportsToNavigation))]
        public virtual Employee ReportsToNavigation { get; set; }

        [InverseProperty(nameof(Employee.ReportsToNavigation))]
        public virtual ICollection<Employee> InverseReportsToNavigation { get; set; }

        [InverseProperty(nameof(Order.Employee))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
