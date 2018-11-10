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
    public class StudentController : Controller
    {
        string Baseurl = "http://localhost:42640";
        private EnvisionEntities db = new EnvisionEntities();
        

        // GET: Student/Details/5
        public async Task<ActionResult> Details(string id)
        {
            StudentView student = new StudentView();
            IEnumerable<SkillsList> sl = db.SkillsLists;
            IEnumerable<DegreeList> dl = db.DegreeLists;
            ViewBag.Degree = dl;
            ViewBag.Skills = sl;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 

                    HttpResponseMessage Res = await client.GetAsync("api/studentsAPI/"+id);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        student = JsonConvert.DeserializeObject<StudentView>(StudentResponse);

                    }
                    else
                    {
                        ViewBag.ResError = "No Such Student Found";
                    }
                }
                
                return View(student);
            }
            catch
            {
                student = null;
                return View(student);   
            }
        }

        
        public async Task<ActionResult> Projects()
        {
            if (Session["user_type"].ToString().Equals("GST") || Session["user_type"].ToString().Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }

            List<ProjectView> ProjectInfo = new List<ProjectView>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient 
                    var str = Session["user_id"];

                    HttpResponseMessage Res = await client.GetAsync("api/projectsAPI?userid="+Session["user_id"]);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        ProjectInfo = JsonConvert.DeserializeObject<List<ProjectView>>(StudentResponse);

                    }
                    else
                    {
                        ViewBag.ResError = "No such projects to display";
                    }
                }

                return View(ProjectInfo);
            }
            catch
            {
                ViewBag.ResError = "No such projects to display";
                ProjectInfo = null;
                return View(ProjectInfo);
            }
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            if (Session["user_type"].Equals("STD") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }
            StudentRegErr err = new StudentRegErr();
            return View(err);
        }

        // POST: Student/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            if (Session["user_type"].Equals("STD") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }
            bool flag = false;
            StudentRegErr err = new StudentRegErr();

            Student s = new Student();
            s.FirstName = Request.Form["fname"];
            s.LastName = Request.Form["lname"];
            s.Email = Request.Form["email"];
            s.Password = Request.Form["password"];
            s.Gender = Request.Form["gender"];

            HttpPostedFileBase cardfront = Request.Files["cardfront"];

            if ((cardfront != null) && (cardfront.ContentLength > 0) && !string.IsNullOrWhiteSpace(cardfront.FileName))
            {
                byte[] cardfrontBytes = new byte[cardfront.ContentLength];
                cardfront.InputStream.Read(cardfrontBytes, 0, Convert.ToInt32(cardfront.ContentLength));
                s.CardFront = cardfrontBytes;
            }
            else
            {
                flag = true;
                err.CardFront = "You must upload a pic of the FRONT of your id card as a .jpg or a .png file";
            }

            HttpPostedFileBase cardback = Request.Files["cardback"];

            if ((cardback != null) && (cardback.ContentLength > 0) && !string.IsNullOrWhiteSpace(cardback.FileName))
            {
                byte[] cardbackBytes = new byte[cardback.ContentLength];
                cardback.InputStream.Read(cardbackBytes, 0, Convert.ToInt32(cardback.ContentLength));
                s.CardBack = cardbackBytes;
            }
            else
            {
                flag = true;
                err.CardBack = "You must upload a pic of the BACK of your id card as a .jpg or a .png file";
            }

            
            
            err.s = s;

            
            DateTime result;
            if (string.IsNullOrWhiteSpace(Request.Form["dob"]) || !DateTime.TryParse(Request.Form["dob"], out result))
            {
                flag = true;
                err.DOB = "Date of Birth Cannot be Empty";
            }
            else
            {
                s.DOB = Convert.ToDateTime(Request.Form["dob"]);
            }

            if (string.IsNullOrWhiteSpace(s.Email) || !Regex.IsMatch(s.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                flag = true;
                err.Email= "Email Cannot be empty. It should be a valid email address.";
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["mobile"]))
            {
                s.Mobile = Request.Form["mobile"];
            }

            if (string.IsNullOrWhiteSpace(s.FirstName))
            {
                flag = true;
                err.FirstName = "First Name Cannot be Empty";
            }
            if (string.IsNullOrWhiteSpace(s.LastName))
            {
                flag = true;
                err.LastName = "Last Name Cannot be Empty";
            }
            if (string.IsNullOrWhiteSpace(s.Password))
            {
                flag = true;
                err.Password = "Password Cannot be Empty";
            }
            if (!s.Password.Equals(Request.Form["repassword"]))
            {
                flag = true;
                err.RePassword = "Both Passwords do not match";
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

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/studentsAPI/", s);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                if(response.Equals("email exists"))
                {
                    err.Email = "This Email Id Already Exists";
                    return View(err);
                }
                else
                {
                    Session["user_type"] = "STD";
                    Session["user_id"] = response;
                    return RedirectToAction("../Home/Index");
                }
                
            }
            catch
            {
                return View(err);
            }
        }
        
        // POST: Student/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("/Home/Index");
            }

            if (!Session["user_type"].Equals("ADM"))
            {
                if(string.IsNullOrWhiteSpace(Request.Form["id"]) || !Session["user_id"].ToString().Equals(Request.Form["id"]))
                {
                    return RedirectToAction("/Home/Index");
                }
            }

            Student s = new Student();
            s.Id = Request.Form["id"];

            HttpPostedFileBase profilepic = Request.Files["profilepic"];

            if ((profilepic != null) && (profilepic.ContentLength > 0) && !string.IsNullOrWhiteSpace(profilepic.FileName))
            {
                byte[] profilepicBytes = new byte[profilepic.ContentLength];
                profilepic.InputStream.Read(profilepicBytes, 0, Convert.ToInt32(profilepic.ContentLength));
                s.ProfilePic = profilepicBytes;
            }

            DateTime result;
            if (!string.IsNullOrWhiteSpace(Request.Form["dob"]) && DateTime.TryParse(Request.Form["dob"], out result))
            {
                s.DOB = Convert.ToDateTime(Request.Form["dob"]);
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["email"]) && Regex.IsMatch(Request.Form["email"], @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                s.Email = Request.Form["email"];
            }

            s.Mobile = Request.Form["mobile"];

            if (!string.IsNullOrWhiteSpace(Request.Form["fname"]))
            {
                s.FirstName = Request.Form["fname"];
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["lname"]))
            {
                s.LastName = Request.Form["lname"];
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

                    HttpResponseMessage Res = await client.PutAsJsonAsync("api/studentsAPI/", s);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var StudentResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(StudentResponse);

                    }
                }

                return RedirectToAction("/Details/"+s.Id);
            }
            catch
            {
                return View();
            }
        }

        
    }
}
