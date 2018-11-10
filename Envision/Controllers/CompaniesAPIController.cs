using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Envision.Models;

namespace Envision.Controllers
{
    public class CompaniesAPIController : ApiController
    {
        private EnvisionEntities db = new EnvisionEntities();

        // GET: api/CompaniesAPI
        public IQueryable<CompanyView> GetCompanies()
        {
            return db.Companies.Select(
                c=> new CompanyView
                {
                    Id =c.Id,
                    Verified = c.Verified,
                    RegTime = c.RegTime,
                    Logo = c.Logo,
                    Name = c.Name,
                    Email = c.Email,
                    CompanyType = c.CompanyType,
                    WebsiteURL = c.WebsiteURL,
                    GoogleId = c.GoogleId
                }
                );
        }

        // GET: api/CompaniesAPI/5
        [ResponseType(typeof(CompanyView))]
        public IHttpActionResult GetCompany(string id)
        {
            CompanyView company = db.Companies.Where(item=> item.Id == id).Select(
                c => new CompanyView
                {
                    Id = c.Id,
                    Name = c.Name,
                    About = c.About,
                    CompanyType = c.CompanyType,
                    Logo = c.Logo,
                    WebsiteURL = c.WebsiteURL,
                    CompanyIndustry = c.CompanyIndustries.Select(s => s.IndustryList.Name).ToList<string>(),
                    GoogleId = c.GoogleId
                }
                ).FirstOrDefault();
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        // Get: api/CompaniesAPI/
        [ResponseType(typeof(string))]
        public IHttpActionResult GetLogin(string email,string password)
        {
            Boolean flag = db.Companies.Any(b => b.Email==email && b.Password==password);
            if (flag)
            {
                return Ok(db.Companies.Where(b => b.Email == email).Select(b => b.Id).FirstOrDefault().ToString());
            }
            return Ok("error");
        }

        // PUT: api/CompaniesAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCompany(Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Company c = db.Companies.Find(company.Id);

            c.Name = company.Name;
            c.GoogleId = company.GoogleId;
            c.About = company.About;
            if (company.CompanyIndustries != null)
            {
                foreach(CompanyIndustry temp in db.CompanyIndustries.Where(ci=>ci.C_Id == company.Id))
                {
                    db.CompanyIndustries.Remove(temp);
                }
                foreach (CompanyIndustry tempc in company.CompanyIndustries)
                {
                    tempc.C_Id = company.Id;
                    db.CompanyIndustries.Add(tempc);
                }
            }
            
            if(company.Logo != null)
            {
                c.Logo = company.Logo;
            }
            db.SaveChanges();

            db.Entry(c).State = EntityState.Modified;
            
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(company.Id))
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

        // POST: api/CompaniesAPI
        [ResponseType(typeof(string))]
        public IHttpActionResult PostCompany(Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            company.Email = company.Email.ToLower();
            company.RegTime = System.DateTime.Now;
           


            if (CompanyEmailExists(company.Email))
            {
                return Ok("email exists");
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            Boolean flag = true;
            Random random = new Random();
            while (flag)
            {
                company.Id = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
                if (!CompanyExists(company.Id))
                {
                    flag = false;
                }
            }

            if(company.CompanyIndustries != null)
            {
                foreach(CompanyIndustry tempc in company.CompanyIndustries)
                {
                    tempc.C_Id = company.Id;
                }
            }

            db.Companies.Add(company);

            foreach (CompanyIndustry tempc in company.CompanyIndustries)
            {
                db.CompanyIndustries.Add(tempc);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok(company.Id);
        }

        // DELETE: api/CompaniesAPI/5
        [ResponseType(typeof(Company))]
        public IHttpActionResult DeleteCompany(string id)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return NotFound();
            }

            db.Companies.Remove(company);
            db.SaveChanges();

            return Ok(company);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompanyExists(string id)
        {
            return db.Companies.Count(e => e.Id == id) > 0;
        }

        private bool CompanyEmailExists(string email)
        {
            return db.Companies.Count(e => e.Email == email) > 0;
        }
    }
}