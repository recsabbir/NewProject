using System;
using System.Web;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace NewProject.Models
{
    public class SalaryInfo
    {
        public int id { get; set; }
        public int employee_id { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int salary { get; set; }



    }

    public class AnnualSalary
    {
        public int year { get; set; }
        public int sumSalary { get; set; }
    }

    public class SalaryByYear
    {
        public int year { get; set; }
        public int month { get; set; }
        public int salary { get; set; }
    }
}