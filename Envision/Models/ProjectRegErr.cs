using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class ProjectRegErr
    {
        public ProjectRegErr()
        {
            p = new Project();
        }
        
        public string ProjectOverview { get; set; }
        public string TeamLooking { get; set; }
        public string Pic { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ProjectHelp { get; set; }

        public Project p;
    }
}