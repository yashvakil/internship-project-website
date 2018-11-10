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
    public class ProjectsAPIController : ApiController
    {
        private EnvisionEntities db = new EnvisionEntities();
        

        // GET: api/Projects
        public IQueryable<ProjectView> GetProjects()
        {
            return db.Projects.Select(
                p => new ProjectView
                {
                    Category = p.CategoriesList.Name,
                    Name = p.Name,
                    CreatedById = p.CreatedBy,
                    CreatedByName = p.Student.FirstName + " " +p.Student.LastName,
                    CountUsers = p.ProjectUsers.Count + 1,
                    Id = p.Id,
                    ProjectOverview = p.ProjectOverview,
                    Pic = p.Pic,
                    CreatedDate = p.CreatedDate
                }
               ).OrderBy(p => p.CreatedDate);
        }

        public IQueryable<ProjectView> GetProjects(string category)
        {
            return db.Projects.Where(b=>b.CategoriesList.Name==category).Select(
                p => new ProjectView
                {
                    Category = category,
                    Name = p.Name,
                    Id = p.Id,
                    CreatedDate = p.CreatedDate,
                    ProjectOverview = p.ProjectOverview,
                    Pic = p.Pic,
                    CountUsers = p.ProjectUsers.Count + 1
                }
               ).OrderBy(p => p.CreatedDate);
        }

        public IQueryable<ProjectView> GetProjectsUser(string userid)
        {
            return db.Projects.Where(b =>b.CreatedBy == userid || b.ProjectUsers.Select(c=>c.U_Id).ToList().Contains(userid)).Select(
                p => new ProjectView
                {
                    Category = p.CategoriesList.Name,
                    Name = p.Name,
                    CreatedDate = p.CreatedDate,
                    Id = p.Id,
                    ProjectOverview = p.ProjectOverview,
                    CreatedById = p.CreatedBy,
                    CreatedByName = p.Student.FirstName + " " + p.Student.LastName,
                    Pic = p.Pic,
                    TeamLooking = p.TeamLooking,
                    CountUsers = p.ProjectUsers.Count
                }
               ).OrderBy(p => p.CreatedDate);
        }

        // GET: api/Projects/5
        [ResponseType(typeof(ProjectView))]
        public IHttpActionResult GetProject(string id)
        {
           
            ProjectView project = db.Projects.Where(p => p.Id == id).Select(
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
                    ProjectUsers = (p.ProjectUsers.ToList()).Select(c => new ProjectView.User
                    {
                        UserId = c.U_Id,
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
                        Status = c.Status
                    }).ToList()
                }
                ).FirstOrDefault();
            
            if(project==null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        

        // PUT: api/Projects/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProject(Project p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Project project = db.Projects.Find(p.Id);
            if(p.ProjectOverview != null)
            {
                project.ProjectOverview = p.ProjectOverview;
            }
            if (p.TeamLooking != null)
            {
                project.TeamLooking = p.TeamLooking;
            }
            if (p.ProjectHelp != null)
            {
                project.ProjectHelp = p.ProjectHelp;
            }
                
            if(p.Pic != null)
            {
                project.Pic = p.Pic;
            }

            db.Entry(project).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(p.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Projects
        [ResponseType(typeof(Project))]
        public IHttpActionResult PostProject(Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            project.CreatedDate = System.DateTime.Now;

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            Boolean flag = true;
            Random random = new Random();
            while (flag)
            {
                project.Id = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
                if (!ProjectExists(project.Id))
                {
                    flag = false;
                }
            }

            db.Projects.Add(project);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ProjectExists(project.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok("success");
        }

        // DELETE: api/Projects/5
        [ResponseType(typeof(Project))]
        public IHttpActionResult DeleteProject(string id)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }

            db.Projects.Remove(project);
            db.SaveChanges();

            return Ok(project);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProjectExists(string id)
        {
            return db.Projects.Count(e => e.Id == id) > 0;
        }
    }
}