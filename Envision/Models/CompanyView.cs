using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class CompanyView
    {
        public class Internship
        {
            public string I_Id;
            public string InternshipIn;
            public int Applicants;
            public DateTime CreatedDate;
            public List<string> Locations;
        }
           
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string WebsiteURL { get; set; }
        public string GoogleId { get; set; }
        public byte[] Logo { get; set; }
        public bool Verified { get; set; }
        public DateTime RegTime { get; set; }
        public string CompanyType { get; set; }
        public string About { get; set; }
        public List<string> CompanyIndustry { get; set; }
        public List<Internship> Internships { get; set; }
    }
}