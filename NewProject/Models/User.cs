using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewProject.Models
{
    public class User
    {
        public int id { get; set; }
        public int employee_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }

        [NotMapped]
        public Employee employee { get; set; }

    }
}