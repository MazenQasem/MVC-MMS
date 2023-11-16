using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace MMS_Models.Models
{
    public class EmpDocModel
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
    
    public partial class Employee
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string EmpCode { get; set; }
    }
    public partial class Doctor
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string EmpCode { get; set; }
    }
    }
}