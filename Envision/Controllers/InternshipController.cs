using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Envision.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Envision.Filters;

namespace Envision.Controllers
{
    [LogFilter]
    public class InternshipController : Controller
    {
        private EnvisionEntities db = new EnvisionEntities();
        string Baseurl = "http://localhost:42640";

        public async Task<ActionResult> Apply(string studentid, string internshipid, string status)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }

            if (Session["user_type"].Equals("STD"))
            {
                studentid = Session["user_id"].ToString();
                status = "IP";
            }

            string response;
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/extraAPI/ApplyInternship?studentid="+studentid+"&internshipid="+internshipid+"&status="+status);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(Response);

                }
            }
            return RedirectToAction("../Company/Applicants/" + internshipid);
        }

        // GET: Internship
        public async Task<ActionResult> Index()
        {
            List<InternshipView> InternshipInfo = new List<InternshipView>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/internshipsAPI/");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var InternshipResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    InternshipInfo = JsonConvert.DeserializeObject<List<InternshipView>>(InternshipResponse);

                }

                InternshipInfo = InternshipInfo.Where(item => item.AvailableSeats > 0).ToList<InternshipView>();
                IEnumerable<LocationsList> ll = db.LocationsLists;
                IEnumerable<CategoriesList> cl = db.CategoriesLists;
                ViewBag.Location = ll;
                ViewBag.Categories = cl;
                ViewBag.Filter = new InternshipFilter();
                //returning the employee list to view  
                return View(InternshipInfo);
            }
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<ActionResult> IndexPost(InternshipFilter f)
        {
            List<InternshipView> InternshipInfo = new List<InternshipView>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/internshipsAPI/");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var InternshipResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    InternshipInfo = JsonConvert.DeserializeObject<List<InternshipView>>(InternshipResponse);

                }

                if (f.Category != "0")
                {
                    InternshipInfo = InternshipInfo.Where(c => c.InternshipIn == f.Category).ToList<InternshipView>();
                }
                if (f.Location != "0")
                {
                    InternshipInfo = InternshipInfo.Where(c => c.InternshipLocations.Contains(f.Location)).ToList<InternshipView>();
                }

                var tempNGO = InternshipInfo.Where(c => c.CompanyType=="NGO");
                var tempGOV = InternshipInfo.Where(c => c.CompanyType == "GOV");
                var tempSTP = InternshipInfo.Where(c => c.CompanyType == "STP");
                var tempEST = InternshipInfo.Where(c => c.CompanyType == "EST");

                var result = new List<InternshipView>();

                if (f.StartUp != null)
                {
                    result = result.Union(tempSTP).ToList<InternshipView>();
                }
                if (f.Established != null)
                {
                    result = result.Union(tempEST).ToList<InternshipView>();
                }
                if (f.Ngo != null)
                {
                    result = result.Union(tempNGO).ToList<InternshipView>();
                }
                if (f.Gov != null)
                {
                    result = result.Union(tempGOV).ToList<InternshipView>();
                }
                if(f.StartUp == null && f.Gov == null && f.Ngo == null && f.Established == null)
                {
                    result = InternshipInfo;
                }
                if(f.MaxDuration != null)
                {
                    result = result.Where(c => c.MaxDuration <= Convert.ToInt32(f.MaxDuration)).ToList<InternshipView>();
                }
                if(f.StartMin != null)
                {
                    var date = Convert.ToDateTime(f.StartMin);
                    result = result.Where(c => c.StartDate>=date).ToList<InternshipView>();
                }
                if(f.StartMax != null)
                {
                    var date = Convert.ToDateTime(f.StartMax);
                    result = result.Where(c => c.StartDate <= date).ToList<InternshipView>();
                }

                result = result.Where(item => item.AvailableSeats > 0).OrderBy(p => p.ApplyBefore).ToList<InternshipView>();
                IEnumerable<LocationsList> ll = db.LocationsLists;
                IEnumerable<CategoriesList> cl = db.CategoriesLists;
                ViewBag.Filter = f;
                ViewBag.Location = ll;
                ViewBag.Categories = cl;
                return View(result);
            }
        }
        // GET: Internship/Details/5
        
        public async Task<ActionResult> Details(string id)
        {
            InternshipView InternshipInfo = new InternshipView();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string str = "api/internshipsAPI/"+ id;
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync(str);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var InternshipResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    InternshipInfo = JsonConvert.DeserializeObject<InternshipView>(InternshipResponse);

                }
            }
                return View(InternshipInfo);
        }

        
        // GET: Internship/Create
        public ActionResult Create()
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("STD"))
            {
                return RedirectToAction("../Home/Index");
            }

            ViewBag.Categories = db.CategoriesLists;
            ViewBag.Locations = db.LocationsLists;
            ViewBag.Perks = db.PerksLists;
            ViewBag.Skills = db.SkillsLists;
            InternshipRegErr err = new InternshipRegErr();
            return View(err);
        }

        // POST: Internship/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("STD"))
            {
                return RedirectToAction("../Home/Index");
            }

            bool flag = false;
            InternshipRegErr err = new InternshipRegErr();
            ViewBag.Categories = db.CategoriesLists;
            ViewBag.Locations = db.LocationsLists;
            ViewBag.Perks = db.PerksLists;
            ViewBag.Skills = db.SkillsLists;

            Internship i = new Internship();
            i.About = Request.Form["about"];
            i.ContactEmail = Request.Form["contactemail"];
            i.ContactMobile = Request.Form["contactmobile"];
            i.C_Id = Session["user_id"].ToString();
            i.InternshipType = Request.Form["type"];

            DateTime result;
            if (string.IsNullOrWhiteSpace(i.About))
            {
                flag = true;
                err.About = "You must write details about the internship";
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["startdate"]))
            {
                if (DateTime.TryParse(Request.Form["startdate"], out result))
                {
                    i.StartDate = Convert.ToDateTime(Request.Form["startdate"]);
                }
                else
                {
                    flag = true;
                    err.StartDate = "Start date not in proper format";
                }
            }

            int value;
            if (string.IsNullOrWhiteSpace(Request.Form["minduration"]) || !Int32.TryParse(Request.Form["minduration"], out value))
            {
                flag = true;
                err.MinDuration = "Minimum Duration cannot be Empty";
            }
            else
            {
                i.MinDuration = Convert.ToInt32(Request.Form["minduration"]);
                if(i.MinDuration <= 0)
                {
                    flag = true;
                    err.MinDuration = "Minimum Duration should be atleast 1 month";
                }
            }

            if (string.IsNullOrWhiteSpace(Request.Form["maxduration"]) || !Int32.TryParse(Request.Form["maxduration"], out value))
            {
                flag = true;
                err.MaxDuration = "Maximum Dauration cannot be Empty";
            }
            else
            {
                i.MaxDuration = Convert.ToInt32(Request.Form["maxduration"]);
                if (i.MaxDuration <= 0)
                {
                    flag = true;
                    err.MaxDuration = "Maximum Duration should be atleast 1 month";
                }
            }

            if (string.IsNullOrWhiteSpace(Request.Form["seats"]) || !Int32.TryParse(Request.Form["seats"], out value))
            {
                flag = true;
                err.AvailableSeats = "Cannot be empty";
            }
            else
            {
                i.AvailableSeats = Convert.ToInt32(Request.Form["seats"]);
                if (i.AvailableSeats <= 0)
                {
                    flag = true;
                    err.AvailableSeats = "Value should be atleast 1 seat";
                }
            }

            if (string.IsNullOrWhiteSpace(Request.Form["applybefore"]) || !DateTime.TryParse(Request.Form["applybefore"], out result))
            {
                flag = true;
                err.ApplyBefore = "Apply Before Date cannot be Empty";
            }
            else
            {
                i.ApplyBefore = Convert.ToDateTime(Request.Form["applybefore"]);
            }

            if (string.IsNullOrWhiteSpace(i.ContactEmail) || !Regex.IsMatch(i.ContactEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                flag = true;
                err.ContactEmail = "Email Cannot be empty. It should be a valid email address.";
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["contactmobile"]))
            {
                i.ContactMobile = Request.Form["contactmobile"];
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["locations"]))
            {
                foreach (string s in Request.Form["locations"].Split(','))
                {
                    InternshipLocation templ = new InternshipLocation();
                    templ.L_Id = Convert.ToInt32(s);
                    i.InternshipLocations.Add(templ);
                }
            }
            else
            {
                flag = true;
                err.Locations = "You should select atleast one location";
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["skills"]))
            {
                foreach (string s in Request.Form["skills"].Split(','))
                {
                    InternshipSkill temps = new InternshipSkill();
                    temps.S_Id = Convert.ToInt32(s);
                    i.InternshipSkills.Add(temps);
                }
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["perks"]))
            {
                foreach (string s in Request.Form["perks"].Split(','))
                {
                    InternshipPerk tempp = new InternshipPerk();
                    tempp.P_Id = Convert.ToInt32(s);
                    i.InternshipPerks.Add(tempp);
                }
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["category"]))
            {
                i.InternshipIn = Convert.ToInt32(Request.Form["category"]);
            }
            else
            {
                flag = true;
                err.Category = "You need to select A Category";
            }

            if (flag)
            {
                err.i = i;
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

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/internshipsAPI/", i);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                if (response.Equals("exists"))
                {
                    err.Error = "Such an Internship already exists";
                    err.i = i;
                    return View(err);
                }
                else
                {
                    return RedirectToAction("../Company/Internships");
                }

            }
            catch
            {
                err.i = i;
                return View(err);
            }
        }

        
        // GET: Internship/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("STD"))
            {
                return RedirectToAction("../Home/Index");
            }

            string response;
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.DeleteAsync("api/internshipsAPI?id=" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var InternshipResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(InternshipResponse);

                }
            }
            return RedirectToAction("../Company/Internships");
        }
        
    }
}
