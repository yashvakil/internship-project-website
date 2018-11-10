using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class AppliedView
    {
        public class Student
        {
            public string S_Id { get; set; }
            public string S_Name { get; set; }
            public string Status { get; set; }
        }
        public string I_Id { get; set; }
        public string I_Name { get; set; }
        public DateTime ApplyBefore { get; set; }
        public List<Student> stud = new List<Student>();
    }
}