using System;
using System.Collections.Generic;
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
    public class CompanyController : Controller
    {
        private EnvisionEntities db = new EnvisionEntities();
        string Baseurl = "http://localhost:42640";
        
        // GET: Company/Details/5
        public async Task<ActionResult> Details(string id)
        {
            ViewBag.Industries = db.IndustryLists;
            CompanyView CompanyInfo = new CompanyView();
            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    HttpResponseMessage Res = await client.GetAsync("api/companiesAPI/" + id);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var CompanyResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        CompanyInfo = JsonConvert.DeserializeObject<CompanyView>(CompanyResponse);

                    }
                    else
                    {
                        ViewBag.ResError = "No Such Company Exists";
                        CompanyInfo = null;
                    }
                    //returning the employee list to view  
                    return View(CompanyInfo);
                }
            }
            catch
            {
                ViewBag.ResError = "No Such Company Exists";
                CompanyInfo = null;
                return View(CompanyInfo);
            }
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            if (Session["user_type"].Equals("CMP") || Session["user_type"].Equals("STD"))
            {
                return RedirectToAction("../Home/Index");
            }
            ViewBag.Industries = db.IndustryLists;
            CompanyRegErr c = new CompanyRegErr();
            return View(c);
        }

        
        public async Task<ActionResult> Internships()
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("STD"))
            {
                return RedirectToAction("../Home/Index");
            }
            
            List<InternshipView> InternshipInfo = new List<InternshipView>();

            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    HttpResponseMessage Res = await client.GetAsync("api/internshipsAPI?companyid=" + Session["user_id"]);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var InternshipResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        InternshipInfo = JsonConvert.DeserializeObject<List<InternshipView>>(InternshipResponse);

                    }
                    else
                    {
                        ViewBag.ResError = "No Such Internships Exists";
                        InternshipInfo = null;
                    }
                    //returning the employee list to view  
                    return View(InternshipInfo);
                }
            }
            catch
            {
                ViewBag.ResError = "No Such Internships Exists";
                InternshipInfo = null;
                return View(InternshipInfo);
            }
        }

        
        public async Task<ActionResult> Applicants()
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("STD"))
            {
                return RedirectToAction("../Home/Index");
            }

            List<InternshipView> StudentInfo = new List<InternshipView>();

            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    HttpResponseMessage Res = await client.GetAsync("api/extraAPI/Applicants?companyid=" + Session["user_id"]);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var InternshipResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        StudentInfo = JsonConvert.DeserializeObject<List<InternshipView>>(InternshipResponse);

                    }
                    else
                    {
                        ViewBag.ResError = "No Such Applicants";
                        StudentInfo = null;
                    }
                    //returning the employee list to view  
                    return View(StudentInfo);
                }
            }
            catch
            {
                ViewBag.ResError = "No Such Applicants";
                StudentInfo = null;
                return View(StudentInfo);
            }
        }


        // POST: Company/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            if (Session["user_type"].Equals("CMP") || Session["user_type"].Equals("STD"))
            {
                return RedirectToAction("../Home/Index");
            }
            bool flag = false;
            CompanyRegErr err = new CompanyRegErr();
            ViewBag.Industries = db.IndustryLists;
            Company c = new Company();
            if (string.IsNullOrWhiteSpace(Request.Form["cname"]) && !string.IsNullOrWhiteSpace(Request.Form["gname"]))
            {
                c.Name = Request.Form["gname"];
            }
            else if (!string.IsNullOrWhiteSpace(Request.Form["cid"]))
            {
                c.GoogleId = Request.Form["cid"];
                c.Name = Request.Form["cname"];
            }
            else
            {
                flag = true;
                err.Name = "Company Name required";
            }
            if (string.IsNullOrWhiteSpace(Request.Form["about"]))
            {
                flag = true;
                err.About = "You must write something about your company";
            }
            else
            {
                c.About = Request.Form["about"];
            }

            if (string.IsNullOrWhiteSpace(Request.Form["cid"]))
            {
                c.Verified = false;
            }
            else
            {
                c.Verified = true;
            }
            c.Email = Request.Form["email"];
            c.Password = Request.Form["password"];
            c.CompanyType = Request.Form["type"];
            if (!string.IsNullOrWhiteSpace(Request.Form["industry"]))
            {
                foreach(string s in Request.Form["industry"].Split(','))
                {
                    CompanyIndustry tempc = new CompanyIndustry();
                    tempc.I_Id = Convert.ToInt32(s);
                    tempc.Time = System.DateTime.Now;
                    c.CompanyIndustries.Add(tempc);
                }
            }

            HttpPostedFileBase logo = Request.Files["logo"];

            if ((logo != null) && (logo.ContentLength > 0) && !string.IsNullOrWhiteSpace(logo.FileName))
            {
                byte[] logoBytes = new byte[logo.ContentLength];
                logo.InputStream.Read(logoBytes, 0, Convert.ToInt32(logo.ContentLength));
                c.Logo = logoBytes;
            }

            c.WebsiteURL = Request.Form["website"];

            err.c = c;

            if (string.IsNullOrWhiteSpace(c.Email) || !Regex.IsMatch(c.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                flag = true;
                err.Email = "Email Cannot be empty. It should be a valid email address.";
            }
            
            if (string.IsNullOrWhiteSpace(c.Name))
            {
                flag = true;
                err.Name = "Company Name Cannot be Empty";
            }
            if (string.IsNullOrWhiteSpace(c.Password))
            {
                flag = true;
                err.Password = "Password Cannot be Empty";
            }
            if (!c.Password.Equals(Request.Form["repassword"]))
            {
                flag = true;
                err.RePassword = "Both Passwords do not match";
            }
            if (string.IsNullOrWhiteSpace(c.About))
            {
                flag = true;
                err.About = "You must write something about the company";
            }


            if (flag)
            {
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

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/companiesAPI/", c);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var CompanyResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(CompanyResponse);

                    }
                }

                if(response.Equals("email exists"))
                {
                    err.Email = "This Email Id Already Exists";
                    return View(err);
                }
                else
                {
                    Session["user_type"] = "CMP";
                    Session["user_id"] = response;
                    return RedirectToAction("../Home/Index");
                }
                
            }
            catch
            {
                return View(err);
            }
            
        }

        
        // GET: Company/Edit/5
        public async Task<ActionResult> Edit()
        {
            if(Session["user_type"].Equals("STD") || Session["user_type"].Equals("GST"))
            {
                return RedirectToAction("../Home/Index");
            }
            ViewBag.Industries = db.IndustryLists;
            CompanyView CompanyInfo = new CompanyView();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/companiesAPI/" + Session["user_id"]);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var CompanyResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    CompanyInfo = JsonConvert.DeserializeObject<CompanyView>(CompanyResponse);

                }
                ViewBag.Industries = db.IndustryLists;
                CompanyRegErr err = new CompanyRegErr();
                Company c = new Company();
                c.Id = CompanyInfo.Id;
                c.Logo = CompanyInfo.Logo;
                c.Name = CompanyInfo.Name;
                c.CompanyType = CompanyInfo.CompanyType;
                c.WebsiteURL = CompanyInfo.WebsiteURL;
                c.GoogleId = CompanyInfo.GoogleId;
                c.About = CompanyInfo.About;

                err.c = c;
                //returning the employee list to view  
                return View(err);
            }
            
        }


        // POST: Company/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("STD"))
            {
                return RedirectToAction("../Home/Index");
            }

            bool flag = false;
            CompanyRegErr err = new CompanyRegErr();
            ViewBag.Industries = db.IndustryLists;
            Company c = new Company();

            if (string.IsNullOrEmpty(Request.Form["id"]) || !Session["user_id"].Equals(Request.Form["id"]))
            {
                return RedirectToAction("../Home/Index");
            }
            else
            {
                c.Id = Request.Form["id"];
            }
            
            if (string.IsNullOrWhiteSpace(Request.Form["cname"]) && !string.IsNullOrWhiteSpace(Request.Form["gname"]))
            {
                c.Name = Request.Form["gname"];
            }
            else if (!string.IsNullOrWhiteSpace(Request.Form["cid"]))
            {
                c.GoogleId = Request.Form["cid"];
                c.Name = Request.Form["cname"];
            }
            else
            {
                flag = true;
                err.Name = "Company Name required";
            }

            if (string.IsNullOrWhiteSpace(Request.Form["about"]))
            {
                flag = true;
                err.About = "You must write something about your company";
            }
            else
            {
                c.About = Request.Form["about"];
            }
            
            if (string.IsNullOrWhiteSpace(Request.Form["cid"]))
            {
                c.Verified = false;
            }
            else
            {
                c.Verified = true;
            }

            c.CompanyType = Request.Form["type"];
            if (!string.IsNullOrWhiteSpace(Request.Form["industry"]))
            {
                foreach (string s in Request.Form["industry"].Split(','))
                {
                    CompanyIndustry tempc = new CompanyIndustry();
                    tempc.I_Id = Convert.ToInt32(s);
                    tempc.Time = System.DateTime.Now;
                    c.CompanyIndustries.Add(tempc);
                }
            }

            HttpPostedFileBase logo = Request.Files["logo"];

            if ((logo != null) && (logo.ContentLength > 0) && !string.IsNullOrWhiteSpace(logo.FileName))
            {
                byte[] logoBytes = new byte[logo.ContentLength];
                logo.InputStream.Read(logoBytes, 0, Convert.ToInt32(logo.ContentLength));
                c.Logo = logoBytes;
            }
            
            
            if (flag)
            {
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

                    HttpResponseMessage Res = await client.PutAsJsonAsync("api/companiesAPI/", c);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var CompanyResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(CompanyResponse);

                    }
                }
                
                
                return RedirectToAction("../Company/Details/"+Session["user_id"]);
                

            }
            catch
            {
                return View(err);
            }
        }
        
    }
}
