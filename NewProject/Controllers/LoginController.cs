using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using NewProject.Models;
using System.Configuration;
using Dapper;


namespace NewProject.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login/
        public ActionResult Index()
        {
            if(this.Session["User"] == null)
            {
                return View("Login");
            }
            else
            {
                return Redirect("~/Home");
            }
            
        }

        //POST: Login/Check
        [HttpPost]
        public String Index( String data )
        {
            var _username = data.Split(':')[0];
            var _password = data.Split(':')[1];
            User u = new User();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                u = db.Query<User>("Select * From Users WHERE username='"+ _username + "'").FirstOrDefault();
            }

            Employee emp = new Employee();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                emp = db.Query<Employee>("Select * From Employees WHERE id=" + u.employee_id).FirstOrDefault();
            }

            u.employee = emp;

            if (u != null)
            {
                if(u.password == _password)
                {
                    u.password = null;
                    this.Session["User"] = u;
                    return "LoggedIn";
                }
                else
                {
                    return "Wrong Password";
                }
            }
            else
            {
                return "Invalid Username";
            }

            

        }

        //GET: Logout
        public ActionResult Logout()
        {
            this.Session["User"] = null;
            return Redirect("~/Home");
        }
    }
}