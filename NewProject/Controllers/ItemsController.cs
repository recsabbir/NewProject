using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Dynamic;
using System.Data.SqlClient;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Configuration;
using Dapper;
using NewProject.Models;
using System.Transactions;



namespace NewProject.Controllers
{
    public class ItemsController : Controller
    {

        // GET: Items
        public ActionResult Index()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            return View("Items");
        }

        public ActionResult Category()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }
            

            return View("ItemCategory");
        }

        public ActionResult ShowItemCategory()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            List<ItemCategory> icategory = new List<ItemCategory>();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                icategory = db.Query<ItemCategory>("Select * From ItemCategories").ToList();
            }

            return PartialView("_ShowItemCategories", icategory);
        }

        public ActionResult AddCategory()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            return PartialView("_AddItemCategory");
        }

        [HttpPost]
        public ActionResult AddCategory(String data )
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            var _name = data.Split(':')[0];
            var _priority = data.Split(':')[1];

            ItemCategory icategory = new ItemCategory();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                icategory = db.Query<ItemCategory>("Select * From ItemCategories").LastOrDefault();
            }
            int newId = 1;
            if (icategory != null)
            {
                newId = icategory.id + 1;
            }

            string sqlQueryStart = "Insert Into ItemCategories (id,";
            string sqlQueryEnd = " Values (" + newId + ",";

            if (_name != null)
            {
                //string[] empname = employee.name.Trim().Split(' ');

                sqlQueryStart += "name";
                sqlQueryEnd += "'" + _name + "'";
            }
            if (_priority != null && _priority.ToString().IsInt())
            {
                sqlQueryStart += ",priority";
                sqlQueryEnd += ",'" + _priority  + "'";
            }
        
            sqlQueryStart += ")";
            sqlQueryEnd += ")";

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                string sqlQuery = sqlQueryStart + sqlQueryEnd;
                db.Execute(sqlQuery);

            }

            return Content("Added");
        }

        public ActionResult GetItemCategory(int id)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            ItemCategory icategory = new ItemCategory();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                icategory = db.Query<ItemCategory>("Select * From ItemCategories WHERE id=" + id).FirstOrDefault();
            }

            return PartialView("_EditItemCategory", icategory);
        }

        [HttpPost]
        public ActionResult UpdateItemCategory(string data)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            var _id = data.Split(':')[0];
            var _name = data.Split(':')[1];
            var _priority = data.Split(':')[2];
            


            ItemCategory icategory = new ItemCategory();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                icategory = db.Query<ItemCategory>("Select * From ItemCategories WHERE id=" + _id).FirstOrDefault();
            }

            string sqlQuery = "UPDATE ItemCategories SET name='" + _name + "',priority='" + _priority + "' WHERE id=" + _id;


            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                var col_Affected = db.Execute(sqlQuery);
            }

            return Content("Added");

        }


        //POST: Items/DeleteItemCategory/5
        [HttpPost]
        public ActionResult DeleteItemCategory(int id)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }


            ItemCategory icategory = new ItemCategory();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                icategory = db.Query<ItemCategory>("Select * From ItemCategories Where id = " + id).FirstOrDefault();
            }

            if (icategory == null)
            {
                return Content("False");
            }

            
            try
            {
                 using (IDbConnection db1 = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                 {
                        string sqlQuery1 = "Delete From ItemCategories Where id = " + id;
                        int rowsAffected1 = db1.Execute(sqlQuery1);
                 }

            }
            catch (Exception e)
            {
                //var aaa = e;
                return Content("False");
            }

            return Content("True");
        }

        //GET: Items/ShowItems
        public ActionResult ShowItems()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            List<Item> itm = new List<Item>();

            //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            //{

            //    itm = db.Query<Item>("Select * From Items").ToList();
            //}


            string sql = "SELECT * FROM Items AS A INNER JOIN ItemCategories AS B ON A.category_id = B.id;";

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                
                    itm = db.Query<Item, ItemCategory, Item>(
                        sql,
                        (item, itemCategory) =>
                        {
                            item.category = itemCategory;
                            return item;
                        })
                    .Distinct()
                    .ToList();
            }

            return PartialView("_ShowItems", itm);
        }

        //GET: Items/GetItem
        public ActionResult GetItem(int id)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            Item itm = new Item();

            //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            //{

            //    itm = db.Query<Item>("Select * From Items").ToList();
            //}


            string sql = "SELECT * FROM Items AS A INNER JOIN ItemCategories AS B ON A.category_id = B.id WHERE A.id="+id+";";

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm = db.Query<Item, ItemCategory, Item>(
                    sql,
                    (item, itemCategory) =>
                    {
                        item.category = itemCategory;
                        return item;
                    })
                .Distinct()
                .FirstOrDefault();
            }

            List<ItemCategory> category = new List<ItemCategory>();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                category = db.Query<ItemCategory>("Select * From ItemCategories").ToList();
            }

            dynamic mymodel = new ExpandoObject();

            mymodel.Item = itm;
            mymodel.Category = category;


            return PartialView("_EditItem", mymodel);
        }

        //GET: Items/AddItem
        public ActionResult AddItem()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            List<ItemCategory> icategory = new List<ItemCategory>();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                icategory = db.Query<ItemCategory>("Select * From ItemCategories").ToList();
            }

            return PartialView("_AddItem", icategory);
        }


        //POST: Items/AddItem
        [HttpPost]
        public ActionResult AddItem(string data)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            var _name = data.Split(':')[0];
            var _categoryId = data.Split(':')[1];
            var _unitPrice = data.Split(':')[2];


            Item itm = new Item();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm = db.Query<Item>("Select * From Items").LastOrDefault();
            }
            int newId = 1;
            if (itm != null)
            {
                newId = itm.id + 1;
            }

            string sqlQueryStart = "Insert Into Items (id,";
            string sqlQueryEnd = " Values (" + newId + ",";

            if (_name != null)
            {   
                sqlQueryStart += "name";
                sqlQueryEnd += "'" + _name + "'";
            }
            if (_categoryId != null && _categoryId.ToString().IsInt())
            {
                sqlQueryStart += ",category_id";
                sqlQueryEnd += ",'" + _categoryId + "'";
            }
            if (_unitPrice != null && _unitPrice.ToString().IsInt())
            {
                sqlQueryStart += ",unit_price";
                sqlQueryEnd += ",'" + _unitPrice + "'";
            }

            sqlQueryStart += ")";
            sqlQueryEnd += ")";

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                string sqlQuery = sqlQueryStart + sqlQueryEnd;
                db.Execute(sqlQuery);

            }

            return Content("Added");

        }

        //POST: Items/DeleteItemCategory/5
        [HttpPost]
        public ActionResult DeleteItem(int id)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }


            Item itm = new Item();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm = db.Query<Item>("Select * From Items Where id = " + id).FirstOrDefault();
            }

            if (itm == null)
            {
                return Content("False");
            }


            try
            {
                using(TransactionScope scope = new TransactionScope())
                {
                    using (IDbConnection db1 = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                    {
                        string sqlQuery1 = "Delete From Items Where id = " + id;
                        int rowsAffected1 = db1.Execute(sqlQuery1);

                        using (IDbConnection db2 = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                        {
                            string sqlQuery2 = "Delete From ItemDistributions Where item_id=" + id;
                            int rowsAffected2 = db1.Execute(sqlQuery1);
                        }
                    }

                    scope.Complete();
                }

                

            }
            catch (Exception e)
            {
                //var aaa = e;
                return Content("False");
            }

            return Content("True");
        }

        //POST: Items/UpdateItem
        [HttpPost]
        public ActionResult UpdateItem(string data)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            var _id = data.Split(':')[0];
            var _name = data.Split(':')[1];
            var _categoryId = data.Split(':')[2];
            var _unitPrice = data.Split(':')[3];


            Item itm = new Item();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm = db.Query<Item>("Select * From Items WHERE id=" + _id).FirstOrDefault();
            }

            string sqlQuery = "UPDATE Items SET name='" + _name + "',category_id='" + _categoryId + "',unit_price='"+ _unitPrice+"' WHERE id="+ _id;
            

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                var col_Affected = db.Execute(sqlQuery);
            }

            return Content("Added");

        }

        //GET: Items/Distribution
        public ActionResult Distribution()
        {
            return View("ItemDistribution");
        }

        //GET: Items/ShowItemDistribution
        public ActionResult ShowItemDistribution()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            List<ItemDistribution> itm = new List<ItemDistribution>();

            //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            //{

            //    itm = db.Query<Item>("Select * From Items").ToList();
            //}


            string sql = "SELECT * FROM ItemDistributions AS A INNER JOIN Items AS B ON A.item_id = B.id INNER JOIN Employees AS C ON A.employee_id = C.id";

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm = db.Query<ItemDistribution, Item, Employee, ItemDistribution>(
                    sql,
                    (itemDistribution, item, employee) =>
                    {
                        itemDistribution.item = item;
                        itemDistribution.employee = employee;

                        return itemDistribution;
                    })
                .Distinct()
                .ToList();
            }

            return PartialView("_ShowItemDistribution", itm);
        }

        //GET: Items/AddItemDistribution
        public ActionResult AddItemDistribution()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            List<Item> itm = new List<Item>();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm = db.Query<Item>("Select * From Items WHERE employee_id IS NULL").ToList();
            }

            List<Employee> emp = new List<Employee>();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                emp = db.Query<Employee>("Select * From Employees").ToList();
            }

            dynamic mymodel = new ExpandoObject();

            mymodel.Items = itm;
            mymodel.Employees = emp;

            return PartialView("_AddItemDistribution", mymodel);
        }


        //POST: Items/AddItemDistribution
        [HttpPost]
        public ActionResult AddItemDistribution(string data)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            var _itemId = data.Split(':')[0];
            var _employeeId = data.Split(':')[1];
            var _startDate = data.Split(':')[2];
            //var _endDate = data.Split(':')[3];


            ItemDistribution itm_dis = new ItemDistribution();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm_dis = db.Query<ItemDistribution>("Select * From ItemDistributions").LastOrDefault();
            }
            int newId = 1;
            if (itm_dis != null)
            {
                newId = itm_dis.id + 1;
            }

            string sqlQueryStart = "Insert Into ItemDistributions (id,";
            string sqlQueryEnd = " Values (" + newId + ",";

            if (_itemId != "" && _itemId.ToString().IsInt())
            {
                sqlQueryStart += "item_id";
                sqlQueryEnd += "'" + _itemId + "'";
            }
            if (_employeeId != "" && _employeeId.ToString().IsInt())
            {
                sqlQueryStart += ",employee_id";
                sqlQueryEnd += ",'" + _employeeId + "'";
            }
            if (_startDate != "")
            {
                sqlQueryStart += ",start_date";
                sqlQueryEnd += ",'" + _startDate + "'";
            }
            else
            {
                DateTime _sd = DateTime.Today;
                sqlQueryStart += ",start_date";
                sqlQueryEnd += ",'" + _sd + "'";
            }
            //if (_endDate != null)
            //{
            //    sqlQueryStart += ",end_date";
            //    sqlQueryEnd += ",'" + _endDate + "'";
            //}

            sqlQueryStart += ")";
            sqlQueryEnd += ")";

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                string sqlQuery = sqlQueryStart + sqlQueryEnd;
                db.Execute(sqlQuery);

            }

            //string[] st_d = _startDate.Split('-');
            ////string[] en_d = _startDate.Split('-');

            //DateTime _today = DateTime.Today;

            //DateTime start_date = new DateTime(Int16.Parse(st_d[0]), Int16.Parse(st_d[1]), Int16.Parse(st_d[2]));

            //if(DateTime.Compare(start_date, _today) <= 0)
            //{
            //    string sqlQuery2 = "UPDATE Items SET employee_id='" + _employeeId + "' WHERE id=" + _itemId;


            //    using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            //    {
            //        var col_Affected = db.Execute(sqlQuery2);
            //    }

            //}

            string sqlQuery2 = "UPDATE Items SET employee_id='" + _employeeId + "' WHERE id=" + _itemId;

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                var col_Affected = db.Execute(sqlQuery2);
            }


            return Content("Distributed");

        }



        //POST: Items/DeleteItemDistribution/5
        [HttpPost]
        public ActionResult DeleteItemDistribution(int id)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }


            ItemDistribution itm = new ItemDistribution();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm = db.Query<ItemDistribution>("Select * From ItemDistributions Where id = " + id).FirstOrDefault();
            }

            if (itm == null)
            {
                return Content("False");
            }


            try
            {
                using(TransactionScope scope = new TransactionScope())
                {
                    using (IDbConnection db1 = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                    {
                        string sqlQuery1 = "Delete From ItemDistributions Where id = " + id;
                        int rowsAffected1 = db1.Execute(sqlQuery1);

                        using (IDbConnection db2 = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                        {
                            string sqlQuery2 = "UPDATE Items SET employee_id=NULL Where id = " + itm.item_id;
                            int colAffected = db2.Execute(sqlQuery2);

                        }
                    }

                    scope.Complete();

                }
                

            }
            catch (Exception e)
            {
                //var aaa = e;
                return Content("False");
            }

            return Content("True");
        }


        //GET: Items/GetItemDistribution
        public ActionResult GetItemDistribution(int id)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            ItemDistribution itmds = new ItemDistribution();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itmds = db.Query<ItemDistribution>("Select * From ItemDistributions WHERE id="+id).FirstOrDefault();
            }

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itmds.item = db.Query<Item>("Select * From Items WHERE id=" + itmds.item_id).FirstOrDefault();
            }

            List<Employee> emps = new List<Employee>();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                emps = db.Query<Employee>("Select * From Employees").ToList();
            }

            dynamic mymodel = new ExpandoObject();

            mymodel.ItemDistribution = itmds;
            mymodel.Employees = emps;


            return PartialView("_EditItemDistribution", mymodel);
        }


        //POST: Items/UpdateItemDistribution
        [HttpPost]
        public ActionResult UpdateItemDistribution(string data)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            var _id = data.Split(':')[0];
            var _employeeId = data.Split(':')[1];
            var _startDate = data.Split(':')[2];
            

            bool employee_change = false;
             

            ItemDistribution itm = new ItemDistribution();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                itm = db.Query<ItemDistribution>("Select * From ItemDistributions WHERE id=" + _id).FirstOrDefault();
            }

            string sqlQuery = "UPDATE ItemDistributions SET ";
            
            if( itm.employee_id != Int16.Parse(_employeeId) )
            {
                sqlQuery += "employee_id=" + _employeeId;
                employee_change = true;
            }

            if (employee_change)
            {
                sqlQuery += ",";
            }

            sqlQuery += "start_date='" + _startDate + "' WHERE id=" + _id;

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                var col_Affected = db.Execute(sqlQuery);
            }

            if (employee_change)
            {
                sqlQuery = "UPDATE Items SET employee_id=" + _employeeId + " WHERE id=" + itm.item_id;

                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                {
                    var col_Affected = db.Execute(sqlQuery);
                }
            }

            return Content("Updated");

        }


        public bool CheckAccess()
        {
            if (this.Session["User"] != null)
            {
                var u = this.Session["User"] as User;
                if (u.role == "A")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



    }
}