using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Envision.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Envision.Filters;

namespace Envision.Controllers
{
    [LogFilter]
    public class ExtraController : Controller
    {
        // GET: Extra
        string Baseurl = "http://localhost:42640";

        [HttpPost]
        public async Task<ActionResult> Skill(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            int skillid;
            int value;
            if (Request.Form["skill"] == null || Request.Form["value"] == null || !Int32.TryParse(Request.Form["value"], out value) || !Int32.TryParse(Request.Form["skill"], out skillid))
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            if (value > 5 || value < 0)
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            SkilledIn s = new SkilledIn();

            s.U_Id = Session["user_id"].ToString();
            s.Value = Convert.ToDecimal(Request.Form["value"]);
            s.S_ID = Convert.ToInt32(Request.Form["skill"]);

            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/extraAPI/Skill", s);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            catch
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Education(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            Studied s = new Studied();
            if (string.IsNullOrWhiteSpace(Request.Form["cname-education"]) && !string.IsNullOrWhiteSpace(Request.Form["college"]))
            {
                s.Name = Request.Form["college"];
            }
            else if (!string.IsNullOrWhiteSpace(Request.Form["cid-education"]))
            {
                s.GoogleId = Request.Form["cid-education"];
                s.Name = Request.Form["cname-education"];
            }
            else
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }

            if (string.IsNullOrWhiteSpace(Request.Form["graduation"]))
            {
                s.GraduationYear = null;
            }
            else
            {
                s.GraduationYear = Convert.ToDecimal(Request.Form["graduation"]);
            }

            s.Degree = Convert.ToInt32(Request.Form["degree"]);
            s.S_Id = Session["user_id"].ToString();
            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/extraAPI/Education", s);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            catch
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Work(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            WorkExperience w = new WorkExperience();

            if (string.IsNullOrWhiteSpace(Request.Form["cname-work"]) && !string.IsNullOrWhiteSpace(Request.Form["work"]))
            {
                w.Name = Request.Form["work"];
            }
            else if (!string.IsNullOrWhiteSpace(Request.Form["cid-work"]))
            {
                w.GoogleId = Request.Form["cid-work"];
                w.Name = Request.Form["cname-work"];
            }
            else
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }

            if (string.IsNullOrWhiteSpace(Request.Form["sdate"]) || string.IsNullOrWhiteSpace(Request.Form["designation"]))
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }

            if (string.IsNullOrWhiteSpace(Request.Form["edate"]))
            {
                w.EndDate = null;
            }
            else
            {
                w.EndDate = Convert.ToDateTime(Request.Form["edate"]);
            }
            w.S_Id = Session["user_id"].ToString();
            w.StartDate = Convert.ToDateTime(Request.Form["sdate"]);
            w.Designation = Request.Form["designation"];

            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/extraAPI/Work", w);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            catch
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Volunteer(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            VolunteerWork w = new VolunteerWork();

            if (string.IsNullOrWhiteSpace(Request.Form["cname-volunteer"]) && !string.IsNullOrWhiteSpace(Request.Form["volunteer"]))
            {
                w.Name = Request.Form["volunteer"];
            }
            else if (!string.IsNullOrWhiteSpace(Request.Form["cid-volunteer"]))
            {
                w.GoogleId = Request.Form["cid-volunteer"];
                w.Name = Request.Form["cname-volunteer"];
            }
            else
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }

            if (string.IsNullOrWhiteSpace(Request.Form["sdate"]) || string.IsNullOrWhiteSpace(Request.Form["topic"]))
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }

            if (string.IsNullOrWhiteSpace(Request.Form["edate"]))
            {
                w.EndDate = null;
            }
            else
            {
                w.EndDate = Convert.ToDateTime(Request.Form["edate"]);
            }

            w.S_Id = Session["user_id"].ToString();
            w.StartDate = Convert.ToDateTime(Request.Form["sdate"]);
            w.Topic = Request.Form["topic"];
            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/extraAPI/Volunteer", w);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            catch
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Skill(string id)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            int sid = Convert.ToInt32(id);
            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.DeleteAsync("api/extraAPI/Skill?userid=" + Session["user_id"].ToString() + "&skillid=" + sid);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            catch
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Education(string name, string degree, string graduation)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            string url = "api/extraAPI/Education?userid=" + Session["user_id"].ToString() + "&name=" + name + "&degree=" + degree + "&graduation=" + graduation;

            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.DeleteAsync(url);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            catch
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Work(string name, string designation, string startdate)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            string url = "api/extraAPI/Work?userid=" + Session["user_id"].ToString() + "&name=" + name + "&designation=" + designation + "&startdate=" + startdate;

            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.DeleteAsync(url);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            catch
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Volunteer(string name, string topic, string startdate)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            string url = "api/extraAPI/Volunteer?userid=" + Session["user_id"].ToString() + "&name=" + name + "&topic=" + topic + "&startdate=" + startdate;

            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.DeleteAsync(url);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
            catch
            {
                return RedirectToAction("../Student/Details/" + Session["user_id"]);
            }
        }

        
    }
}