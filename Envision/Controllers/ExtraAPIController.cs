using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Envision.Models;

namespace Envision.Controllers
{
    
    public class ExtraAPIController : ApiController
    {
        private EnvisionEntities db = new EnvisionEntities();

        // GET: api/StudentsAPI
        [HttpPost]
        [Route("api/extraAPI/Skill")]
        public IHttpActionResult AddSkill(SkilledIn skill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.SkilledIns.Add(skill);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Ok("error");
            }

            return Ok("success");
        }

        [HttpPost]
        [Route("api/extraAPI/Work")]
        public IHttpActionResult AddWork(WorkExperience work)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WorkExperiences.Add(work);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Ok("error");
            }

            return Ok("success");
        }

        [HttpPost]
        [Route("api/extraAPI/Volunteer")]
        public IHttpActionResult AddVolunteer(VolunteerWork volunteer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            db.VolunteerWorks.Add(volunteer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Ok("error");
            }

            return Ok("success");
        }

        [HttpPost]
        [Route("api/extraAPI/Education")]
        public IHttpActionResult AddEducation(Studied education)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            db.Studieds.Add(education);
        
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Ok("error");
            }
            
            return Ok("success");
        }

        [HttpDelete]
        [Route("api/extraAPI/Skill")]
        public IHttpActionResult DeleteSkill(string userid,int skillid)
        {
            SkilledIn skill = db.SkilledIns.Where(item => item.U_Id == userid && item.S_ID == skillid).FirstOrDefault();
            if (skill == null)
            {
                return NotFound();
            }

            db.SkilledIns.Remove(skill);
            db.SaveChanges();

            return Ok("success");
        }

        [HttpDelete]
        [Route("api/extraAPI/Education")]
        public IHttpActionResult DeleteEducation(string userid, string name, string degree, string graduation)
        {
            Studied stud;
            if (graduation.Equals("null"))
            {
                stud = db.Studieds.Where(item => item.S_Id == userid && item.Name == name && item.DegreeList.Name == degree && item.GraduationYear == null).FirstOrDefault();
            }
            else
            {
                Decimal grad = Convert.ToDecimal(graduation);
                stud = db.Studieds.Where(item => item.S_Id == userid && item.Name == name && item.DegreeList.Name == degree && item.GraduationYear == grad).FirstOrDefault();
            }
            
            if (stud == null)
            {
                return NotFound();
            }

            db.Studieds.Remove(stud);
            db.SaveChanges();

            return Ok("success");
        }

        [HttpDelete]
        [Route("api/extraAPI/Work")]
        public IHttpActionResult DeleteWork(string userid, string name, string designation, string startdate)
        {
            WorkExperience work;
            
            DateTime sdate = Convert.ToDateTime(startdate);
            work = db.WorkExperiences.Where(item => item.S_Id == userid && item.Name == name && item.Designation == designation && item.StartDate == sdate).FirstOrDefault();
            

            if (work == null)
            {
                return NotFound();
            }

            db.WorkExperiences.Remove(work);
            db.SaveChanges();

            return Ok("success");
        }

        [HttpDelete]
        [Route("api/extraAPI/Volunteer")]
        public IHttpActionResult DeleteVolunteer(string userid, string name, string topic, string startdate)
        {
            VolunteerWork work;

            DateTime sdate = Convert.ToDateTime(startdate);
            work = db.VolunteerWorks.Where(item => item.S_Id == userid && item.Name == name && item.Topic == topic && item.StartDate == sdate).FirstOrDefault();


            if (work == null)
            {
                return NotFound();
            }

            db.VolunteerWorks.Remove(work);
            db.SaveChanges();

            return Ok("success");
        }

        [HttpGet]
        [Route("api/extraAPI/ApplyInternship")]
        public IHttpActionResult ApplyInternship(string studentid,string internshipid,string status)
        {
            Applied app = db.Applieds.Where(item => item.I_Id == internshipid && item.S_Id == studentid).FirstOrDefault();
            if (app == null)
            {
                app = new Applied();
                app.S_Id = studentid;
                app.I_Id = internshipid;
                app.Status = "IP";
                db.Applieds.Add(app);
            }
            else
            {
                app.Status = status;
                if (status.Equals("AP"))
                {
                    app.Internship.AvailableSeats = app.Internship.AvailableSeats - 1;
                }
                db.Entry(app).State = EntityState.Modified;
            }
            
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Ok("error");
            }
            

            return Ok("success");
        }

        [HttpGet]
        [Route("api/extraAPI/JoinProject")]
        public IHttpActionResult JoinProject(string studentid, string projectid, string status)
        {
            Join app = db.Joins.Where(item => item.P_Id == projectid && item.S_Id == studentid).FirstOrDefault();
            if (app == null)
            {
                app = new Join();
                app.S_Id = studentid;
                app.P_Id = projectid;
                app.Status = "IP";
                db.Joins.Add(app);
            }
            else
            {
                app.Status = status;
                db.Entry(app).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Ok("error");
            }


            return Ok("success");
        }

        [HttpGet]
        [Route("api/extraAPI/Applicants")]
        public IEnumerable<InternshipView> Applicants(string companyid)
        {
            return db.Internships.Include(c=>c.CategoriesList).Where(item => item.C_Id.Equals(companyid)).Where(b=>b.AvailableSeats>0).Select(
                p => new InternshipView
                {
                    Id = p.Id,
                    InternshipIn = p.CategoriesList.Name,
                    ApplyBefore = p.ApplyBefore,
                    AvailableSeats = p.AvailableSeats,
                    stud = p.Applieds.Where(q=>q.Status!="AP").Select(
                            b => new InternshipView.Student
                            {
                               S_Id = b.S_Id,
                               S_Name = b.Student.FirstName + " " + b.Student.LastName,
                               Status = b.Status
                            }
                          ).ToList<InternshipView.Student>()
                }
            );
            
        }

        [HttpGet]
        [Route("api/extraAPI/Joins")]
        public ProjectView Joins(string projectid)
        {
            return db.Projects.Where(item => item.Id == projectid).Select(
                p => new ProjectView
                {
                    Id = p.Id,
                    CreatedById = p.CreatedBy,
                    ProjectJoins = (p.Joins.ToList()).Select(c => new ProjectView.Join
                    {
                        S_Id = c.S_Id,
                        Name = c.Student.FirstName + " " + c.Student.LastName,
                        Status = c.Status
                    }).ToList()
                }
               ).First();
        }

        [HttpGet]
        [Route("api/extraAPI/Users")]
        public ProjectView Users(string projectid)
        {
            return db.Projects.Where(item => item.Id == projectid).Select(
                p => new ProjectView
                {
                    Id = p.Id,
                    CreatedById = p.CreatedBy,
                    ProjectUsers = (p.ProjectUsers.ToList()).Select(c => new ProjectView.User
                    {
                        UserId = c.U_Id,
                        Pic = c.Student.ProfilePic,
                        UserName = c.Student.FirstName + " " + c.Student.LastName,
                        Role = c.RoleList.Name
                    }).ToList()
                }
               ).First();
        }

        [HttpGet]
        [Route("api/extraAPI/Tasks")]
        public ProjectView Tasks(string projectid)
        {
            return db.Projects.Where(item => item.Id == projectid).Select(
                p => new ProjectView
                {
                    Id = p.Id,
                    CreatedById = p.CreatedBy,
                    ProjectUsers = (p.ProjectUsers.ToList()).Select(c => new ProjectView.User
                    {
                        UserId = c.Id,
                        Pic = c.Student.ProfilePic,
                        UserName = c.Student.FirstName + " " + c.Student.LastName,
                        Role = c.RoleList.Name
                    }).ToList(),
                    ProjectTasks = (p.ProjectTasks.ToList()).Select(c => new ProjectView.Tasks
                    {
                        Id = c.Id,
                        Heading = c.TasksList.Name,
                        Description = c.Description,
                        DeadLine = c.DeadLine,
                        Status = c.Status,
                        UserTasks = (c.TasksAssigneds.ToList()).Select(u => new ProjectView.Tasks.User
                        {
                            Pic = u.ProjectUser.Student.ProfilePic,
                            Name = u.ProjectUser.Student.FirstName + " " + u.ProjectUser.Student.LastName,
                            UserId = u.ProjectUser.U_Id
                        }).ToList(),
                    }).ToList()
                }
               ).First();
        }

        [HttpPost]
        [Route("api/extraAPI/JoinApply")]
        public IHttpActionResult JoinApply(ProjectUser u)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!db.ProjectUsers.Any(item => item.U_Id == u.U_Id && item.P_Id == u.P_Id))
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
                Boolean flag = true;
                Random random = new Random();
                while (flag)
                {
                    u.Id = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
                    if (!db.ProjectUsers.Any(item => item.Id == u.Id))
                    {
                        flag = false;
                    }
                }

                db.ProjectUsers.Add(u);
                Join j = db.Joins.Where(item => item.P_Id == u.P_Id && item.S_Id == u.U_Id).First();
                db.Joins.Remove(j);
            }
            else
            {
                ProjectUser pu = db.ProjectUsers.Where(item => item.U_Id == u.U_Id && item.P_Id == u.P_Id).First();
                pu.R_Id = u.R_Id;
                db.Entry(pu).State = EntityState.Modified;
            }
            

            db.SaveChanges();

            return Ok("success");
        }

        [HttpDelete]
        [Route("api/extraAPI/DeleteJoin")]
        public IHttpActionResult DeleteJoin(string projectid, string userid)
        {
            ProjectUser pu = db.ProjectUsers.Where(item => item.P_Id == projectid && item.U_Id == userid).First();
            if(db.TasksAssigneds.Any(item=>item.PU_Id == pu.Id))
            {
                foreach(TasksAssigned t in db.TasksAssigneds.Where(j=>j.PU_Id == pu.Id))
                {
                    db.TasksAssigneds.Remove(t);
                }
            }
            db.ProjectUsers.Remove(pu);
            db.SaveChanges();
            return Ok("success");
        }

        [HttpPut]
        [Route("api/extraAPI/UpdateTask")]
        public IHttpActionResult UpdateTask(ProjectTask pt)
        {
            if (pt.TasksAssigneds != null)
            {
                foreach(TasksAssigned temp1 in db.TasksAssigneds.Where(item => item.PT_Id == pt.Id))
                {
                    db.TasksAssigneds.Remove(temp1);
                }
                foreach(TasksAssigned temp2 in pt.TasksAssigneds)
                {
                    db.TasksAssigneds.Add(temp2);
                }
            }

            ProjectTask temp = db.ProjectTasks.Where(item => item.Id == pt.Id).First();
            if (!string.IsNullOrWhiteSpace(pt.Description))
            {
                temp.Description = pt.Description;
            }
            if(pt.DeadLine != null)
            {
                temp.DeadLine = pt.DeadLine;
            }
            temp.Status = pt.Status;

            db.Entry(temp).State = EntityState.Modified;
            db.SaveChanges();
            return Ok("success");
        }


        [HttpPost]
        [Route("api/extraAPI/AddTask")]
        public IHttpActionResult AddTask(ProjectTask pt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            Boolean flag = true;
            Random random = new Random();
            while (flag)
            {
               pt.Id = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
               if (!db.ProjectTasks.Any(item => item.Id == pt.Id))
               {
                   flag = false;
               }
            }

            db.ProjectTasks.Add(pt);
            try{
                db.SaveChanges();
            }
            catch
            {

            }
            return Ok("success");
        }


        [HttpPost]
        [Route("api/extraAPI/Contact")]
        public IHttpActionResult Contact(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            contact.Time = System.DateTime.Now;

            db.Contacts.Add(contact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Ok("error");
            }

            return Ok("success");
        }

    }
}
