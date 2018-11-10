using System;
using System.Linq;
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
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace Envision.Controllers
{
    [LogFilter]
    public class AdminController : Controller
    {
        private EnvisionEntities db = new EnvisionEntities();
        string Baseurl = "http://localhost:42640";

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            string[] words;
            string[] password = { "mercury", "earth" ,"jupiter", "venus"};
            words = Request.Form["list"].ToString().Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
            foreach(var l in words)
            {
                System.Diagnostics.Debug.WriteLine(l);
            }

            bool areEqual = words.SequenceEqual(password);
            if (areEqual)
            {
                Session["user_type"] = "ADM";
                Session["user_id"] = "Admin";
                return RedirectToAction("/Dashboard");
            }
            return RedirectToAction("/Index");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("/Index");
        }

        public async Task<ActionResult> ReadMsg(int id)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/ReadMsg?id="+id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ContactResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    string response = JsonConvert.DeserializeObject<string>(ContactResponse);

                }
            }
                return RedirectToAction("../Admin/Messages/");
        }
        public async Task<ActionResult> Messages()
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            List<Contact> ContactInfo = new List<Contact>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/Messages");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ContactResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    ContactInfo = JsonConvert.DeserializeObject<List<Contact>>(ContactResponse);

                }
                return View(ContactInfo);
            }
        }
        public async Task<ActionResult> Dashboard()
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            int CountSessions = ((int)HttpContext.Application["SessionsCount"]) - 1;

            ADashboard DashboardInfo = new ADashboard();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/Dashboard");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DashboardResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    DashboardInfo = JsonConvert.DeserializeObject<ADashboard>(DashboardResponse);

                }
                DashboardInfo.CountSessions = CountSessions;
                
                return View(DashboardInfo);
            }
        }

        
        
        [HttpPost]
        [ActionName("Dashboard")]
        public async Task<ActionResult> DashboardPost()
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            int CountSessions = ((int)HttpContext.Application["SessionsCount"]) - 1;

            ADashboard DashboardInfo = new ADashboard();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/Dashboard");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DashboardResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    DashboardInfo = JsonConvert.DeserializeObject<ADashboard>(DashboardResponse);

                }
                DashboardInfo.CountSessions = CountSessions;

            }

            
            if (string.IsNullOrWhiteSpace(Request.Form["query"]))
            {
                DashboardInfo.Error = "Empty Query";
                return View(DashboardInfo);
            }
            else
            {
                DashboardInfo.Query = Request.Form["query"];
            }
            if (!DashboardInfo.Query.ToUpper().StartsWith(Request.Form["type"]))
            {
                DashboardInfo.Error = "QUERY NOT OF PROPER TYPE";
                return View(DashboardInfo);
            }

            string constring = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);
            SqlCommand cmd = new SqlCommand(DashboardInfo.Query, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            if (Request.Form["type"].Equals("SELECT"))
            {
                try
                {
                    con.Open();
                    adapter.Fill(ds, "MyTable");
                    con.Close();

                    DashboardInfo.data = ds.Tables["MyTable"];
                }
                catch (Exception e)
                {
                    DashboardInfo.Error = e.Message;
                }
            }
            else
            {
                try
                {
                    con.Open();
                    DashboardInfo.Msg = (cmd.ExecuteNonQuery()).ToString();
                    con.Close();
                }
                catch (Exception e)
                {
                    DashboardInfo.Error = e.Message;
                }
            }
            AdminQuery aq = new AdminQuery();
            aq.Query = DashboardInfo.Query;
            aq.Time = System.DateTime.Now;
            db.AdminQueries.Add(aq);

            db.SaveChanges();
            return View(DashboardInfo);
        }

        public async Task<ActionResult> Companies()
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            List<CompanyView> CompanyInfo = new List<CompanyView>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/companiesAPI/");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var CompanyResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    CompanyInfo = JsonConvert.DeserializeObject<List<CompanyView>>(CompanyResponse);

                }

                //returning the employee list to view  
                return View(CompanyInfo);
            }
        }
        public async Task<ActionResult> Students()
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            List<StudentView> StudentInfo = new List<StudentView>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/studentsAPI/");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    StudentInfo = JsonConvert.DeserializeObject<List<StudentView>>(StudentResponse);

                }

                //returning the employee list to view  
                return View(StudentInfo);
            }
            
        }
        public async Task<ActionResult> Internships()
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
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

                //returning the employee list to view  
                return View(InternshipInfo);
            }
        }
        public async Task<ActionResult> Projects()
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            List<ProjectView> ProjectInfo = new List<ProjectView>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/projectsAPI/");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    ProjectInfo = JsonConvert.DeserializeObject<List<ProjectView>>(ProjectResponse);

                }

                //returning the employee list to view  
                return View(ProjectInfo);
            }
        }


        public async Task<ActionResult> Company(string id)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            CompanyView CompanyInfo = new CompanyView();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/Company?companyid="+id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var CompanyResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    CompanyInfo = JsonConvert.DeserializeObject<CompanyView>(CompanyResponse);

                }

                //returning the employee list to view  
                return View(CompanyInfo);
            }
        }
        public async Task<ActionResult> Student(string id)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            StudentView StudentInfo = new StudentView();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/Student?studentid="+id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    StudentInfo = JsonConvert.DeserializeObject<StudentView>(StudentResponse);

                }
                ViewBag.Roles = db.RoleLists;
                //returning the employee list to view  
                return View(StudentInfo);
            }

        }


        public async Task<ActionResult> Internship(string id)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            InternshipView InternshipInfo = new InternshipView();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/Internship?internshipid="+id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var InternshipResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    InternshipInfo = JsonConvert.DeserializeObject<InternshipView>(InternshipResponse);

                }

                //returning the employee list to view  
                return View(InternshipInfo);
            }
        }
        public async Task<ActionResult> Project(string id)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            ProjectView ProjectInfo = new ProjectView();
            ViewBag.Roles = db.RoleLists;
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/Project?projectid="+id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    ProjectInfo = JsonConvert.DeserializeObject<ProjectView>(ProjectResponse);

                }

                //returning the employee list to view  
                return View(ProjectInfo);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DeleteStudent(string studentid)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            string response = "";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/DeleteStudent?studentid=" + studentid);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DashboardResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(DashboardResponse);

                }
                return RedirectToAction("../Admin/Students/");
            }
        }


        [HttpGet]
        public async Task<ActionResult> DeleteCompany(string companyid)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            string response = "";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/DeleteCompany?companyid=" + companyid);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DashboardResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(DashboardResponse);

                }
                return RedirectToAction("../Admin/Companies/");
            }
        }

        [HttpGet]
        public async Task<ActionResult> DeleteInternship(string internshipid)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            string response = "";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/DeleteInternship?internshipid=" + internshipid);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DashboardResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(DashboardResponse);

                }
                return RedirectToAction("../Admin/Internships/");
            }
        }

        [HttpGet]
        public async Task<ActionResult> DeleteProject(string projectid)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            string response = "";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/DeleteProject?projectid=" + projectid);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DashboardResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(DashboardResponse);

                }
                return RedirectToAction("../Admin/Projects/");
            }
        }


        [HttpGet]
        public async Task<ActionResult> VerifyStudent(string studentid)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            string response = "";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/VerifyStudent?studentid=" + studentid);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DashboardResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(DashboardResponse);

                }
                return RedirectToAction("../Admin/Students");
            }
        }

        [HttpGet]
        public async Task<ActionResult> VerifyCompany(string companyid)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            string response = "";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/adminAPI/VerifyCompany?companyid=" + companyid);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DashboardResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(DashboardResponse);

                }
                return RedirectToAction("../Admin/Companies/");
            }
        }

        [HttpPost]
        public ActionResult EditStudent(FormCollection collection)
        {
            if (!Session["user_type"].Equals("ADM"))
            {
                return RedirectToAction("../Home/Index");
            }
            Student s = db.Students.Find(Request.Form["id"]);
            HttpPostedFileBase cardfront = Request.Files["cardfront"];

            if ((cardfront != null) && (cardfront.ContentLength > 0) && !string.IsNullOrWhiteSpace(cardfront.FileName))
            {
                byte[] cardfrontBytes = new byte[cardfront.ContentLength];
                cardfront.InputStream.Read(cardfrontBytes, 0, Convert.ToInt32(cardfront.ContentLength));
                s.CardFront = cardfrontBytes;
                s.CardBack = cardfrontBytes;
            }


            db.Entry(s).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("../Admin/Students");
        }


    }
}