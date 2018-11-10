using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class StudentView
    {   
        public class ProjectJoin
        {
            public string P_Id;
            public string Name;
            public string Category;
            public string CreatedBy;
            public string Status;
        }
        public class ProjectUser
        {
            public string P_Id;
            public string Name;
            public string Category;
            public string CreatedBy;
            public string Role;
        }
        public class InternshipApplied
        {
            public string I_Id;
            public string InternshipIn;
            public string C_Id;
            public string C_Name;
            public string Status;
        }
        public class Studied
        {
            public string Id;
            public string Name;
            public string Degree;
            public Nullable<decimal> GraduationYear;
        }
        public class WorkExperience
        {
            public string Id;
            public string Name;
            public DateTime StartDate;
            public Nullable<DateTime> EndDate;
            public string Designation;
        }
        public class VolunteerWork
        {
            public string Id;
            public string Name;
            public DateTime StartDate;
            public Nullable<DateTime> EndDate;
            public string Topic;
        }
        public class SkilledIn
        {
            public int Id;
            public string Name;
            public decimal Value;
        }

        public StudentView()
        {
            WorkExperiences = new List<WorkExperience>();
            Studieds = new List<Studied>();
            VolunteerWorks = new List<VolunteerWork>();
            SkilledIns = new List<SkilledIn>();
        }
        public string Id { get; set; }
        public bool Verified { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public byte[] ProfilePic { get; set; }
        public byte[] CardFront { get; set; }
        public byte[] CardBack { get; set; }
        public System.DateTime RegTime { get; set; }
        public System.DateTime DOB { get; set; }

        public List<InternshipApplied> Applieds;
        public List<SkilledIn> SkilledIns;
        public List<WorkExperience> WorkExperiences;
        public List<VolunteerWork> VolunteerWorks;
        public List<Studied> Studieds;
        public List<ProjectJoin> ProjectJoins;
        public List<ProjectUser> ProjectUsers;
    }
}