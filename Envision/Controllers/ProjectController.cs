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
using Envision.Filters;

namespace Envision.Controllers
{
    [LogFilter]
    public class ProjectController : Controller
    {
        private EnvisionEntities db = new EnvisionEntities();
        string Baseurl = "http://localhost:42640";

        
        public async Task<ActionResult> Join(string studentid, string projectid, string status)
        {
            if (Session["user_type"].Equals("CMP"))
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
                HttpResponseMessage Res = await client.GetAsync("api/extraAPI/JoinProject?studentid=" + studentid + "&projectid=" + projectid + "&status=" + status);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(Response);

                }
            }
            return RedirectToAction("../Project/Details/" + projectid);
        }

        // GET: Project
        public async Task<ActionResult> Index(string category)
        {
            List<ProjectView> ProjectInfo = new List<ProjectView>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res;

                if (category==null || category.Equals("All"))
                {
                    Res = await client.GetAsync("api/projectsAPI/");
                }
                else
                {
                    Res = await client.GetAsync("api/projectsAPI?category="+category);
                }
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    ProjectInfo = JsonConvert.DeserializeObject<List<ProjectView>>(ProjectResponse);

                }

                
                IEnumerable<string> cl = db.CategoriesLists.Select(c=>c.Name) ;
                if (category == null)
                {
                    category = "All";
                }
                ViewBag.Category = category;
                ViewBag.Categories = cl;
                //returning the employee list to view  
                return View(ProjectInfo);
            }


        }
        

        // GET: Project/Details/5
        public async Task<ActionResult> Details(string id)
        {
            ProjectView ProjectInfo = new ProjectView();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/projectsAPI/"+id);
                
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
        public async Task<ActionResult> Joins(string id)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }
            if (!Session["user_type"].Equals("ADM"))
            {
                if (!Session["user_id"].Equals(db.Projects.Where(p => p.Id.Equals(id)).Select(c => c.CreatedBy).FirstOrDefault()))
                {
                    return RedirectToAction("../Home/Index");
                }
            }
                

            ProjectView ProjectInfo = new ProjectView();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/extraAPI/Joins?projectid=" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    ProjectInfo = JsonConvert.DeserializeObject<ProjectView>(ProjectResponse);

                }

                ViewBag.Roles = db.RoleLists;
                //returning the employee list to view  
                return View(ProjectInfo);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Users(string id)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }
            if (!Session["user_type"].Equals("ADM"))
            {
                if (!Session["user_id"].Equals(db.Projects.Where(p => p.Id.Equals(id)).Select(c => c.CreatedBy).FirstOrDefault()))
                {
                    return RedirectToAction("../Home/Index");
                }
            }
            

            ProjectView ProjectInfo = new ProjectView();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/extraAPI/Users?projectid=" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    ProjectInfo = JsonConvert.DeserializeObject<ProjectView>(ProjectResponse);

                }

                ViewBag.Roles = db.RoleLists;
                //returning the employee list to view  
                return View(ProjectInfo);
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> Tasks(string id)
        {
            ViewBag.Tasks = db.TasksLists;
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }

            ProjectView ProjectInfo = new ProjectView();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/extraAPI/Tasks?projectid=" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    ProjectInfo = JsonConvert.DeserializeObject<ProjectView>(ProjectResponse);

                }

                
                ViewBag.Roles = db.RoleLists;
                //returning the employee list to view  
                return View(ProjectInfo);
            }
        }


        // GET: Project/Create
        public ActionResult Create()
        {
            if(Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }
            ProjectRegErr err = new ProjectRegErr();
            ViewBag.Categories = db.CategoriesLists;
            
            return View(err);
        }

        // POST: Project/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }

            bool flag = false;
            ProjectRegErr err = new ProjectRegErr();
            ViewBag.Categories = db.CategoriesLists;

            Project p = new Project();
            p.Name = Request.Form["name"];
            p.ProjectOverview = Request.Form["projectoverview"];
            p.ProjectHelp = Request.Form["projecthelp"];
            p.TeamLooking = Request.Form["teamlooking"];
            p.CreatedBy = Session["user_id"].ToString();


            if (string.IsNullOrWhiteSpace(Request.Form["category"]))
            {
                flag = true;
                err.Category = "You must select the category in which project belongs";
            }
            else
            {
                p.C_Id = Convert.ToInt32(Request.Form["category"]); 
            }
            HttpPostedFileBase pic = Request.Files["pic"];

            if ((pic != null) && (pic.ContentLength > 0) && !string.IsNullOrWhiteSpace(pic.FileName))
            {
                byte[] picBytes = new byte[pic.ContentLength];
                pic.InputStream.Read(picBytes, 0, Convert.ToInt32(pic.ContentLength));
                p.Pic = picBytes;
            }
            else
            {
                flag = true;
                err.Pic = "You must upload a pic related to the project as a .jpg or a .png file";
            }

            if (string.IsNullOrWhiteSpace(p.Name))
            {
                flag = true;
                err.Name = "You must give the project a name";
            }
            if (string.IsNullOrWhiteSpace(p.ProjectHelp))
            {
                flag = true;
                err.ProjectHelp = "You must give a description on where the project needs help";
            }
            if (string.IsNullOrWhiteSpace(p.ProjectOverview))
            {
                flag = true;
                err.ProjectOverview = "You must give a detailed overview of the project and the work do be done in it";
            }
            if (string.IsNullOrWhiteSpace(p.TeamLooking))
            {
                flag = true;
                err.TeamLooking = "You must give a description on what kind of team are you looking for";
            }

            if (flag)
            {
                err.p = p;
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

                    HttpResponseMessage Res = await client.PostAsJsonAsync("api/projectsAPI/", p);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var Response = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(Response);

                    }
                }

                if (response.Equals("exists"))
                {
                    err.p = p;
                    return View(err);
                }
                else
                {
                    return RedirectToAction("../Student/Projects");
                }

            }
            catch
            {
                err.p = p;
                return View(err);
            }
        }

        
        // POST: Project/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection collection)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }
            if (!Session["user_type"].Equals("ADM"))
            {
                if (string.IsNullOrWhiteSpace(Request.Form["id"]) || !Session["user_id"].Equals(db.Projects.Where(b => b.Id.Equals(Request.Form["id"])).Select(c => c.CreatedBy).FirstOrDefault()))
                {
                    return RedirectToAction("../Home/Index");
                }
            }
            

            Project p = new Project();
            p.Id = Request.Form["id"];
            p.ProjectOverview = Request.Form["projectoverview"];
            p.ProjectHelp = Request.Form["projecthelp"];
            p.TeamLooking = Request.Form["teamlooking"];

            HttpPostedFileBase pic = Request.Files["pic"];

            if ((pic != null) && (pic.ContentLength > 0) && !string.IsNullOrWhiteSpace(pic.FileName))
            {
                byte[] picBytes = new byte[pic.ContentLength];
                pic.InputStream.Read(picBytes, 0, Convert.ToInt32(pic.ContentLength));
                p.Pic = picBytes;
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

                    HttpResponseMessage Res = await client.PutAsJsonAsync("api/projectsAPI/", p);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var Response = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        response = JsonConvert.DeserializeObject<string>(Response);

                    }
                }

                return RedirectToAction("/Details/"+p.Id);
            }
            catch
            {
                return View();
            }
        }
        

        [HttpPost]
        public async Task<ActionResult> JoinApprove(FormCollection collection)
        {
            if(Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }
            if (!Session["user_type"].Equals("ADM"))
            {
                if (!Session["user_id"].Equals(db.Projects.Where(b => b.Id.Equals(Request.Form["projectid"])).Select(c => c.CreatedBy).FirstOrDefault()))
                {
                    return RedirectToAction("../Home/Index");
                }
            }

            ProjectUser p = new ProjectUser();
            p.U_Id = Request.Form["studentid"];
            p.P_Id = Request.Form["projectid"];
            p.R_Id = Convert.ToInt32(Request.Form["role"]);
            string response;
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.PostAsJsonAsync("api/extraAPI/JoinApply/", p);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(ProjectResponse);

                }


                ViewBag.Roles = db.RoleLists;
                //returning the employee list to view  
                return RedirectToAction("/Details/"+p.P_Id);
            }
        }

        public async Task<ActionResult> DeleteApprove(string projectid,string userid)
        {
            if (Session["user_type"].Equals("GST") || Session["user_type"].Equals("CMP"))
            {
                return RedirectToAction("../Home/Index");
            }
            if (!Session["user_type"].Equals("ADM"))
            {
                if (!Session["user_id"].Equals(db.Projects.Where(b => b.Id.Equals(Request.Form["projectid"])).Select(c => c.CreatedBy).FirstOrDefault()))
                {
                    return RedirectToAction("../Home/Index");
                }
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
                HttpResponseMessage Res = await client.DeleteAsync("api/extraAPI/DeleteJoin?projectid="+projectid+"&userid="+userid);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(ProjectResponse);

                }


                ViewBag.Roles = db.RoleLists;
                //returning the employee list to view  
                return RedirectToAction("/Details/"+projectid);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTask(FormCollection collection)
        {
            ProjectTask p = new ProjectTask();
            List<TasksAssigned> t = new List<TasksAssigned>();
            p.Id = Request.Form["id"];
            p.Status = Request.Form["status"];
            if (!string.IsNullOrWhiteSpace(Request.Form["description"]))
            {
                p.Description = Request.Form["description"];
            }
            DateTime result;
            if (string.IsNullOrWhiteSpace(Request.Form["deadline"]) || !DateTime.TryParse(Request.Form["deadline"], out result))
            {
                return RedirectToAction("/Details/" + Request.Form["projectid"]);
            }
            else
            {
                p.DeadLine = Convert.ToDateTime(Request.Form["deadline"]);
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["users"]))
            {
                foreach (string s in Request.Form["users"].Split(','))
                {
                    TasksAssigned temp = new TasksAssigned();
                    temp.PT_Id = Request.Form["id"];
                    temp.PU_Id = s;
                    temp.Time = System.DateTime.Now;
                    p.TasksAssigneds.Add(temp);
                }
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
                HttpResponseMessage Res = await client.PutAsJsonAsync("api/extraAPI/UpdateTask/",p);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(ProjectResponse);

                }


                ViewBag.Roles = db.RoleLists;
                //returning the employee list to view  
                return RedirectToAction("/Details/" + Request.Form["projectid"]);
            }
        }


        [HttpPost]
        public async Task<ActionResult> AddTask(FormCollection collection)
        {
            ProjectTask p = new ProjectTask();
            p.Status = "PEN";
            if (!string.IsNullOrWhiteSpace(Request.Form["description"]))
            {
                p.Description = Request.Form["description"];
            }
            else
            {
                return RedirectToAction("/Details/" + Request.Form["projectid"]);
            }

            p.T_Id = Convert.ToInt32(Request.Form["task"]);

            DateTime result;
            if (string.IsNullOrWhiteSpace(Request.Form["deadline"]) || !DateTime.TryParse(Request.Form["deadline"], out result))
            {
                return RedirectToAction("/Details/" + Request.Form["projectid"]);
            }
            else
            {
                p.DeadLine = Convert.ToDateTime(Request.Form["deadline"]);
            }
            p.P_Id = Request.Form["projectid"];

            string response;
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.PostAsJsonAsync("api/extraAPI/AddTask/", p);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProjectResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    response = JsonConvert.DeserializeObject<string>(ProjectResponse);

                }


                ViewBag.Roles = db.RoleLists;
                //returning the employee list to view  
                return RedirectToAction("/Details/" + Request.Form["projectid"]);
            }
        }
    }
}
