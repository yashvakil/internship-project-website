using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class InternshipFilter
    {
        public string StartUp { get; set; }
        public string Established { get; set; }
        public string Ngo { get; set; }
        public string Gov { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string MaxDuration { get; set; }
        public string StartMin { get; set; }
        public string StartMax { get; set; }
    }
}