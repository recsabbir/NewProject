using System;
using System.Web;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace NewProject.Models
{
    public class Employee
    {
        public int id { get; set; }
        public string name { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public string nid { get; set; }
        public string bgroup { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string pic { get; set; }
        public Nullable<System.DateTime> joining_date { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }

        [NotMapped]
        public List<AnnualSalary> annualSalaries { get; set; }

        [NotMapped]
        public List<SalaryByYear> salaryByYear { get; set; }

        public Boolean CheckEqual(Employee emp)
        {
            if (this.id == emp.id && this.name == emp.name && this.dob == emp.dob
                && this.nid == emp.nid && this.bgroup == emp.bgroup && this.email == emp.email
                && this.phone == emp.phone && this.address == emp.address && this.pic == emp.pic)
            {
                return true;
            }

            return false;
        }

    }
}