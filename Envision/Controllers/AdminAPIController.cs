using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Envision.Models;

namespace Envision.Controllers
{
    public class AdminAPIController : ApiController
    {
        private EnvisionEntities db = new EnvisionEntities();

        [HttpGet]
        [Route("api/adminAPI/Student")]
        public IHttpActionResult GetStudent(string studentid)
        {
            

            StudentView student = db.Students.Include(b => b.Studieds).Include(b => b.SkilledIns).Include(b => b.VolunteerWorks).Include(b => b.WorkExperiences).Where(item => item.Id == studentid).Select(
                p => new StudentView
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    Mobile = p.Mobile,
                    DOB = p.DOB,
                    Gender = p.Gender,
                    ProfilePic = p.ProfilePic,
                    CardBack = p.CardBack,
                    CardFront = p.CardFront,
                    WorkExperiences = (p.WorkExperiences.ToList()).Select(w => new StudentView.WorkExperience
                    {
                        Name = w.Name,
                        StartDate = w.StartDate,
                        EndDate = w.EndDate,
                        Designation = w.Designation
                    }).Select(o => new { Obj = o, EndDate = o.EndDate ?? DateTime.MaxValue }).OrderByDescending(o => o.EndDate).Select(o => o.Obj).ToList(),
                    VolunteerWorks = (p.VolunteerWorks.ToList()).Select(v => new StudentView.VolunteerWork
                    {
                        Name = v.Name,
                        StartDate = v.StartDate,
                        EndDate = v.EndDate,
                        Topic = v.Topic
                    }).Select(o => new { Obj = o, EndDate = o.EndDate ?? DateTime.MaxValue }).OrderByDescending(o => o.EndDate).Select(o => o.Obj).ToList(),
                    SkilledIns = (p.SkilledIns.ToList()).Select(s => new StudentView.SkilledIn
                    {
                        Id = s.S_ID,
                        Name = s.SkillsList.Name,
                        Value = s.Value
                    }).ToList(),
                    Studieds = (p.Studieds.ToList()).Select(s => new StudentView.Studied
                    {
                        Degree = s.DegreeList.Name,
                        Name = s.Name,
                        GraduationYear = s.GraduationYear
                    }).Select(o => new { Obj = o, GraduationYear = o.GraduationYear ?? Decimal.MaxValue }).OrderByDescending(o => o.GraduationYear).Select(o => o.Obj).ToList(),
                    Applieds = (p.Applieds.ToList()).Select(s => new StudentView.InternshipApplied
                    {
                        I_Id = s.I_Id,
                        InternshipIn = s.Internship.CategoriesList.Name,
                        C_Id = s.Internship.C_Id,
                        C_Name = s.Internship.Company.Name,
                        Status = s.Status
                    }).ToList(),
                    ProjectJoins = (p.Joins.ToList()).Select(s => new StudentView.ProjectJoin
                    {
                        P_Id = s.P_Id,
                        Name = s.Project.Name,
                        CreatedBy = s.Project.Student.FirstName + " " + s.Project.Student.LastName,
                        Category = s.Project.CategoriesList.Name,
                        Status = s.Status
                    }).ToList(),
                    ProjectUsers = (p.ProjectUsers.ToList()).Select(s => new StudentView.ProjectUser
                    {
                        P_Id = s.P_Id,
                        Name = s.Project.Name,
                        CreatedBy = s.Project.Student.FirstName + " " + s.Project.Student.LastName,
                        Category = s.Project.CategoriesList.Name,
                        Role = s.RoleList.Name
                    }).ToList(),
                }

                ).FirstOrDefault();
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);

        }

        [HttpGet]
        [Route("api/adminAPI/Internship")]
        public IHttpActionResult GetInternship(string internshipid)
        {
            InternshipView internship = db.Internships.Include(b => b.Company).Include(b => b.CategoriesList).Include(b => b.InternshipLocations).Include(b => b.InternshipPerks).Include(b => b.InternshipSkills).Where(item => item.Id == internshipid).Select(
                       p => new InternshipView
                       {
                           CompanyType = p.Company.CompanyType,
                           Id = p.Id,
                           PostedOn = (DateTime)p.CreatedDate,
                           ContactEmail = p.ContactEmail,
                           ContactMobile = p.ContactMobile,
                           C_Name = p.Company.Name,
                           AboutCompany = p.Company.About,
                           StartDate = p.StartDate,
                           ApplyBefore = p.ApplyBefore,
                           Stipend = p.Stipend,
                           InternshipType = p.InternshipType,
                           InternshipLocations = (p.InternshipLocations.ToList()).Select(c => c.LocationsList.City).ToList(),
                           InternshipPerks = (p.InternshipPerks.ToList()).Select(
                                                item => new InternshipView.Perk
                                                {
                                                    Name = item.PerksList.Name,
                                                    Description = item.PerksList.Description
                                                }
                                              ).ToList(),
                           InternshipSkills = (p.InternshipSkills.ToList()).Select(c => c.SkillsList.Name).ToList(),
                           About = p.About,
                           InternshipIn = p.CategoriesList.Name,
                           AvailableSeats = p.AvailableSeats,
                           MinDuration = p.MinDuration,
                           MaxDuration = p.MaxDuration,
                           stud = (p.Applieds.ToList()).Select(c => new InternshipView.Student
                           {
                               S_Id = c.S_Id,
                               S_Name = c.Student.FirstName + " " + c.Student.LastName,
                               Status = c.Status
                           }).ToList()
                       }
                   ).OrderBy(p => p.ApplyBefore).FirstOrDefault();
            if (internship == null)
            {
                return NotFound();
            }

            return Ok(internship);
        }

        [HttpGet]
        [Route("api/adminAPI/Project")]
        public IHttpActionResult GetProject(string projectid)
        {

            ProjectView project = db.Projects.Where(p => p.Id == projectid).Select(
                p => new ProjectView
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.CategoriesList.Name,
                    Pic = p.Pic,
                    CreatedById = p.CreatedBy,
                    CreatedByName = p.Student.FirstName + " " + p.Student.LastName,
                    ProjectOverview = p.ProjectOverview,
                    TeamLooking = p.TeamLooking,
                    ProjectHelp = p.ProjectHelp,
                    CountUsers = p.ProjectUsers.Count,
                    ProjectJoins = (p.Joins.ToList()).Select(c => new ProjectView.Join
                    {
                        S_Id = c.S_Id,
                        Pic = c.Student.ProfilePic,
                        Name = c.Student.FirstName + " " + c.Student.LastName,
                        Status = c.Status
                    }).ToList(),
                    ProjectUsers = (p.ProjectUsers.ToList()).Select(c => new ProjectView.User
                    {
                        UserId = c.U_Id,
                        Pic = c.Student.ProfilePic,
                        UserName = c.Student.FirstName + " " + c.Student.LastName,
                        Role = c.RoleList.Name
                    }).ToList(),
                    ProjectTasks = (p.ProjectTasks.ToList()).Select(c => new ProjectView.Tasks
                    {
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
                ).FirstOrDefault();

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpGet]
        [Route("api/adminAPI/Company")]
        public IHttpActionResult GetCompany(string companyid)
        {
            CompanyView company = db.Companies.Where(item => item.Id == companyid).Select(
                c => new CompanyView
                {
                    Id = c.Id,
                    Name = c.Name,
                    About = c.About,
                    CompanyType = c.CompanyType,
                    Logo = c.Logo,
                    WebsiteURL = c.WebsiteURL,
                    CompanyIndustry = c.CompanyIndustries.Select(s => s.IndustryList.Name).ToList<string>(),
                    GoogleId = c.GoogleId,
                    Internships = (c.Internships.ToList()).Select(s => new CompanyView.Internship
                    {
                        I_Id = s.Id,
                        InternshipIn = s.CategoriesList.Name,
                        CreatedDate = s.CreatedDate,
                        Applicants = (s.Applieds.ToList()).Count(),
                        Locations = (s.InternshipLocations.ToList()).Select(l => l.LocationsList.City).ToList()
                    }).ToList()
                }
                ).FirstOrDefault();
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        [HttpGet]
        [Route("api/adminAPI/Dashboard")]
        public IHttpActionResult GetDashboard()
        {
            DateTime time = db.Logs.Where(c => c.UserType == "ADM" && c.UserId == "Admin" && c.Link == "http://localhost:42640/Admin/Logout").OrderByDescending(b=>b.Time).Select(item => item.Time).FirstOrDefault();
            int students = db.Students.Where(item => item.RegTime >= time).Count();
            int companies = db.Companies.Where(item => item.RegTime >= time).Count();
            int projects = db.Projects.Where(item => item.CreatedDate >= time).Count();
            int internships = db.Internships.Where(item => item.CreatedDate >= time).Count();

            ADashboard ad = new ADashboard();
            ad.CountMsgs = db.Contacts.Where(item => item.Read == false).Count();
            ad.CountUsers = students + companies;
            ad.CountPosts = projects + internships;

            return Ok(ad);
        }

        [HttpGet]
        [Route("api/adminAPI/Messages")]
        public IEnumerable<Contact> GetMessages()
        {
            return db.Contacts.OrderBy(item=>item.Read).ThenBy(item => item.Time).ToList();
        }

        [HttpGet]
        [Route("api/adminAPI/DeleteStudent")]
        public IHttpActionResult DeleteStudent(string studentid)
        {
            foreach(TasksAssigned ta in db.TasksAssigneds.Where(item=>item.ProjectUser.U_Id.Equals(studentid)))
            {
                db.TasksAssigneds.Remove(ta);
            }
            foreach(ProjectUser pu in db.ProjectUsers.Where(item => item.U_Id.Equals(studentid)))
            {
                db.ProjectUsers.Remove(pu);
            }
            foreach(Join j in db.Joins.Where(item => item.S_Id.Equals(studentid)))
            {
                db.Joins.Remove(j);
            }
            foreach(Applied a in db.Applieds.Where(item => item.S_Id.Equals(studentid)))
            {
                db.Applieds.Remove(a);
            }
            foreach (string pid in db.Projects.Where(item => item.CreatedBy.Equals(studentid)).Select(p=>p.Id))  
            {
                foreach (TasksAssigned ta in db.TasksAssigneds.Where(item => item.ProjectTask.P_Id.Equals(pid)))
                {
                    db.TasksAssigneds.Remove(ta);
                }
                foreach (ProjectUser pu in db.ProjectUsers.Where(item => item.P_Id.Equals(pid)))
                {
                    db.ProjectUsers.Remove(pu);
                }
                foreach(ProjectTask pt in db.ProjectTasks.Where(item => item.P_Id.Equals(pid)))
                {
                    db.ProjectTasks.Remove(pt);
                }
                foreach (Join j in db.Joins.Where(item => item.P_Id.Equals(pid)))
                {
                    db.Joins.Remove(j);
                }
                db.Projects.Remove(db.Projects.Find(pid));
            }
            db.Students.Remove(db.Students.Find(studentid));
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Ok("error");
            }
            return Ok("success");
        }

        [HttpGet]
        [Route("api/adminAPI/DeleteCompany")]
        public IHttpActionResult DeleteCompany(string companyid)
        {
            foreach(string iid in db.Internships.Where(item => item.C_Id.Equals(companyid)).Select(i => i.Id))
            {
                foreach (Applied a in db.Applieds.Where(item => item.I_Id.Equals(iid)))
                {
                    db.Applieds.Remove(a);
                }
                foreach (InternshipPerk ip in db.InternshipPerks.Where(item => item.I_Id.Equals(iid)))
                {
                    db.InternshipPerks.Remove(ip);
                }
                foreach (InternshipSkill ins in db.InternshipSkills.Where(item => item.I_Id.Equals(iid)))
                {
                    db.InternshipSkills.Remove(ins);
                }
                foreach (InternshipLocation il in db.InternshipLocations.Where(item => item.I_Id.Equals(iid)))
                {
                    db.InternshipLocations.Remove(il);
                }

                db.Internships.Remove(db.Internships.Find(iid));
            }
            db.Companies.Remove(db.Companies.Find(companyid));
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Ok("error");
            }
            return Ok("success");
        }

        [HttpGet]
        [Route("api/adminAPI/DeleteInternship")]
        public IHttpActionResult DeleteInternship(string internshipid)
        {
            foreach (Applied a in db.Applieds.Where(item => item.I_Id.Equals(internshipid)))
            {
                db.Applieds.Remove(a);
            }
            foreach (InternshipPerk ip in db.InternshipPerks.Where(item => item.I_Id.Equals(internshipid)))
            {
                db.InternshipPerks.Remove(ip);
            }
            foreach (InternshipSkill ins in db.InternshipSkills.Where(item => item.I_Id.Equals(internshipid)))
            {
                db.InternshipSkills.Remove(ins);
            }
            foreach (InternshipLocation il in db.InternshipLocations.Where(item => item.I_Id.Equals(internshipid)))
            {
                db.InternshipLocations.Remove(il);
            }
            
            db.Internships.Remove(db.Internships.Find(internshipid));
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Ok("error");
            }
            return Ok("success");
        }

        [HttpGet]
        [Route("api/adminAPI/DeleteProject")]
        public IHttpActionResult DeleteProject(string projectid)
        {
            foreach (TasksAssigned ta in db.TasksAssigneds.Where(item => item.ProjectUser.P_Id.Equals(projectid)))
            {
                db.TasksAssigneds.Remove(ta);
            }
            foreach (ProjectUser pu in db.ProjectUsers.Where(item => item.P_Id.Equals(projectid)))
            {
                db.ProjectUsers.Remove(pu);
            }
            foreach (ProjectTask pt in db.ProjectTasks.Where(item => item.P_Id.Equals(projectid)))
            {
                db.ProjectTasks.Remove(pt);
            }
            foreach (Join j in db.Joins.Where(item => item.P_Id.Equals(projectid)))
            {
                db.Joins.Remove(j);
            }
            db.Projects.Remove(db.Projects.Find(projectid));
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Ok("error");
            }
            return Ok("success");
        }

        [HttpGet]
        [Route("api/adminAPI/VerifyStudent")]
        public IHttpActionResult VerifyStudent(string studentid)
        {
            Student student = db.Students.Find(studentid);
            student.Verified = true;
            db.Entry(student).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Ok("error");
            }
            return Ok("success");
        }

        [HttpGet]
        [Route("api/adminAPI/VerifyCompany")]
        public IHttpActionResult VerifyCompany(string companyid)
        {
            Company company = db.Companies.Find(companyid);
            company.Verified = true;
            db.Entry(company).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Ok("error");
            }
            return Ok("success");
        }

        [HttpGet]
        [Route("api/adminAPI/ReadMsg")]
        public IHttpActionResult ReadMsg(int id)
        {
            Contact contact = db.Contacts.Find(id);
            contact.Read = true;
            db.Entry(contact).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Ok("error");
            }
            return Ok("success");
        }
    }

}