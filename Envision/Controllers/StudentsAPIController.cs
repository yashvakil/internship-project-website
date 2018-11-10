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
    public class StudentsAPIController : ApiController
    {
        private EnvisionEntities db = new EnvisionEntities();

        // GET: api/StudentsAPI
        public IQueryable<StudentView> GetStudents()
        {
            return db.Students.Select(
                s=>new StudentView
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    Verified = s.Verified,
                    RegTime = s.RegTime,
                    ProfilePic = s.ProfilePic,
                    Gender = s.Gender
                }
                );
        }

        // GET: api/StudentsAPI/5
        [ResponseType(typeof(StudentView))]
        public IHttpActionResult GetStudent(string id)
        {
            
            StudentView student = db.Students.Include(b => b.Studieds).Include(b => b.SkilledIns).Include(b => b.VolunteerWorks).Include(b => b.WorkExperiences).Where(item => item.Id == id).Select(
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
                    WorkExperiences = (p.WorkExperiences.ToList()).Select(w => new StudentView.WorkExperience {
                        Name = w.Name,
                        StartDate = w.StartDate,
                        EndDate = w.EndDate,
                        Designation = w.Designation
                    }).Select(o => new { Obj = o, EndDate = o.EndDate ?? DateTime.MaxValue }).OrderByDescending(o => o.EndDate).Select(o => o.Obj).ToList(),
                    VolunteerWorks = (p.VolunteerWorks.ToList()).Select(v => new StudentView.VolunteerWork {
                        Name = v.Name,
                        StartDate = v.StartDate,
                        EndDate = v.EndDate,
                        Topic = v.Topic
                    }).Select(o => new { Obj = o, EndDate = o.EndDate ?? DateTime.MaxValue }).OrderByDescending(o => o.EndDate).Select(o => o.Obj).ToList(),
                    SkilledIns = (p.SkilledIns.ToList()).Select(s => new StudentView.SkilledIn{
                       Id = s.S_ID,
                       Name = s.SkillsList.Name,
                       Value = s.Value
                    }).ToList(),
                    Studieds = (p.Studieds.ToList()).Select(s=> new StudentView.Studied
                    {
                        Degree = s.DegreeList.Name,
                        Name = s.Name,
                        GraduationYear = s.GraduationYear
                    }).Select(o => new { Obj = o, GraduationYear = o.GraduationYear ?? Decimal.MaxValue }).OrderByDescending(o => o.GraduationYear).Select(o => o.Obj).ToList()
                }
                
                ).FirstOrDefault();
            if (student == null)
            {
                return NotFound();
            }    

            return Ok(student);
        }

        public IQueryable<StudentView> GetApplicants(string internshipid)
        {
            return db.Applieds.Include(b => b.Student).Where(item => item.I_Id == internshipid).Select(
                p => new StudentView
                {
                    Id = p.S_Id,
                    FirstName = p.Student.FirstName,
                    LastName = p.Student.LastName,
                    Email = p.Status,
                    Mobile = internshipid
                    
                }

            );
           
            
            
        }

        // Get: api/CompaniesAPI/
        [ResponseType(typeof(string))]
        public IHttpActionResult GetLogin(string email, string password)
        {
            Boolean flag = db.Students.Any(b => b.Email == email && b.Password == password);
            if (flag)
            {
                return Ok(db.Students.Where(b => b.Email == email).Select(b => b.Id).FirstOrDefault().ToString());
            }
            return Ok("error");
        }
        
        // PUT: api/StudentsAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStudent(Student s)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Student student = db.Students.Find(s.Id);
            if (s.DOB != null)
            {
                student.DOB = s.DOB;
            }

            if(s.ProfilePic != null)
            {
                student.ProfilePic = s.ProfilePic;
            }
            if (!string.IsNullOrWhiteSpace(s.Email))
            {
                student.Email = s.Email;
            }

            student.Mobile = s.Mobile;

            if (!string.IsNullOrWhiteSpace(s.FirstName))
            {
                student.FirstName = s.FirstName;
            }
            if (!string.IsNullOrWhiteSpace(s.LastName))
            {
                student.LastName = s.LastName;
            }

            db.Entry(student).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(s.Id))
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

        // POST: api/StudentsAPI
        [ResponseType(typeof(Student))]
        public IHttpActionResult PostStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            student.Email = student.Email.ToLower();
            student.FirstName = cultInfo.ToTitleCase(student.FirstName);
            student.LastName = cultInfo.ToTitleCase(student.LastName);
            student.RegTime = System.DateTime.Now;
            student.Verified = false;

            if (StudentEmailExists(student.Email))
            {
                return Ok("email exists") ;
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            Boolean flag = true;
            Random random = new Random();
            while (flag)
            {
                student.Id = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
                if (!StudentExists(student.Id))
                {
                    flag = false;
                }
            }
            

            db.Students.Add(student);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok(student.Id);
        }

        // DELETE: api/StudentsAPI/5
        [ResponseType(typeof(Student))]
        public IHttpActionResult DeleteStudent(string id)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            db.Students.Remove(student);
            db.SaveChanges();

            return Ok(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StudentExists(string id)
        {
            return db.Students.Count(e => e.Id == id) > 0;
        }

        private bool StudentEmailExists(string email)
        {
            return db.Students.Count(e => e.Email == email) > 0;
        }
    }
}