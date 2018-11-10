using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class ADashboard
    {
        public ADashboard()
        {
            data = new System.Data.DataTable();
        }
        public int CountUsers { get; set; }
        public int CountMsgs { get; set; }
        public int CountSessions { get; set; }
        public int CountPosts { get; set; }


        public string Query { get; set; }
        public string Type { get; set; }
        public string Error { get; set; }
        public System.Data.DataTable data { get; set; }
        public string Msg { get; set; }
    }
}