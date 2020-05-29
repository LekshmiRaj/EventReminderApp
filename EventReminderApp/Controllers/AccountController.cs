using EventReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace EventReminderApp.Controllers
{
    public class AccountController : Controller
    {
        string ConnectionString = @"Data Source=DESKTOP-3VSFCTT\LEKSHMISQL; Initial Catalog=EventReminderDB; Integrated Security=True";
        EventRepository eventRepository = new EventRepository();
        
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult test()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoginRegForm()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult SignUp(Registration registration)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string query = "Insert Into tblRegistration Values(@UserName,@Email,@Password)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserName", registration.UserName);
                cmd.Parameters.AddWithValue("@Email", registration.Email);
                cmd.Parameters.AddWithValue("@Password", registration.Password);               
                cmd.ExecuteNonQuery();                
            }
            return RedirectToAction("LoginRegForm", "Account");            
        }
       


        [HttpPost]
        public ActionResult SignIn(Registration login)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string query = $"Select UserId,UserName,Password from [dbo].[tblRegistration] where UserName='{login.UserName}' and Password='{login.Password}'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                // cmd.Parameters.AddWithValue("@UserId", login.UserId);
                //cmd.Parameters.AddWithValue("@UserName", login.UserName);                
                //cmd.Parameters.AddWithValue("@Password", login.Password);
                //SqlDataReader sdr = cmd.ExecuteReader();
                
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable datatable = new DataTable();
                sda.Fill(datatable);
                if (datatable.Rows.Count==1)
                {
                    DataRow row = datatable.Rows[0];
                    login.UserId =Convert.ToInt32(row["Userid"]);
                    string uid = row["Userid"].ToString();
                    Session["userid"] = uid;
                    Session["username"] = login.UserName;                    
                    return RedirectToAction("CalenderView", "Account");
                }
                else
                {
                   //alert("Incorrect username or Password");
                    ViewBag.Error = "User login failed";                    
                }
                con.Close();
                return RedirectToAction("LoginRegForm", "Account");
            }                       
        }

        public ActionResult UserHome()
        {
            string userid = Session["userid"].ToString();
            List<EventModel> eventList = eventRepository.ListEvents(userid);

            return View(eventList);
        }

        public ActionResult CreateNew()
        {
            ViewBag.page = "Create";
            return View(new EventModel());
        }

        [HttpPost]
        public ActionResult CreateNew(EventModel eventModel)
        {
            string userid = Session["userid"].ToString();
            eventRepository.AddEditEvent(eventModel, userid);
            return RedirectToAction("UserHome", "Account");
        }

        public ActionResult Edit(int id)
        {
            EventModel eventModel = eventRepository.GetEventById(id);
            ViewBag.page = "Edit";            
            return View("CreateNew", eventModel);
        }

        [HttpPost]
        public ActionResult Edit(EventModel eventModel)
        {            
            string userid = Session["userid"].ToString();            
            eventRepository.AddEditEvent(eventModel, userid);
            return RedirectToAction("UserHome", "Account");
        }

        public ActionResult Delete(int id)
        {
            eventRepository.DeleteEvent(id);
            return RedirectToAction("UserHome", "Account");
        }



// calender part /////////

        public ActionResult CalenderView()
        {
            return View();
        }

        public JsonResult GetEvents()
        {                         
            string userid = Session["userid"].ToString();
            
            List<EventModel> eventList = eventRepository.ListEvents(userid);

            return new JsonResult { Data = eventList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CalenderTest()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            var d = eventRepository.DeleteEvent(eventID);
            if (d != 0)
            {
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }


        [HttpPost]
        public JsonResult SaveEvent(EventModel eventModel)
        {
            //var status = false;
            string userid = Session["userid"].ToString();
            eventRepository.AddEditEvent(eventModel, userid);


            //using (MyDatabaseEntities dc = new MyDatabaseEntities())
            //{
            //    if (e.EventID > 0)
            //    {
            //        Update the event
            //        var v = dc.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
            //        if (v != null)
            //        {
            //            v.Subject = e.Subject;
            //            v.Start = e.Start;
            //            v.End = e.End;
            //            v.Description = e.Description;
            //        }
            //    }

            var status = true;
            //}
            return new JsonResult { Data = new { status = status } };
        }


    }

}




