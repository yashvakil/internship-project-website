using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Envision.Models;
using Envision.Filters;
using System.Text.RegularExpressions;

namespace Envision.Controllers
{
    [LogFilter]
    public class HomeController : Controller
    {
        string Baseurl = "http://localhost:42640";

        
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Login()
        {
            if (!Session["user_type"].Equals("GST"))
            {
                return RedirectToAction("/Home/Index");
            }
            return View();
        }

        
        public ActionResult Logout()
        {
            if (Session["user_type"].Equals("GST"))
            {
                return RedirectToAction("/Home/Index");
            }
            Session.Clear();
            Session.Abandon();
            Session["user_type"] = "GST";
            return View("../Home/Index");
        }

        [HttpPost]
        public async Task<ActionResult> Login(FormCollection collection)
        {
            if (Session["user_type"].Equals("STD") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }

            bool flag = false;
            if (string.IsNullOrEmpty(Request.Form["email"]) || string.IsNullOrEmpty(Request.Form["password"]))
            {
                flag = true;
                ViewBag.Message = "Email or Password Cannot be Empty";
            }
            if (flag)
            {
                return View();
            }

            try
            {
                string response = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string url;
                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 
                    if (Request.Form["type"].Equals("STD"))
                    {
                        url = "api/studentsAPI?email=" + Request.Form["email"].ToLower() + "&password=" + Request.Form["password"];
                    }
                    else
                    {
                        url = "api/companiesAPI?email=" + Request.Form["email"].ToLower() + "&password=" + Request.Form["password"];
                    }

                    HttpResponseMessage Res = await client.GetAsync(url);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var LoginResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(LoginResponse);

                    }
                }

                if (response.Equals("error"))
                {
                    ViewBag.Message = "Incorrect Credentials";
                    return View();
                }
                else
                {
                    Session["user_type"] = Request.Form["type"];
                    Session["user_id"] = response.ToString();
                    return RedirectToAction("../Home/Index");
                }

                
            }
            catch
            {
                return View();
            }

        }

        [HttpPost]
        public async Task<ActionResult> Index(FormCollection collection)
        {
            ContactRegErr err = new ContactRegErr();
            bool flag = false;
            Contact c = new Contact();
            if (string.IsNullOrWhiteSpace(Request.Form["email"]) || !Regex.IsMatch(Request.Form["email"], @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                flag = true;
                err.Email = "Email should be a valid email address";
            }
            else
            {
                c.Email = Request.Form["email"];
            }

            if (string.IsNullOrWhiteSpace(Request.Form["msg"]))
            {
                flag = true;
                err.Msg = "Message cannot be empty";
            }
            else
            {
                c.Msg = Request.Form["msg"];
            }

            if (string.IsNullOrWhiteSpace(Request.Form["name"]))
            {
                flag = true;
                err.Name = "Please enter your name";
            }
            else
            {
                c.Name = Request.Form["name"];
            }

            if (string.IsNullOrWhiteSpace(Request.Form["subject"]))
            {
                flag = true;
                err.Subject = "Please enter a subject of the message";
            }
            else
            {
                c.Subject = Request.Form["subject"];
            }

            err.c = c;
            if (flag)
            {
                ViewBag.Contact = err;
                return View(err);
            }

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

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/extraAPI/Contact", c);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("../Home/Index/");
            }
            catch
            {
                return RedirectToAction("../Home/Index/");
            }
        }

    }
}
