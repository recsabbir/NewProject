using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using NewProject.Models;
using Dapper;
using Newtonsoft.Json.Linq;

namespace NewProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (this.Session["User"] != null)
            {
                var _user = this.Session["User"] as User;

                if (_user.role == "A")
                {
                    return View();
                }
                else
                {
                    return Redirect("~/Login/Logout");
                }

                
            }
            else
            {
                return Redirect("~/Login");
            }

            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult DashBoard()
        {
            return PartialView("_DashBoard");
        }

        public JsonResult Data_ItemPiChart()
        {
            List<Item> itemList = new List<Item>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                itemList = db.Query<Item>("Select * From Items").ToList();
            }

            dynamic mymodel = new ExpandoObject();

            int avl = (from item in itemList where item.employee_id == null select item).Count();
            int asn = (from item in itemList where item.employee_id != null select item).Count();

            

            //string str = "{ 'context_name': { 'lower_bound': 'value', 'upper_bound': 'value', 'values': [ 'value1', 'valueN' ] } }";
            //string jsonStr = "{data : [available : "+ avl + ", assigned : "+ asn +"]}";
            //JObject json = JObject.Parse(jsonStr);

            mymodel.available = avl;
            mymodel.assigned = asn;
            

            return Json(mymodel, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult Data_ItemDropdown()
        {
            List<Item> itemList = new List<Item>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                itemList = db.Query<Item>("Select * From Items").ToList();
            }

            dynamic mymodel = new ExpandoObject();

            List<Item> avl = new List<Item>();
            List<Item> asn = new List<Item>();
            avl = (from item in itemList where item.employee_id == null select item).ToList();
            asn = (from item in itemList where item.employee_id != null select item).ToList();

            //string data = @"[
            //                    'assigned' : " + asn + @",
            //                    'available' : " + avl + @"
            //                ]";

            mymodel.available = avl;
            mymodel.assigned = asn;


            return Json(mymodel, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Data_NumberOfEmployee()
        {
            //List<Item> itemList = new List<Item>();
            int numberOfEmployee;
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                numberOfEmployee = db.Query<int>("Select COUNT(id) From Employees").FirstOrDefault();
            }

            dynamic mymodel = new ExpandoObject();

            mymodel.totalEmployee = numberOfEmployee;
           
            return Json(mymodel, JsonRequestBehavior.AllowGet);
        }

    }
}