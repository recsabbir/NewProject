using System;
using System.Transactions;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using NewProject.Models;
using System.IO;
using Dapper;

namespace NewProject.Controllers
{
    public class EmployeesController : Controller
    {
        
        // GET: Employees
        public ActionResult Index()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            return View();
        }


        // GET: Employees/GetEmployees
        public JsonResult GetEmployees()
        {

            List<Employee> employeeList = new List<Employee>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                employeeList = db.Query<Employee>("Select * From Employees").ToList();
            }


            return Json(employeeList, JsonRequestBehavior.AllowGet);
        }

        // GET: Employees/GetEmployeeList
        public ActionResult GetEmployeeList()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            List<Employee> employeeList = new List<Employee>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                employeeList = db.Query<Employee>("Select * From Employees").ToList();
            }


            return PartialView("_EmployeeList", employeeList);
        }
        


        //GET: Employees/GetEmployee/5
        public JsonResult GetEmployee(int id)
        {
            Employee employee = new Employee();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                employee = db.Query<Employee>("Select * From Employees Where id = " + id).FirstOrDefault();
            }



            return Json(employee, JsonRequestBehavior.AllowGet);
        }

        //GET: Employees/CreateEmployee
        
        public ActionResult CreateEmployee()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            return PartialView("_CreateEmployee");

        }


        //POST: Employees/CreateEmployee
        [HttpPost]
        public bool CreateEmployee(Employee employee)
        {
            int newId = 1;
            Employee emplast;
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                emplast = db.Query<Employee>("Select * From Employees").ToList().LastOrDefault();
            }

            if (emplast != null)
            {
                newId = emplast.id + 1;
            }

            employee.id = newId;
            string im = employee.pic;

            if (employee.ImageFile != null)
            {
                if (im == null)
                {
                    im = "a.jpg";
                }
                if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Content/Employees/"), im)))
                {
                    // If file found, delete it
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Employees/"), im));
                }


                //string fileName = Path.GetFileNameWithoutExtension(emp.ImageFile.FileName);
                string fileName = employee.name;
                string extension = Path.GetExtension(employee.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                employee.pic = fileName;

                fileName = Path.Combine(Server.MapPath("~/Content/Employees/"), fileName);

                employee.ImageFile.SaveAs(fileName);
            }



            string sqlQueryStart = "Insert Into Employees (id,";
            string sqlQueryEnd = " Values (" + employee.id + ",";

            if (employee.name != null)
            {
                //string[] empname = employee.name.Trim().Split(' ');

                sqlQueryStart += "name";
                sqlQueryEnd += "'" + employee.name + "'";
            }
            if (employee.dob != null)
            {
                sqlQueryStart += ",dob";
                sqlQueryEnd += ",'" + employee.dob.Value.Year + "-" + employee.dob.Value.Month + "-" + employee.dob.Value.Day + "'";
            }
            if (employee.nid != null)
            {
                sqlQueryStart += ",nid";
                sqlQueryEnd += ",'" + employee.nid + "'";
            }
            if (employee.bgroup != null)
            {
                sqlQueryStart += ",bgroup";
                sqlQueryEnd += ",'" + employee.bgroup + "'";
            }
            if (employee.email != null)
            {
                sqlQueryStart += ",email";
                sqlQueryEnd += ",'" + employee.email + "'";
            }
            if (employee.phone != null)
            {
                sqlQueryStart += ",phone";
                sqlQueryEnd += ",'" + employee.phone + "'";
            }
            if (employee.address != null)
            {
                sqlQueryStart += ",address";
                sqlQueryEnd += ",'" + employee.address + "'";
            }
            if (employee.pic != null)
            {
                sqlQueryStart += ",pic";
                sqlQueryEnd += ",'" + employee.pic + "'";
            }
            if (employee.joining_date != null)
            {
                sqlQueryStart += ",joining_date";
                sqlQueryEnd += ",'" + employee.joining_date.Value.Year + "-" + employee.joining_date.Value.Month + "-" + employee.joining_date.Value.Day + "'";
            }
            else
            {
                DateTime jDate = DateTime.Now;
                sqlQueryStart += ",joining_date";
                sqlQueryEnd += ",'" + jDate.Year + "-" + jDate.Month + "-" + jDate.Day + "'";

            }



            sqlQueryStart += ")";
            sqlQueryEnd += ")";

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                string sqlQuery = sqlQueryStart+ sqlQueryEnd;
                db.Execute(sqlQuery);

            }

            return true;

        }


        //POST: Employees/UpdateEmployee
        [HttpPost]
        public bool UpdateEmployee(Employee employee)
        {
            Employee emp = new Employee();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                emp = db.Query<Employee>("Select * From Employees Where id = " + employee.id).FirstOrDefault();
            }

            if (emp.CheckEqual(employee) && employee.ImageFile == null)
            {
                return true;
            }

            string im = emp.pic;

            if (employee.ImageFile != null)
            {
                if (im == null)
                {
                    im = "a.jpg";
                }
                if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Content/Employees/"), im)))
                {
                    // If file found, delete it

                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Employees/"), im));

                }


                //string fileName = Path.GetFileNameWithoutExtension(emp.ImageFile.FileName);
                string fileName = employee.name;
                string extension = Path.GetExtension(employee.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                im = fileName;

                fileName = Path.Combine(Server.MapPath("~/Content/Employees/"), fileName);

                employee.ImageFile.SaveAs(fileName);


            }

            string sqlQuery = "Update Employees Set ";
            int updateCount = 0;

            if (emp.name != employee.name)
            {
                sqlQuery += "name='" + employee.name + "'";
                updateCount++;
            }
            if (emp.dob != employee.dob && employee.dob != null)
            {
                if (updateCount > 0)
                {
                    sqlQuery += ",";
                }
                sqlQuery += "dob='" + employee.dob.Value.Year +"-"+ employee.dob.Value.Month + "-" + employee.dob.Value.Day + "'";
                updateCount++;
            }
            if (emp.nid != employee.nid)
            {
                if (updateCount > 0)
                {
                    sqlQuery += ",";
                }
                sqlQuery += "nid='" + employee.nid + "'";
                updateCount++;
            }
            if (emp.bgroup != employee.bgroup)
            {
                if (updateCount > 0)
                {
                    sqlQuery += ",";
                }
                sqlQuery += "bgroup='" + employee.bgroup + "'";
                updateCount++;
            }
            if (emp.email != employee.email)
            {
                if (updateCount > 0)
                {
                    sqlQuery += ",";
                }
                sqlQuery += "email='" + employee.email + "'";
                updateCount++;
            }
            if (emp.phone != employee.phone)
            {
                if (updateCount > 0)
                {
                    sqlQuery += ",";
                }
                sqlQuery += "phone='" + employee.phone + "'";
                updateCount++;
            }
            if (emp.address != employee.address)
            {
                if (updateCount > 0)
                {
                    sqlQuery += ",";
                }
                sqlQuery += "address='" + employee.address + "'";
                updateCount++;
            }
            if (emp.pic != im)
            {
                if (updateCount > 0)
                {
                    sqlQuery += ",";
                }
                sqlQuery += "pic='" + im + "'";
                updateCount++;
            }
            if (emp.joining_date != employee.joining_date && employee.joining_date != null)
            {
                if (updateCount > 0)
                {
                    sqlQuery += ",";
                }
                sqlQuery += "joining_date='" + employee.joining_date.Value.Year + "-" + employee.joining_date.Value.Month + "-" + employee.joining_date.Value.Day + "'";
                updateCount++;
                
            }
            

            sqlQuery += " Where id=" + employee.id;
            if(updateCount > 0)
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                {

                    int rowsAffected = db.Execute(sqlQuery);
                }
            }
            

            return true;
        }


        //POST: Employees/DeleteEmployee/5
        [HttpPost]
        public bool DeleteEmployee(int id)
        {
            Employee employee = new Employee();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                employee = db.Query<Employee>("Select * From Employees Where id = " + id).FirstOrDefault();
            }

            if (employee == null)
            {
                return false;
            }

            string im = employee.pic;

            

            try
            {
                using( TransactionScope scope = new TransactionScope())
                {
                    using (IDbConnection db1 = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                    {
                        string sqlQuery1 = "Delete From Employees Where id = " + employee.id;
                        int rowsAffected1 = db1.Execute(sqlQuery1);

                        using (IDbConnection db2 = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                        {
                            string sqlQuery2 = "Delete From SalaryInfos Where employee_id = " + employee.id;
                            int rowsAffected2 = db2.Execute(sqlQuery2);
                        }


                    }

                    scope.Complete();

                }

            }
            catch (Exception e)
            {
                //var aaa = e;
                return false;
            }

            try
            {
                //deleteing the profile picture
                if (im == null)
                {
                    im = "a.jpg";
                }
                if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Content/Employees/"), im)))
                {
                    // If file found, delete it
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Employees/"), im));

                }
            }
            catch(Exception e)
            {
                //var abc = e;
                return true;
            }
            
            return true;
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            return View("Create");
        }

        // GET: Employees/UpdateSalaryInfo
        public ActionResult UpdateSalaryInfo()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            return PartialView("_AddSalaryInfo");
        }


        // POST: Employees/UpdateSalaryInfo/"id:year-month-salary"
        [HttpPost]
        public bool UpdateSalaryInfo(string data)
        {
            if (data.Split(':').Length != 2)
            {
                return false;
            }
            else
            {
                if (data.Split(':')[1].Split('-').Length != 3 && !(data.Split(':')[1].Split('-')[2].IsInt()))
                {
                    return false;
                }
            }
            //sqlQuery = "Insert Into SalaryInfos (id,employee_id,year,month,salary) Values (1,2,2020,:,2)"


            var _empid = data.Split(':')[0];
            string[] _data = data.Split(':')[1].Split('-');
            var _year = _data[0];
            var _month = _data[1];
            var _salary = _data[2];


            int newId = 1;
            SalaryInfo sinfo;
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                sinfo = db.Query<SalaryInfo>("Select * From SalaryInfos").ToList().LastOrDefault();
            }

            if (sinfo != null)
            {
                newId = sinfo.id + 1;
            }

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                sinfo = db.Query<SalaryInfo>("Select * From SalaryInfos where employee_id=" + _empid + " AND year=" + _year + " AND month=" + _month ).LastOrDefault();
            }
            string sqlQuery;

            if (sinfo != null)
            {
                sqlQuery = "Update SalaryInfos Set salary=" + _salary + " WHERE id=" + sinfo.id;
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                {

                    int rowsAffected = db.Execute(sqlQuery);
                }
            }
            else
            {
                sqlQuery = "Insert Into SalaryInfos (id,employee_id,year,month,salary) Values (" + newId + "," + _empid + "," + _year + "," + _month + "," + _salary + ")";
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                {
                    db.Execute(sqlQuery);
                }
            }

            

            return true;
        }


        // GET: Employees/UpdateSalaryInfoEmployees
        public JsonResult GetSalaryInfoEmployees()
        {
            List<Employee> employeeList = new List<Employee>();
            
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                employeeList = db.Query<Employee>("Select id, name From Employees").ToList();
            }

            List<string> ret = new List<string>();
            for( int i=0; i< employeeList.Count; i++)
            {
                ret.Add(employeeList[i].id + " : " + employeeList[i].name);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        

        // GET: Employees/GetJoiningDate/5
        public JsonResult GetJoiningDate(int id)
        {
            Employee emp = new Employee();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                emp = db.Query<Employee>("Select joining_date From Employees Where id=" + id).FirstOrDefault();
            }

            
            return Json(emp, JsonRequestBehavior.AllowGet);
        }


        // GET: Employees/GetSalaryInfoSalary/"id:year-month"
        public JsonResult GetSalaryInfoSalary(string data)
        {
            var emp_id = data.Split(':')[0];
            var year = data.Split(':')[1].Split('-')[0];
            var month = data.Split(':')[1].Split('-')[1];

            SalaryInfo sinfo = new SalaryInfo();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

               sinfo = db.Query<SalaryInfo>("Select salary From SalaryInfos Where employee_id=" + emp_id + " AND year=" + year + " AND month=" + month  ).FirstOrDefault();
            }

            if (sinfo == null)
            {
                sinfo = new SalaryInfo();
                sinfo.salary = 0;
            }
            return Json(sinfo, JsonRequestBehavior.AllowGet);
        }


        // GET: Employees/UpdateSalaryInfo
        public ActionResult ShowSalaryInfo()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            List<Employee> emp = new List<Employee>();
            List<AnnualSalary> sal = new List<AnnualSalary>();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                emp = db.Query<Employee>("Select * From Employees").ToList();
            }

            foreach(var item in emp)
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
                {

                    item.annualSalaries = db.Query<AnnualSalary>("SELECT year, SUM(salary) AS sumSalary FROM SalaryInfos WHERE employee_id=" + item.id + " GROUP BY year").ToList();
                }
            }

            return PartialView("_ShowSalaryInfo", emp);
        }

        // POST: Employees/ShowSalaryInfoByYear (id-year)
        //[HttpPost]
        public ActionResult ShowSalaryInfoByYear(string data)
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            var employee_id = data.Split('-')[0];
            var _year = data.Split('-')[1];
       
            Employee emp = new Employee();
            List<SalaryByYear> sal = new List<SalaryByYear>();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {

                emp = db.Query<Employee>("Select id, name From Employees WHERE id="+employee_id).FirstOrDefault();
            }

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["NPDB"].ConnectionString))
            {
                emp.salaryByYear = db.Query<SalaryByYear>("SELECT year, month, salary FROM SalaryInfos WHERE employee_id=" + employee_id + " AND year=" + _year).ToList();
            }
            

            return PartialView("_SalaryDetailByYear", emp);
        }

        public ActionResult SalaryInfo()
        {
            if (!CheckAccess())
            {
                return Redirect("~/Home");
            }

            return View("SalaryInfo");
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
