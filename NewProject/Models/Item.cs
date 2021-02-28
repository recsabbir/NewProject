using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewProject.Models
{

    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public int category_id { get; set; }
        public int unit_price { get; set; }
        public int? employee_id { get; set; }

        [NotMapped]
        public ItemCategory category { get; set; }

        [NotMapped]
        public Employee employee { get; set; }

    }

    public class ItemCategory
    {
        public int id { get; set; }
        public string name { get; set; }
        public int priority { get; set; }

    }

    public class ItemDistribution
    {
        public int id { get; set; }
        public int item_id { get; set; }
        public int employee_id { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        //public Nullable<System.DateTime> end_date { get; set; }


        [NotMapped]
        public Item item { get; set; }

        [NotMapped]
        public Employee employee { get; set; }

    }
}