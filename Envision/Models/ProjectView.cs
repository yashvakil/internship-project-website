using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class ProjectView
    {
        public class Join
        {

            public byte[] Pic { get; set; }
            public string S_Id { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
        }

        public class User
        {
            public string UserId { get; set; } 
            public byte[] Pic { get; set; }
            public string UserName { get; set; }
            public string Role { get; set; }
        }

        public class Tasks
        {
            public class User
            {
                public byte[] Pic { get; set; }
                public string UserId { get; set; }
                public string Name { get; set; } 
            }

            public string Id { get; set; }
            public string Heading { get; set; }
            public System.DateTime DeadLine { get; set; }
            public string Status { get; set; }
            public string Description { get; set; }

            public List<User> UserTasks { get; set; }
            
        }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string ProjectOverview { get; set; }
        public string TeamLooking { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public byte[] Pic { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Int32 CountUsers { get; set; }
        public string ProjectHelp { get; set; }
        public List<User> ProjectUsers { get; set; }
        public List<Tasks> ProjectTasks { get; set; }
        public List<Join> ProjectJoins{ get; set; }
    }
}