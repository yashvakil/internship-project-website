using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class InternshipRegErr
    {
        public InternshipRegErr()
        {
            i = new Internship();
        }
        
        public string About { get; set; }
        public string ApplyBefore { get; set; }
        public string InternshipType { get; set; }
        public string InternshipIn { get; set; }
        public string AvailableSeats { get; set; }
        public string MinDuration { get; set; }
        public string MaxDuration { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMobile { get; set; }
        public string StartDate { get; set; }
        public string Category { get; set; }
        public string Error { get; set; }
        public string Locations { get; set; }


        public Internship i;
    }
}