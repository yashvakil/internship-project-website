using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class InternshipView
    {
        public class Student
        {
            public string S_Id { get; set; }
            public string S_Name { get; set; }
            public string Status { get; set; }
        }

        public class Perk
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public string CompanyType { get; set; }
        public string AboutCompany { get; set; }
        public System.DateTime PostedOn { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMobile { get; set; }
        public string Id { get; set; }
        public string C_Name { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public System.DateTime ApplyBefore { get; set; }
        public Nullable<int> Stipend { get; set; }
        public string InternshipType { get; set; }
        public string InternshipIn { get; set; }
        public decimal AvailableSeats { get; set; }
        public decimal MinDuration { get; set; }
        public decimal MaxDuration { get; set; }
        public List<string> InternshipLocations { get; set; }
        public List<Perk> InternshipPerks { get; set; }
        public List<string> InternshipSkills { get; set; }
        public string About { get; set; }
        public List<Student> stud { get; set; }
    }
}