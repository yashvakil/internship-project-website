using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Envision.Models;
using System.Globalization;

namespace Envision.Controllers
{
    public class InternshipsAPIController : ApiController
    {
        private EnvisionEntities db = new EnvisionEntities();

        // GET: api/InternshipsAPI
        public IQueryable<InternshipView> GetInternships()
        {
            
            return db.Internships.Include(b => b.Company).Include(b => b.CategoriesList).Include(b => b.InternshipLocations).Select(
                    p => new InternshipView
                    {
                        Id = p.Id,
                        CompanyType = p.Company.CompanyType,
                        PostedOn = (DateTime)p.CreatedDate,
                        C_Name = p.Company.Name,
                        StartDate = p.StartDate,
                        ApplyBefore = p.ApplyBefore,
                        Stipend = p.Stipend,
                        InternshipType = p.InternshipType,
                        InternshipLocations =(p.InternshipLocations.ToList()).Select(c=>c.LocationsList.City).ToList(),
                        InternshipIn = p.CategoriesList.Name,
                        AvailableSeats = p.AvailableSeats,
                        MinDuration = p.MinDuration,
                        MaxDuration = p.MaxDuration,
                    }
                ).Where(p=>p.ApplyBefore>=DateTime.Now).OrderBy(p => p.ApplyBefore);
        }

        public IQueryable<InternshipView> GetInternships(string companyid)
        {

            return db.Internships.Include(b => b.Company).Include(b => b.CategoriesList).Include(b => b.InternshipLocations).Where(b=>b.C_Id == companyid).Select(
                    p => new InternshipView
                    {
                        Id = p.Id,
                        CompanyType = p.Company.CompanyType,
                        PostedOn = (DateTime)p.CreatedDate,
                        C_Name = p.Company.Name,
                        StartDate = p.StartDate,
                        ApplyBefore = p.ApplyBefore,
                        Stipend = p.Stipend,
                        InternshipType = p.InternshipType,
                        InternshipLocations = (p.InternshipLocations.ToList()).Select(c => c.LocationsList.City).ToList(),
                        InternshipIn = p.CategoriesList.Name,
                        AvailableSeats = p.AvailableSeats,
                        MinDuration = p.MinDuration,
                        MaxDuration = p.MaxDuration,
                    }
                ).OrderBy(p => p.ApplyBefore);
        }

        // GET: api/InternshipsAPI/5
        [ResponseType(typeof(InternshipView))]
        public IHttpActionResult GetInternship(string id)
        {
            InternshipView internship = db.Internships.Include(b => b.Company).Include(b => b.CategoriesList).Include(b => b.InternshipLocations).Include(b => b.InternshipPerks).Include(b => b.InternshipSkills).Where(item => item.Id == id).Select(
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
                       }
                   ).OrderBy(p => p.ApplyBefore).FirstOrDefault();
            if (internship == null)
            {
                return NotFound();
            }

            return Ok(internship);
        }

        // PUT: api/InternshipsAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutInternship(string id, Internship internship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != internship.Id)
            {
                return BadRequest();
            }

            db.Entry(internship).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InternshipExists(id))
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

        // POST: api/InternshipsAPI
        [ResponseType(typeof(Internship))]
        public IHttpActionResult PostInternship(Internship internship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            internship.ContactEmail = internship.ContactEmail.ToLower();
            internship.CreatedDate = System.DateTime.Now;
            internship.AvailableSeats = Convert.ToDecimal(internship.AvailableSeats);
            internship.MinDuration = Convert.ToDecimal(internship.MinDuration);
            internship.MaxDuration = Convert.ToDecimal(internship.MaxDuration);

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            Boolean flag = true;
            Random random = new Random();
            if (flag)
            {
                internship.Id = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
                if (!InternshipExists(internship.Id))
                {
                    flag = false;
                }
            }

            if (internship.InternshipLocations != null)
            {
                foreach (InternshipLocation tempc in internship.InternshipLocations)
                {
                    tempc.I_Id = internship.Id;
                    tempc.Time = System.DateTime.Now;
                }
            }

            if (internship.InternshipPerks != null)
            {
                foreach (InternshipPerk tempc in internship.InternshipPerks)
                {
                    tempc.I_Id = internship.Id;
                    tempc.Time = System.DateTime.Now;
                }
            }

            if (internship.InternshipSkills != null)
            {
                foreach (InternshipSkill tempc in internship.InternshipSkills)
                {
                    tempc.I_Id = internship.Id;
                    tempc.Time = System.DateTime.Now;
                }
            }
            
            db.Internships.Add(internship);

            
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok("success");
        }

        // DELETE: api/InternshipsAPI/5
        [ResponseType(typeof(Internship))]
        public IHttpActionResult DeleteInternship(string id)
        {
            
            Internship internship = db.Internships.Where(item=>item.Id==id).First();
            if (internship == null)
            {
                return NotFound();
            }

            db.Internships.Remove(internship);
            db.SaveChanges();

            return Ok("success");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InternshipExists(string id)
        {
            return db.Internships.Count(e => e.Id == id) > 0;
        }
    }
}