using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class StudentRegErr
    {
        public StudentRegErr()
        {
            s = new Student();
        }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public string CardFront { get; set; }
        public string CardBack { get; set; }

        public Student s;
    }
}